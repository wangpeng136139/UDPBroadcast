using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugServer
{
    internal class EnvironmentHelper
    {
        public static int GetAIServerPort()
        {
            string str = Environment.GetEnvironmentVariable("NETCORE_AISERVER_PORT");
            if (string.IsNullOrEmpty(str))
            {
                return 8888;
            }

            Int32.TryParse(str, out int result);
            return result;
        }

    }
}
