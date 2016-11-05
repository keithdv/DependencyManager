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

    public delegate IBusinessItemList FetchBusinessItemList();
    public delegate IBusinessItemList FetchBusinessItemListGuid(Guid criteria);

    [Serializable]
    internal class BusinessItemList : DtoBusinessListBase<BusinessItemList, IBusinessItem>, IBusinessItemList
    {
        
        public static DependencyPropertyInfo<FetchBusinessItem> FetchBusinessItemProperty = new DependencyPropertyInfo<FetchBusinessItem>(nameof(FetchBusinessItemProperty));

        public FetchBusinessItem FetchBusinessItem
        {
            get { return GetDependencyProperty(FetchBusinessItemProperty); }
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
                Add(FetchBusinessItem(d));
            }

        }

        public void Fetch(Guid criteria)
        {

            var dtos = Dal.Fetch(criteria);

            foreach (var d in dtos)
            {
                Add(FetchBusinessItem(d));
            }

        }

    }
}
