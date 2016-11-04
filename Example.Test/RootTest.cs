using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;
using ObjectPortal;
using Example.Lib;
using Example.DalConcrete;
using Example.Dal;

namespace Example.Test
{
    [TestClass]
    public class RootTest
    {

        static IContainer container;
        ILifetimeScope scope;


        [TestInitialize]
        public void TestInitialize()
        {

            if(container == null)
            {

                ContainerBuilder builder = new ContainerBuilder();

                builder.RegisterGeneric(typeof(ObjectPortal<>)).As(typeof(IObjectPortal<>));

                // In actual implementation this would be in modules
                // But I want to keep it all in one place for discussion
                builder.RegisterType<Root>().As<IRoot>();
                builder.RegisterType<BusinessItem>().As<IBusinessItem>();
                builder.RegisterType<BusinessItemList>().As<IBusinessItemList>();

                builder.RegisterType<RootDal>().AsImplementedInterfaces();
                builder.RegisterType<BusinessItemDal>().AsImplementedInterfaces();


                builder.RegisterType<DependencyManager>();

                container = builder.Build();

            }

            scope = container.BeginLifetimeScope();

        }

        [TestMethod]
        public void Root_Fetch()
        {

            var portal = scope.Resolve<IObjectPortal<IRoot>>();

            var result = portal.Fetch();

        }

        [TestMethod]
        public void Root_Fetch_Criteria()
        {
            var portal = scope.Resolve<IObjectPortal<IRoot>>();
            var criteria = Guid.NewGuid();

            var result = portal.Fetch(criteria);

            Assert.AreEqual(criteria, result.BusinessItemList[0].Criteria);

        }

        [TestMethod]
        public void Root_BusinessRule()
        {

            var portal = scope.Resolve<IObjectPortal<IRoot>>();

            var result = portal.Fetch();

            result.BusinessItemList[0].Name = Guid.NewGuid().ToString();

            Assert.IsTrue(result.IsValid);

        }

        //[TestMethod]
        //public void Root_Fetch_Save()
        //{
        //    var portal = scope.Resolve<IObjectPortal<IRoot>>();
        //    var criteria = Guid.NewGuid();

        //    var result = portal.Fetch(criteria);

        //    result.SaveDto();

        //    Assert.AreEqual(2, result.BusinessItemList.Count);

        //    foreach(var r in result.BusinessItemList)
        //    {
        //        Assert.IsNotNull(r.UpdatedID);
        //    }
        //}
    }
}
