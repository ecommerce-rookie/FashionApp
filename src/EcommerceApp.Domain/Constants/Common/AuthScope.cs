﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Constants.Common
{
    public static class AuthScope
    {
        public const string Read = nameof(Read);
        public const string Write = nameof(Write);
        public const string All = nameof(All);
    }
}
