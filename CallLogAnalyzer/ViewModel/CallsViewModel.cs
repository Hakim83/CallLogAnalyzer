using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telecom;
using Android.Views;
using Android.Widget;
using CallLogAnalyzer.Helpers;
using CallLogAnalyzer.Model;

namespace CallLogAnalyzer.ViewModel
{
    public class CallsViewModel
    {
        public List<CallInfo> Calls { get; set; }
        public int CallsDuration => Calls.Sum(c => (int) c.Duration);
        public int CallsCount => Calls.Count;

        public CallsViewModel(List<CallInfo> calls, string sortBy)
        {
            if (sortBy == nameof(DateTime))
            {
                Calls= calls.OrderByDescending(c => c.DateTime).ToList();
            }
            else if (sortBy == nameof(CallsCount))
            {
                //Calls = calls.OrderByDescending(c => c.CallsCount).ToList();
                //TODO: does count mean any thing here? just sort by date!!
                Calls= calls.OrderByDescending(c => c.DateTime).ToList();
            }

            else if (sortBy == nameof(CallsDuration))
            {
                Calls= calls.OrderByDescending(c => c.Duration).ToList();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("All Calls " + CallsCount + " calls " + CallsDuration.ToDurationString());
            foreach (var call in Calls)
            {
                sb.AppendLine();
                sb.AppendLine(call.Title);
                sb.AppendLine(call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                              + ((int)call.Duration).ToDurationString());
            }

            return sb.ToString();
        }
    }
}