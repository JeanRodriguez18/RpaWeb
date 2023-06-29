using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpaWeb
{
    public class DriverNotFoundException : Exception
    {
        public DriverNotFoundException() { }

        public DriverNotFoundException(string message) : base(message) { }

        public DriverNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
