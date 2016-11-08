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
                fetchMethod = opType.GetMethod(nameof(ObjectPortal<Csla.IBusinessBase>.Fetch), new Type[0]);
            }
            else
            {

                // parameterCount = 1

                fetchMethod = opType.GetMethods().Where(x => x.Name == nameof(ObjectPortal<Csla.IBusinessBase>.Fetch) && x.IsGenericMethod).First();

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

        public static void ObjectPortalUpdate(this ContainerBuilder builder, Type boType)
        {

            // Some serious WTF code!

            if (boType == null)
            {
                throw new ArgumentNullException(nameof(boType));
            }

            if (typeof(Delegate).IsAssignableFrom(boType))
            {
                // This is a delegate type
                ObjectPortalUpdate_Delegate(builder, boType);
                return;
            }

            // Business object type
            // Update with no parameters

            // Need to resolve an IObjectPortal<BusinessObjectType>
            var opType = typeof(IObjectPortal<>).MakeGenericType(boType);
            var delegateType = typeof(ObjectPortalUpdate<>).MakeGenericType(boType);

            MethodInfo updateMethod = null;

            updateMethod = opType.GetMethod(nameof(ObjectPortal<Csla.IBusinessBase>.Update), new Type[1] { boType });

            builder.Register((c) =>
            {
                var portal = c.Resolve(opType);

                return Convert.ChangeType(Delegate.CreateDelegate(delegateType, portal, updateMethod), delegateType);

            }).As(delegateType);

        }

        private static void ObjectPortalUpdate_Delegate(this ContainerBuilder builder, Type delegateType)
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



            if (invoke == null)
            {
                throw new Exception($"Unable to load invoke method on ${delegateType.Name}");
            }

            var parameterCount = invoke.GetParameters().Count();

            if (parameterCount == 0 || parameterCount > 2)
            {
                throw new Exception($"Delegate ${delegateType.Name} cannot have 1 or 2 method parameter.");
            }

            var boType = invoke.GetParameters()[0].ParameterType;

            // Need to resolve an IObjectPortal<BusinessObjectType>
            var opType = typeof(IObjectPortal<>).MakeGenericType(boType);

            MethodInfo updateMethod = null;

            // ObjectPortal concrete has two fetch methods
            // One that takes parameters and one that doesn't
            // Chose the right one based on the parameters our delegate
            if (parameterCount == 1)
            {
                updateMethod = opType.GetMethod(nameof(ObjectPortal<Csla.IBusinessBase>.Update), new Type[0]);
            }
            else
            {

                // parameterCount = 1
                updateMethod = opType.GetMethods().Where(x => x.Name == nameof(ObjectPortal<Csla.IBusinessBase>.Update) && x.IsGenericMethod).First();

                // We need to match the delegateType signature
                // So Fetch<C> needs to be Fetch<Criteria>
                // Again we look to our invoke for this
                var criteriaType = invoke.GetParameters()[1].ParameterType;
                updateMethod = updateMethod.MakeGenericMethod(new Type[1] { criteriaType });

            }

            builder.Register((c) =>
            {
                var portal = c.Resolve(opType);

                return Convert.ChangeType(Delegate.CreateDelegate(delegateType, portal, updateMethod), delegateType);

            }).As(delegateType);

        }
    }


    public delegate void ObjectPortalUpdate<T>(T Bo) where T : Csla.Core.ITrackStatus;
    public delegate void ObjectPortalUpdate<T, C>(T Bo, C criteria) where T : Csla.Core.ITrackStatus;

    /// <summary>
    /// Abstract BO object creating, fetching and updating each other
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPortal<T> : IObjectPortal<T>
        where T : Csla.Core.ITrackStatus
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

        public void Update(T bo)
        {
            var update = bo as IHandleObjectPortalUpdate;

            if (update == null)
            {
                throw new ObjectPortalOperationNotSupportedException($"Update not implemented on {typeof(T).Name}");
            }

            if (bo.IsDirty)
            {
                if (bo.IsNew)
                {
                    update.Insert();
                }
                else
                {
                    update.Update();
                }
            }
        }

        public void Update<C>(T bo, C criteria)
        {
            var update = bo as IHandleObjectPortalUpdate<C>;

            if (update == null)
            {
                throw new ObjectPortalOperationNotSupportedException($"Update not implemented on {typeof(T).Name} with criteria {typeof(C).Name}");
            }

            if (bo.IsDirty)
            {
                if (bo.IsNew)
                {
                    update.Insert(criteria);
                }
                else
                {
                    update.Update(criteria);
                }
            }
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
