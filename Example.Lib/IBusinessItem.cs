using Example.Dal;
using ObjectPortal;
using System;

namespace Example.Lib
{
    public interface IBusinessItem : Csla.IBusinessBase, IHandleObjectPortalFetch<BusinessItemDto>
    {
        string Name { get; set; }
        Guid Criteria { get; }
        Guid ScopeID { get;  }
        Guid FetchID { get;  }
        Guid UpdatedID { get; }

    }
}