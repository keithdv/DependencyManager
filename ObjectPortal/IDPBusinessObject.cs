﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPortal
{
    public interface IDPBusinessObject
    {

        DependencyManager DependencyManager { get; set; }
    }
}