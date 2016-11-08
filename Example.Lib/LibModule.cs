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

            //builder.Register<FetchRoot>((c) =>
            //{
            //    var portal = c.Resolve<IObjectPortal<IRoot>>();
            //    return () => portal.Fetch(); // C# lets you implicitly convert a lamda to a delegate...can't do this anywhere else!
            //});

            // Update - Best I came up with
            builder.ObjectPortalFetch(typeof(FetchRoot));
            builder.ObjectPortalFetch(typeof(FetchRootGuid));
            builder.ObjectPortalFetch(typeof(FetchBusinessItem));
            builder.ObjectPortalFetch(typeof(FetchBusinessItemList));
            builder.ObjectPortalFetch(typeof(FetchBusinessItemListGuid));

            builder.ObjectPortalUpdate(typeof(IRoot));
            builder.ObjectPortalUpdate(typeof(IBusinessItemList));
            builder.ObjectPortalUpdate(typeof(UpdateBusinessItem));

        }

    }
}
