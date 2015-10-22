using System;
using WebEmailExtractor.Logging;

namespace WebEmailExtractor.Tests.WebEmailExtraction.Fakes
{
    public class VerboseLoggerFake : VerboseLogger
    {

        public int LogCnt { get; set; }
        public int LogVerboseCnt { get; set; }


        public VerboseLoggerFake(bool verboseLogEnabled, Action<string> onLog) 
            : base(verboseLogEnabled, onLog)
        {
        }

        public override void Log(string message)
        {
            LogCnt++;
        }

        public override void LogVerbose(string message)
        {
            LogVerboseCnt++;
        }

    }
}
