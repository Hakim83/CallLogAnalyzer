using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CallLogAnalyzer.Helpers;
using CallLogAnalyzer.ViewModel;

namespace CallLogAnalyzer
{
    class ListViewItemsBuilder
    {
        public List<Item> GetItems(object data)
        {
            List<Item> items = new List<Item>();
            if (data.GetType()== typeof(CallsViewModel))
            {
                var callsVm = data as CallsViewModel;

                items.Add(new Item(0)
                {
                    Text = "All Calls " + callsVm.CallsCount + " calls " + callsVm.CallsDuration.ToDurationString()
                });
                foreach (var call in callsVm.Calls)
                {
                    items.Add(new Item(1)
                    {
                        Text = call.Title,
                        SecondText = call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                                  + ((int)call.Duration).ToDurationString()
                    });
                }
            }
            else if (data.GetType()== typeof(CallersViewModel))
            {
                

                //sb.AppendLine("All Calls " + CallsCount + " calls " + CallsDuration.ToDurationString());
                //foreach (var caller in Callers)
                //{
                //    sb.AppendLine("+" + caller.Title + " " + caller.CallsCount + " calls "
                //                  + caller.CallsDuration.ToDurationString());
                //    foreach (var number in caller.Numbers)
                //    {
                //        sb.AppendLine("++" + number.Number + " " + number.CallsCount + " calls "
                //                      + number.CallsDuration.ToDurationString());
                //        foreach (var callType in number.CallTypes)
                //        {
                //            sb.AppendLine("+++" + callType.CallType + " " + callType.CallsCount + " calls "
                //                          + callType.CallsDuration.ToDurationString());
                //            foreach (var callTime in callType.CallTimes)
                //            {
                //                sb.AppendLine("++++" + callTime.DateTime.ToString("dd/MM HH:mm ")
                //                                     + callTime.CallDuration.ToDurationString());
                //            }
                //        }
                //    }
                //}
            }
            else if(data.GetType()== typeof(DetailedCallsViewModel))
            {
                
            }
            else
            {
                throw new ArgumentException("Unknown data type!");
            }

            return items;
        }
    }
}