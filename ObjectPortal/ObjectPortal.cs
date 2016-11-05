using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Core;
using Autofac.Builder;
using Autofac;
using System.Reflection;
using System.Linq.Expressions;

namespace ObjectPortal
{

    public static class ObjectPortal
    {


        public static void ObjectPortalFetch(this ContainerBuilder builder, Type delegateType)
        {

            // Some serious WTF code!

            // We assume delegateType is a Delegate. 
            // The business object type we need is the return type of Delegate.Invoke.
            var invoke = delegateType.GetMethod("Invoke");
            var boType = invoke.ReturnType;

            // Need to resolve an IObjectPortal<BusinessObjectType>
            var opType = typeof(IObjectPortal<>).MakeGenericType(boType);

            var fetchMethods = typeof(ObjectPortal<>)
                .MakeGenericType(boType)
                .GetMethods()
                .Where(x => x.Name == "Fetch").ToList();
            MethodInfo fetchMethod = null;

            // ObjectPortal concrete has two fetch methods
            // One that takes parameters and one that doesn't
            // Chose the right one based on our delegate
            if(invoke.GetParameters().Count() == 0)
            {
                fetchMethod = fetchMethods.Where(x => x.GetParameters().Count() == 0).First();
            } else
            {
                fetchMethod = fetchMethods.Where(x => x.GetParameters().Count() > 0).First();
            }

            // We need to match the delegateType signature
            // So Fetch<C> needs to be Fetch<Criteria>
            // Again we look to our invoke for this
            // Brittle: Only one parameter
            if (fetchMethod.IsGenericMethod)
            {
                fetchMethod = fetchMethod.MakeGenericMethod(invoke.GetParameters().First().ParameterType);
            }

            builder.Register((c) =>
            {
                var portal = c.Resolve(opType);

                return Convert.ChangeType(Delegate.CreateDelegate(delegateType, portal, fetchMethod), delegateType);

            }).As(delegateType);

        }
    }

    /// <summary>
    /// Abstract BO object creating, fetching and updating each other
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPortal<T> : IObjectPortal<T>
    {



        Func<T> createT;
        Func<DependencyManager> createDM;

        public ObjectPortal(Func<T> createT, Func<DependencyManager> createDM)
        {
            this.createT = createT;
            this.createDM = createDM;
        }

        public T Fetch()
        {

            // Thought - For root operations should this create a new scope?? Then when will it be disposed???
            // Example: For 2-Tier applications how will an Update use a single .InstancePerLifetimeScope sql connection and transaction??

            // This is also where we would enforce create authorization rules

            var result = createT();

            var fetch = result as IHandleObjectPortalFetch;

            if (fetch == null)
            {
                throw new ObjectPortalOperationNotSupportedException("Fetch with no criteria not supported");
            }

            var dtoBB = result as IDPBusinessObject;

            if (dtoBB != null)
            {
                dtoBB.DependencyManager = createDM();
            }

            fetch.Fetch();

            return result;

        }

        public T Fetch<C>(C criteria)
        {

            var result = createT();

            var fetch = result as IHandleObjectPortalFetch<C>;

            if (fetch == null)
            {
                throw new ObjectPortalOperationNotSupportedException("Fetch with no criteria not supported");
            }

            var dtoBB = result as IDPBusinessObject;

            if (dtoBB != null)
            {
                dtoBB.DependencyManager = createDM();
            }

            fetch.Fetch(criteria);

            return result;

        }

    }


    [Serializable]
    public class ObjectPortalOperationNotSupportedException : Exception
    {
        public ObjectPortalOperationNotSupportedException() { }
        public ObjectPortalOperationNotSupportedException(string message) : base(message) { }
        public ObjectPortalOperationNotSupportedException(string message, Exception inner) : base(message, inner) { }
        protected ObjectPortalOperationNotSupportedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
