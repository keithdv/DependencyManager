using Csla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using ObjectPortal;
using Example.Dal;

namespace Example.Lib
{
    [Serializable]
    public class BusinessItemList : DtoBusinessListBase<BusinessItemList, IBusinessItem>, IBusinessItemList
    {
        
        public static DependencyPropertyInfo<IObjectPortal<IBusinessItem>> ItemPortalProperty = new DependencyPropertyInfo<IObjectPortal<IBusinessItem>>(nameof(ItemPortal));

        public IObjectPortal<IBusinessItem> ItemPortal
        {
            get { return GetDependencyProperty(ItemPortalProperty); }
        }

        public static readonly DependencyPropertyInfo<IBusinessItemDal> DalProperty = new DependencyPropertyInfo<IBusinessItemDal>(nameof(Dal));
        public IBusinessItemDal Dal
        {
            get { return GetDependencyProperty(DalProperty); }
        }

        public void Fetch()
        {
            var dtos = Dal.Fetch();

            foreach(var d in dtos)
            {
                Add(ItemPortal.Fetch(d));
            }

        }

        public void Fetch(Guid criteria)
        {

            var dtos = Dal.Fetch(criteria);

            foreach (var d in dtos)
            {
                Add(ItemPortal.Fetch(d));
            }

        }

    }
}
