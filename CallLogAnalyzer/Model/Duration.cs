
using System;
using System.Text;

namespace CallLogAnalyzer.Model
{
    public class Duration
    {
        private TimeSpan timeSpan;
        public int Sec { get; set; }
        public Duration(int seconds)
        {
            Sec = seconds;
            timeSpan = TimeSpan.FromSeconds(seconds);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (timeSpan.TotalHours>=1)
            {
                sb.Append((int) timeSpan.TotalHours + "h ");
            }

            if (timeSpan.Minutes>0)
            {
                sb.Append(timeSpan.Minutes + "m ");
            }

            sb.Append(timeSpan.Seconds + "s");

            return sb.ToString();
        }
    }
}