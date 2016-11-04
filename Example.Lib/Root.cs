using Csla;
using System;
using Autofac;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectPortal;
using Example.Dal;

namespace Example.Lib
{
    [Serializable]
    public class Root : DPBusinessBase<Root>, IRoot, IHandleObjectPortalFetch, IHandleObjectPortalFetch<Guid>
    {

        public static DependencyPropertyInfo<IObjectPortal<IBusinessItemList>> BusinessItemListPortalProperty = new DependencyPropertyInfo<IObjectPortal<IBusinessItemList>>(nameof(BusinessItemListPortal));

        public IObjectPortal<IBusinessItemList> BusinessItemListPortal
        {
            get { return GetDependencyProperty(BusinessItemListPortalProperty); }
        }
        
        public static readonly PropertyInfo<IBusinessItemList> BusinessItemListProperty = RegisterProperty<IBusinessItemList>(c => c.BusinessItemList);
        public IBusinessItemList BusinessItemList
        {
            get { return GetProperty(BusinessItemListProperty); }
            set { SetProperty(BusinessItemListProperty, value); }
        }

        void IHandleObjectPortalFetch.Fetch()
        {
            BusinessItemList = BusinessItemListPortal.Fetch();
        }

        public void Fetch(Guid criteria)
        {
            BusinessItemList = BusinessItemListPortal.Fetch(criteria);
        }

    }
}
