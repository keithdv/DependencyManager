using ObjectPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPortal
{
    [Serializable]
    public class DPBusinessBase<T> : Csla.BusinessBase<T>, IDPBusinessObject
        where T : DPBusinessBase<T>
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
