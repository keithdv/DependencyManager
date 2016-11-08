using Autofac;
using Csla;
using Example.Dal;
using ObjectPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Csla.Core;
using Csla.Rules;

namespace Example.Lib
{

    public delegate IBusinessItem FetchBusinessItem(BusinessItemDto criteria);
    public delegate void UpdateBusinessItem(IBusinessItem bo, Guid criteria);

    [Serializable]
    internal class BusinessItem : DPBusinessBase<BusinessItem>, IBusinessItem, IHandleObjectPortalUpdate<Guid>
    {

        // Thought: "new DependencyPropertyInfo" should be a base class call like RegisterProperty
        public static readonly DependencyPropertyInfo<IBusinessItemDal> DalProperty = new DependencyPropertyInfo<IBusinessItemDal>(nameof(Dal));
        public IBusinessItemDal Dal
        {
            get { return GetDependencyProperty(DalProperty); }
        }

        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
        public string Name
        {
            get { return GetProperty(NameProperty); }
            set { SetProperty(NameProperty, value); }
        }

        public static readonly PropertyInfo<Guid> CriteriaProperty = RegisterProperty<Guid>(c => c.Criteria);
        public Guid Criteria
        {
            get { return GetProperty(CriteriaProperty); }
            set { SetProperty(CriteriaProperty, value); }
        }

        public static readonly PropertyInfo<Guid> UniqueIDProperty = RegisterProperty<Guid>(c => c.FetchID);
        public Guid FetchID
        {
            get { return GetProperty(UniqueIDProperty); }
            set { SetProperty(UniqueIDProperty, value); }
        }


        public static readonly PropertyInfo<Guid> UpdatedIDProperty = RegisterProperty<Guid>(c => c.UpdatedID);
        public Guid UpdatedID
        {
            get { return GetProperty(UpdatedIDProperty); }
            set { SetProperty(UpdatedIDProperty, value); }
        }

        public static readonly PropertyInfo<Guid> ScopeIDProperty = RegisterProperty<Guid>(c => c.ScopeID);
        public Guid ScopeID
        {
            get { return GetProperty(ScopeIDProperty); }
            set { SetProperty(ScopeIDProperty, value); }
        }

        public void Fetch(BusinessItemDto dto)
        {
            MarkAsChild();
            this.FetchID = dto.FetchUniqueID;
            this.Criteria = dto.Criteria;
        }

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();
            BusinessRules.AddRule(new DependencyBusinessRUle(NameProperty, DalProperty));
        }

        public void Insert(Guid criteria)
        {

            var dto = new BusinessItemDto();
            Dal.Update(dto);

            using (BypassPropertyChecks)
            {
                this.UpdatedID = dto.UpdateUniqueID;
            }

        }

        public void Update(Guid criteria)
        {
            var dto = new BusinessItemDto();
            Dal.Update(dto);

            using (BypassPropertyChecks)
            {
                this.UpdatedID = dto.UpdateUniqueID;
            }
        }

        internal class DependencyBusinessRUle : DependencyBusinessRule
        {

            public DependencyBusinessRUle(IPropertyInfo nameProperty, IDependencyPropertyInfo<IBusinessItemDal> dalDep) : base(nameProperty)
            {
                InputProperties.Add(nameProperty);
                DependencyProperties.Add(dalDep);
            }

            protected override void Execute(RuleContext context)
            {
                base.Execute(context);

                var dal = DependencyPropertyValues[DependencyProperties[0]];

                if(dal == null)
                {
                    context.AddErrorResult("Did not recieve dependency!");
                }

            }
        }

    }
}
