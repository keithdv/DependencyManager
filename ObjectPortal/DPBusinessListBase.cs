using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPortal
{
    [Serializable]
    public class DtoBusinessListBase<T, C> : Csla.BusinessListBase<T, C>, IDPBusinessObject
        where C : Csla.Core.IEditableBusinessObject
        where T:DtoBusinessListBase<T, C>
    {

        [NonSerialized]
        private DependencyManager _dependencyManager;

        public DependencyManager DependencyManager
        {
            get { return _dependencyManager; }
        }

        DependencyManager IDPBusinessObject.DependencyManager
        {
            get { return _dependencyManager; }
            set { _dependencyManager = value; }
        }

        protected DP GetDependencyProperty<DP>(IDependencyPropertyInfo<DP> propertyInfo)
        {
            return DependencyManager.GetPropertyValue(propertyInfo);

        }

    }
}
