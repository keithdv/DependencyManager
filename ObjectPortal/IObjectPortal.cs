using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ObjectPortal
{
    public interface IObjectPortal<T>
        where T : Csla.Core.ITrackStatus
    {

        T Fetch();
        T Fetch<C>(C criteria);
        void Update(T bo);
        void Update<C>(T bo, C criteria);
    }
}
