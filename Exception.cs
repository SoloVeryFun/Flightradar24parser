using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewException
{
    public class FlighNumberIsMissingException: Exception
    {
        public FlighNumberIsMissingException() : base("Flight number is missing") { }
    }
}
