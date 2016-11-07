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

        private delegate void Nothing();

        public static void ObjectPortalFetch(this ContainerBuilder builder, Type delegateType)
        {

            // Some serious WTF code!

            if (delegateType == null)
            {
                throw new ArgumentNullException(nameof(delegateType));
            }

            if (!typeof(Delegate).IsAssignableFrom(delegateType))
            {
                throw new Exception("Only Delegates types allowed.");
            }


            // We assume delegateType is a Delegate. 
            // The business object type we need is the return type of Delegate.Invoke.
            var invoke = delegateType.GetMethod(nameof(Nothing.Invoke)); // Better way to get "Invoke"???

            var boType = invoke.ReturnType;


            if (invoke == null)
            {
                throw new Exception($"Unable to load invoke method on ${delegateType.Name}");
            }

            var parameterCount = invoke.GetParameters().Count();

            if (parameterCount > 1)
            {
                throw new Exception($"Delegate ${delegateType.Name} cannot have more than one method parameter.");
            }

            // Need to resolve an IObjectPortal<BusinessObjectType>
            var opType = typeof(IObjectPortal<>).MakeGenericType(boType);

            MethodInfo fetchMethod = null;

            // ObjectPortal concrete has two fetch methods
            // One that takes parameters and one that doesn't
            // Chose the right one based on the parameters our delegate
            if (parameterCount == 0)
            {
                fetchMethod = opType.GetMethod(nameof(ObjectPortal<object>.Fetch), new Type[0]);
            }
            else
            {

                // parameterCount = 1

                fetchMethod = opType.GetMethods().Where(x => x.Name == nameof(ObjectPortal<object>.Fetch) && x.IsGenericMethod).First();

                // We need to match the delegateType signature
                // So Fetch<C> needs to be Fetch<Criteria>
                // Again we look to our invoke for this
                var paramType = invoke.GetParameters().First().ParameterType;
                fetchMethod = fetchMethod.MakeGenericMethod(paramType);

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
