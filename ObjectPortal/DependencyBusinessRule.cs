using Csla.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Rules;

namespace ObjectPortal
{
    public class DependencyBusinessRule : Csla.Rules.BusinessRule
    {

        public DependencyBusinessRule() : base() { }

        public DependencyBusinessRule(IPropertyInfo primaryProperty) : base(primaryProperty)
        {

        }

        protected List<IDependencyPropertyInfo> DependencyProperties = new List<IDependencyPropertyInfo>();

        // This would be added to the context
        protected Dictionary<IDependencyPropertyInfo, object> DependencyPropertyValues = new Dictionary<IDependencyPropertyInfo, object>();

        protected override void Execute(RuleContext context)
        {
            base.Execute(context);

            var dp = context.Target as IDPBusinessObject;

            foreach (var dpi in DependencyProperties)
            {
                DependencyPropertyValues.Add(dpi, dp.DependencyManager.GetPropertyValue(dpi));
            }

        }

    }
}
