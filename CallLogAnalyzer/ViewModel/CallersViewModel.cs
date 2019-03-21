using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallLogAnalyzer.Helpers;
using CallLogAnalyzer.Model;

namespace CallLogAnalyzer.ViewModel
{
    public class CallersViewModel
    {
        public List<CallerViewModel> Callers { get; set; }
        public int CallsCount => Callers.Count == 0 ? 0 : Callers.Sum(c => c.CallsCount);
        public int CallsDuration => Callers.Count == 0 ? 0 : Callers.Sum(c => c.CallsDuration);

        public CallersViewModel(List<CallInfo> calls, string sortBy = nameof(DateTime))
        {
            var callerList = new List<CallerViewModel>();
            foreach (var call in calls)
            {
                var title = call.CallerName ?? call.Number;
                var caller = callerList.FirstOrDefault(c => c.Title == title);
                if (caller == null)
                {
                    caller = new CallerViewModel { Title = title, Name = call.CallerName };
                    callerList.Add(caller);
                }

                var number = caller.Numbers.FirstOrDefault(c => c.Number == call.Number);
                if (number == null)
                {
                    number = new CallerViewModel.NumberViewModel { Number = call.Number };
                    caller.Numbers.Add(number);
                }

                var type = number.CallTypes.FirstOrDefault(t => t.CallType == call.Type);
                if (type == null)
                {
                    type = new CallerViewModel.NumberViewModel.CallTypeViewModel { CallType = call.Type };
                    number.CallTypes.Add(type);
                }

                type.CallTimes.Add(new CallerViewModel.NumberViewModel.CallTypeViewModel.CallTimeViewModel
                {
                    DateTime = call.DateTime,
                    CallDuration = (int)call.Duration
                });
            }


            if (sortBy == nameof(DateTime))
            {
                callerList = callerList.OrderByDescending(c => c.DateTime).ToList();
            }
            else if (sortBy == nameof(CallsCount))
            {
                callerList = callerList.OrderByDescending(c => c.CallsCount).ToList();
            }

            else if (sortBy == nameof(CallsDuration))
            {
                callerList = callerList.OrderByDescending(c => c.CallsDuration).ToList();
            }

            Callers = callerList;
        }

        public class CallerViewModel
        {
            public string Title { get; set; }
            public string Name { get; set; }
            public int CallsCount
            {
                get { return Numbers.Count == 0 ? 0 : Numbers.Sum(n => n.CallsCount); }
            }
            public int CallsDuration
            {
                get { return Numbers.Count == 0 ? 0 : Numbers.Sum(n => n.CallsDuration); }
            }

            public DateTime DateTime => Numbers.OrderByDescending(n => n.DateTime).Select(n => n.DateTime).First();
            public List<NumberViewModel> Numbers { get; set; } = new List<NumberViewModel>();

            public class NumberViewModel
            {
                public string Number { get; set; }

                public int CallsCount
                {
                    get { return CallTypes.Count == 0 ? 0 : CallTypes.Sum(ct => ct.CallsCount); }
                }
                public int CallsDuration
                {
                    get { return CallTypes.Count == 0 ? 0 : CallTypes.Sum(ct => ct.CallsDuration); }
                }
                public DateTime DateTime
                {
                    get => CallTypes.OrderByDescending(ct => ct.DateTime).Select(ct => ct.DateTime).First();
                }
                public List<CallTypeViewModel> CallTypes { get; set; } = new List<CallTypeViewModel>();

                public class CallTypeViewModel
                {
                    public CallType CallType { get; set; }
                    public DateTime DateTime => CallTimes.Count == 0 ? new DateTime() : CallTimes.OrderByDescending(ct => ct.DateTime).Select(ct => ct.DateTime).First();
                    public int CallsCount => CallTimes.Count;
                    public int CallsDuration => CallTimes.Count == 0 ? 0 : CallTimes.Sum(ct => ct.CallDuration);
                    public List<CallTimeViewModel> CallTimes { get; set; } = new List<CallTimeViewModel>();

                    public class CallTimeViewModel
                    {
                        public int CallDuration { get; set; }
                        public DateTime DateTime { get; set; }
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("All Calls " + CallsCount + " calls " + CallsDuration.ToDurationString());
            foreach (var caller in Callers)
            {
                sb.AppendLine("+" + caller.Title + " " + caller.CallsCount + " calls "
                              + caller.CallsDuration.ToDurationString());
                foreach (var number in caller.Numbers)
                {
                    sb.AppendLine("++"+number.Number + " " + number.CallsCount + " calls "
                                  + number.CallsDuration.ToDurationString());
                    foreach (var callType in number.CallTypes)
                    {
                        sb.AppendLine("+++" + callType.CallType + " " + callType.CallsCount + " calls "
                                      + callType.CallsDuration.ToDurationString());
                        foreach (var callTime in callType.CallTimes)
                        {
                            sb.AppendLine("++++" + callTime.DateTime.ToString("dd/MM HH:mm ")
                                                 + callTime.CallDuration.ToDurationString());
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}