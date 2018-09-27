using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Log
{
    public class LogHandler
    {
        public delegate void LogHandlerDelegate(ITradingLog result);
    }
}
