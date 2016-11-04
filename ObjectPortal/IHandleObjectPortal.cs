using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ObjectPortal
{

    public interface IHandleObjectPortalFetch
    {

        void Fetch();

    }

    public interface IHandleObjectPortalFetch<T>
    {

        void Fetch(T criteria);

    }

}
