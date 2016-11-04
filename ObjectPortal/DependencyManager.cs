using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace ObjectPortal
{
    public class DependencyManager
    {

        public DependencyManager(ILifetimeScope scope)
        {
            this.scope = scope;

        }

        [NonSerialized]
        Dictionary<IDependencyPropertyInfo, object> dependencies = new Dictionary<IDependencyPropertyInfo, object>();

        // Option: Store serializable dependencies in a serialized collection

        [NonSerialized]
        ILifetimeScope scope;

        // Thought: There's nothing that formally ties this to the business object and the dependencies defined within the business object

        public DP GetPropertyValue<DP>(IDependencyPropertyInfo<DP> propertyInfo)
        {

            if (!dependencies.ContainsKey(propertyInfo))
            {
                dependencies.Add(propertyInfo, scope.Resolve(typeof(DP)));
            }

            return (DP)dependencies[propertyInfo];

        }

        public object GetPropertyValue(IDependencyPropertyInfo propertyInfo)
        {
            if (!dependencies.ContainsKey(propertyInfo))
            {
                dependencies.Add(propertyInfo, scope.Resolve(propertyInfo.Type));
            }

            return dependencies[propertyInfo];
        }

    }
}
