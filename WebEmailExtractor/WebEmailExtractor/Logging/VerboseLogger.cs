using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEmailExtractor.Logging
{
    public class VerboseLogger
    {

        protected readonly bool VerboseLogEnabled;
        protected readonly Action<string> OnLog;


        public VerboseLogger(bool verboseLogEnabled, Action<string> onLog)
        {
            VerboseLogEnabled = verboseLogEnabled;
            OnLog = onLog;
        }

        public virtual void Log(string message)
        {
            OnLog(message);
        }

        public virtual void LogVerbose(string message)
        {
            if (VerboseLogEnabled)
            {
                OnLog($"[debug]:{message}");
            }
        }

    }
}
