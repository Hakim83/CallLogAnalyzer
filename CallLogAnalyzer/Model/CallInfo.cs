using System;

namespace CallLogAnalyzer.Model
{
    public class CallInfo
    {
        public string Number { get; set; }
        public DateTime DateTime { get; set; }
        public long Duration { get; set; }
        public CallType Type { get; set; }
        public string CallerName { get; set; }
        public string Title => CallerName == null ? Number : CallerName+" ("+Number+")";
    }

    public enum CallType
    {
        Incoming = 1,
        Outgoing = 2,
        Missed = 3,
        VoiceMail = 4,
        Rejected = 5,
        Blocked = 6
    }
}