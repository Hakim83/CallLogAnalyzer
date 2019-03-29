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
using MultilevelView;

namespace CallLogAnalyzer
{
    class ListViewItemsBuilder
    {
        public List<RecyclerViewItem> GetItems(object data)
        {
            InitStrings();
            List<RecyclerViewItem> items = new List<RecyclerViewItem>();
            if (data.GetType() == typeof(CallsViewModel))
            {
                var callsVm = data as CallsViewModel;

                items.Add(new Item(0)
                {
                    Text = $"{str_all_calls} " + callsVm.CallsCount + $" {str_calls} " + callsVm.CallsDuration.ToDurationString()
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
            else if (data.GetType() == typeof(CallersViewModel))
            {
                var callersVm = data as CallersViewModel;

                items.Add(new Item(0)
                {
                    Text = $"{str_all_calls} " + callersVm.CallsCount
                                        + $" {str_calls} " + callersVm.CallsDuration.ToDurationString(),
                    Children = callersVm.Callers.Select(caller => new Item(1)
                    {
                        Text = caller.Title + " " + caller.CallsCount + $" {str_calls} "
                               + caller.CallsDuration.ToDurationString(),
                        Children = caller.Numbers.Select(number => new Item(2)
                        {
                            Text = number.Number + " " + number.CallsCount + $" {str_calls} "
                                   + number.CallsDuration.ToDurationString(),
                            Children = number.CallTypes.Select(callType => new Item(3)
                            {
                                Text = callType.CallType + " " + callType.CallsCount + $" {str_calls} "
                                       + callType.CallsDuration.ToDurationString(),
                                Children = callType.CallTimes.Select(callTime => new Item(4)
                                {
                                    Text = callTime.DateTime.ToString("dd/MM HH:mm ")
                                           + callTime.CallDuration.ToDurationString()
                                }).ToList<RecyclerViewItem>()
                            }).ToList<RecyclerViewItem>()
                        }).ToList<RecyclerViewItem>()
                    }).ToList<RecyclerViewItem>()
                });

                //sb.AppendLine($"{str_all_calls} " + CallsCount + $" {str_calls} " + CallsDuration.ToDurationString());
                //foreach (var caller in Callers)
                //{
                //    sb.AppendLine("+" + caller.Title + " " + caller.CallsCount + $" {str_calls} "
                //                  + caller.CallsDuration.ToDurationString());
                //    foreach (var number in caller.Numbers)
                //    {
                //        sb.AppendLine("++" + number.Number + " " + number.CallsCount + $" {str_calls} "
                //                      + number.CallsDuration.ToDurationString());
                //        foreach (var callType in number.CallTypes)
                //        {
                //            sb.AppendLine("+++" + callType.CallType + " " + callType.CallsCount + $" {str_calls} "
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
            else if (data.GetType() == typeof(DetailedCallsViewModel))
            {
                var callsVm = data as DetailedCallsViewModel;

                items.AddRange(callsVm.Countries.Select(country => new Item(0)
                {
                    Text = country.CountryName + " " + country.CallsCount
                          + $" {str_calls} " + country.CallsDuration.ToDurationString(),
                    Children = new List<RecyclerViewItem>
                    {
                        new Item(1)
                        {
                            Text=$"{str_mobile_calls} " + country.MobileType.CallsCount + $" {str_calls} "
                                                 + country.MobileType.CallsDuration.ToDurationString(),
                            Children = country.MobileType.Carriers.Select(carrier=> new Item(2)
                            {
                                Text = carrier.CarrierName + " " + carrier.CallsCount
                                      + $" {str_calls} " + carrier.CallsDuration.ToDurationString(),
                                Children = carrier.Calls.Select(call=> new Item(3)
                                {
                                    Text = call.Title,
                                    SecondText = call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                                                 + ((int)call.Duration).ToDurationString()
                                }).ToList<RecyclerViewItem>()
                            }).ToList<RecyclerViewItem>()
                        },
                        new Item(1)
                        {
                            Text=$"{str_fixed_line_calls} " + country.FixedLineType.CallsCount + $" {str_calls} "
                                 + country.FixedLineType.CallsDuration.ToDurationString(),
                            Children = country.FixedLineType.Areas.Select(area=> new Item(2)
                            {
                                Text = area.AreaName + " " + area.CallsCount
                                       + $" {str_calls} " + area.CallsDuration.ToDurationString(),
                                Children = area.Calls.Select(call=> new Item(3)
                                {
                                    Text = call.Title,
                                    SecondText = call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                                                 + ((int)call.Duration).ToDurationString()
                                }).ToList<RecyclerViewItem>()
                            }).ToList<RecyclerViewItem>()
                        },
                        new Item(1)
                        {
                            Text =$"{str_other_calls} "+country.OtherType.CallsCount + $" {str_calls} "
                                  + country.OtherType.CallsDuration.ToDurationString(),
                            Children = country.OtherType.Calls.Select(call=> new Item(2)
                                {
                                    Text = call.Title,
                                    SecondText = call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                                                 + ((int)call.Duration).ToDurationString()
                                }).ToList<RecyclerViewItem>()
                        }
                    }
                }).ToList());
                //foreach (var country in Countries)
                //{
                //    sb.AppendLine(country.CountryName + " " + country.CallsCount
                //                  + $" {str_calls} " + country.CallsDuration.ToDurationString());

                //    sb.AppendLine("+Mobile calls " + country.MobileType.CallsCount
                //                                   + $" {str_calls} " + country.MobileType.CallsDuration.ToDurationString());
                //    foreach (var carrier in country.MobileType.Carriers)
                //    {
                //        sb.AppendLine("++" + carrier.CarrierName + " " + carrier.CallsCount
                //                      + $" {str_calls} " + carrier.CallsDuration.ToDurationString());
                //        foreach (var call in carrier.Calls)
                //        {
                //            sb.AppendLine("+++" + call.Title);
                //            sb.AppendLine("   " + call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                //                          + ((int)call.Duration).ToDurationString());
                //        }
                //    }

                //    sb.AppendLine("+Fixed Line calls " + country.FixedLineType.CallsCount
                //                                   + $" {str_calls} " + country.FixedLineType.CallsDuration.ToDurationString());
                //    foreach (var area in country.FixedLineType.Areas)
                //    {
                //        sb.AppendLine("++" + area.AreaName + " " + area.CallsCount
                //                      + $" {str_calls} " + area.CallsDuration.ToDurationString());
                //        foreach (var call in area.Calls)
                //        {
                //            sb.AppendLine("+++" + call.Title);
                //            sb.AppendLine("   " + call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                //                          + ((int)call.Duration).ToDurationString());
                //        }
                //    }

                //    sb.AppendLine("+Other calls " + country.OtherType.CallsCount
                //                                   + $" {str_calls} " + country.OtherType.CallsDuration + " sec");
                //    foreach (var call in country.OtherType.Calls)
                //    {
                //        sb.AppendLine("+++" + call.Title);
                //        sb.AppendLine("   " + call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                //                      + ((int)call.Duration).ToDurationString());
                //    }
                //}
            }
            else
            {
                throw new ArgumentException("Unknown data type!");
            }

            return items;
        }

        private string str_calls;
        private string str_all_calls;
        private string str_mobile_calls;
        private string str_fixed_line_calls;
        private string str_other_calls;

        private void InitStrings()
        {
            var context = ContextHolder.Context;
            str_calls = context.GetString(Resource.String.calls);
            str_all_calls = context.GetString(Resource.String.all_calls);
            str_mobile_calls = context.GetString(Resource.String.mobile_calls);
            str_fixed_line_calls = context.GetString(Resource.String.fixed_line_calls);
            str_other_calls = context.GetString(Resource.String.other_calls);
        }
    }
}