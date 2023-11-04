﻿using DocumentManagement.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Core.Services.Implementations
{
    public class SystemClock : ISystemClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
