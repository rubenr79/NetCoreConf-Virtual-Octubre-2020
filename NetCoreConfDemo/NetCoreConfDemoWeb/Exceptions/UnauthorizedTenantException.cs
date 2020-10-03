using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreConfDemoWeb.Exceptions
{
    public class UnauthorizedTenantException : UnauthorizedAccessException
    {
        public UnauthorizedTenantException() : base() { }
        public UnauthorizedTenantException(string message) : base(message) { }
    }
}
