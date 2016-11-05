using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ObjectPortal;

namespace Example.Lib
{
    public class LibModule : Autofac.Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<Root>().As<IRoot>();
            builder.RegisterType<BusinessItem>().As<IBusinessItem>();
            builder.RegisterType<BusinessItemList>().As<IBusinessItemList>();

            // Need to find a way to make this generic
            // Delegates and generics do not play nice together!!

            builder.Register<FetchRoot>((c) =>
            {
                var portal = c.Resolve<IObjectPortal<IRoot>>();
                return () => portal.Fetch(); // C# lets you implicitly convert a lamda to a delegate...can't do this anywhere else!
            });

            builder.Register<FetchRootGuid>((c) =>
            {
                var portal = c.Resolve<IObjectPortal<IRoot>>();
                return (crit) => portal.Fetch(crit);
            });

            builder.Register<FetchBusinessItem>((c) =>
            {
                var portal = c.Resolve<IObjectPortal<IBusinessItem>>();
                return (d) => { return portal.Fetch(d); };
            });

            builder.Register<FetchBusinessItemList>((c) =>
            {
                var portal = c.Resolve<IObjectPortal<IBusinessItemList>>();
                return () => { return portal.Fetch(); }; ;
            });

            builder.Register<FetchBusinessItemListGuid>((c) =>
            {
                var portal = c.Resolve<IObjectPortal<IBusinessItemList>>();
                return (crit) => { return portal.Fetch(crit); }; ;
            });

        }

    }
}
