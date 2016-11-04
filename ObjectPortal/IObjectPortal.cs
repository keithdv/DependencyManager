using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ObjectPortal
{
    public interface IObjectPortal<T>
    {

        T Fetch();
        T Fetch<C>(C criteria);
    }
}
