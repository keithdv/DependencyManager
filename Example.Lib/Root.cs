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

    public delegate IRoot FetchRoot();
    public delegate IRoot FetchRootGuid(Guid criteria);

    [Serializable]
    internal class Root : DPBusinessBase<Root>, IRoot, IHandleObjectPortalFetch, IHandleObjectPortalFetch<Guid>, IHandleObjectPortalUpdate
    {

        public static DependencyPropertyInfo<FetchBusinessItemList> BusinessItemListPortalProperty = new DependencyPropertyInfo<FetchBusinessItemList>(nameof(CreateBusinessItemList));

        public FetchBusinessItemList CreateBusinessItemList
        {
            get { return GetDependencyProperty(BusinessItemListPortalProperty); }
        }

        public static DependencyPropertyInfo<FetchBusinessItemListGuid> CreateBusinessItemListGuidProperty = new DependencyPropertyInfo<FetchBusinessItemListGuid>(nameof(CreateBusinessItemListGuid));

        public FetchBusinessItemListGuid CreateBusinessItemListGuid
        {
            get { return GetDependencyProperty(CreateBusinessItemListGuidProperty); }
        }

        public static DependencyPropertyInfo<ObjectPortalUpdate<IBusinessItemList>> UpdateBusinessItemListProperty = new DependencyPropertyInfo<ObjectPortalUpdate<IBusinessItemList>>(nameof(UpdateBusinessItemList));

        public ObjectPortalUpdate<IBusinessItemList> UpdateBusinessItemList
        {
            get { return GetDependencyProperty(UpdateBusinessItemListProperty); }
        }

        public static readonly PropertyInfo<IBusinessItemList> BusinessItemListProperty = RegisterProperty<IBusinessItemList>(c => c.BusinessItemList);
        public IBusinessItemList BusinessItemList
        {
            get { return GetProperty(BusinessItemListProperty); }
            set { SetProperty(BusinessItemListProperty, value); }
        }

        void IHandleObjectPortalFetch.Fetch()
        {
            BusinessItemList = CreateBusinessItemList();
        }

        public void Fetch(Guid criteria)
        {
            BusinessItemList = CreateBusinessItemListGuid(criteria);
        }

        public void Insert()
        {
            UpdateBusinessItemList(BusinessItemList);
        }

        public void Update()
        {
            UpdateBusinessItemList(BusinessItemList);
        }
    }
}
