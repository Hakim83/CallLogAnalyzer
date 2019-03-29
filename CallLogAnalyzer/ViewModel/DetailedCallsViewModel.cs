using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Telecom;
using CallLogAnalyzer.Helpers;
using CallLogAnalyzer.Model;
using Com.Google.I18n.Phonenumbers;

namespace CallLogAnalyzer.ViewModel
{
    public class DetailedCallsViewModel
    {
        public DetailedCallsViewModel(IEnumerable<CallInfo> calls)
        {
            Countries = calls.Select(c => new DetailedCallViewModel(c))
                .GroupBy(
                c => c.CountryName,
                (countryName, countryCalls) =>
                {
                    var detailedCalls = countryCalls.ToList();
                    return new CountryViewModel
                    {
                        CountryName = countryName,
                        MobileType = new CountryViewModel.MobileTypeViewModel
                        {
                            Carriers = detailedCalls
                                .Where(c => c.PhoneNumberType == PhoneNumberUtil.PhoneNumberType.Mobile)
                                .GroupBy(c => c.CarrierName, (carrierName, carrierCalls) =>
                                    new CountryViewModel.MobileTypeViewModel.CarrierViewModel
                                    {
                                        CarrierName = carrierName,
                                        Calls = carrierCalls.Select(detailCall => detailCall.CallInfo)
                                    })
                        },
                        FixedLineType = new CountryViewModel.FixedLineTypeViewModel
                        {
                            Areas = detailedCalls
                                .Where(c => c.PhoneNumberType == PhoneNumberUtil.PhoneNumberType.FixedLine)
                                .GroupBy(c => c.AreaName, (areaName, areaCalls) =>
                                    new CountryViewModel.FixedLineTypeViewModel.AreaViewModel
                                    {
                                        AreaName = areaName,
                                        Calls = areaCalls.Select(detailCall => detailCall.CallInfo)
                                    })
                        },
                        OtherType = new CountryViewModel.OtherTypeViewModel
                        {
                            Calls = detailedCalls
                                .Where(c => c.PhoneNumberType != PhoneNumberUtil.PhoneNumberType.Mobile &&
                                            c.PhoneNumberType != PhoneNumberUtil.PhoneNumberType.FixedLine)
                                .Select(detailCall => detailCall.CallInfo)
                        }
                    };
                });
        }

        public int CallsCount => Countries.Sum(c => c.CallsCount);
        public int CallsDuration => Countries.Sum(c => c.CallsDuration);

        public IEnumerable<CountryViewModel> Countries { get; set; }
        public class CountryViewModel
        {
            public string CountryName { get; set; }

            public MobileTypeViewModel MobileType { get; set; }
            public FixedLineTypeViewModel FixedLineType { get; set; }
            public OtherTypeViewModel OtherType { get; set; }

            public int CallsCount => MobileType.CallsCount + FixedLineType.CallsCount + OtherType.CallsCount;
            public int CallsDuration => MobileType.CallsDuration + FixedLineType.CallsDuration + OtherType.CallsDuration;
            public class MobileTypeViewModel
            {
                public int CallsCount => Carriers.Sum(c => c.CallsCount);
                public int CallsDuration => Carriers.Count() >= 0 ? Carriers.Sum(c => (int)c.CallsDuration) : 0;
                public IEnumerable<CarrierViewModel> Carriers { get; set; }
                public class CarrierViewModel
                {
                    public string CarrierName { get; set; }
                    public int CallsCount => Calls.Count();
                    public int CallsDuration => Calls.Count() >= 0 ? Calls.Sum(c => (int)c.Duration) : 0;
                    public IEnumerable<CallInfo> Calls { get; set; }
                }
            }
            public class FixedLineTypeViewModel
            {
                public IEnumerable<AreaViewModel> Areas { get; set; }
                public int CallsCount => Areas.Sum(a => a.CallsCount);
                public int CallsDuration => Areas.Count() >= 0 ? Areas.Sum(c => (int)c.CallsDuration) : 0;
                public class AreaViewModel
                {
                    public string AreaName { get; set; }
                    public int CallsCount => Calls.Count();
                    public int CallsDuration => Calls.Count() >= 0 ? Calls.Sum(c => (int)c.Duration) : 0;
                    public IEnumerable<CallInfo> Calls { get; set; }
                }
            }
            public class OtherTypeViewModel
            {
                public int CallsCount => Calls.Count();
                public int CallsDuration => Calls.Count() >= 0 ? Calls.Sum(c => (int)c.Duration) : 0;
                public IEnumerable<CallInfo> Calls { get; set; }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var country in Countries)
            {
                sb.AppendLine(country.CountryName + " " + country.CallsCount
                              + " calls " + country.CallsDuration.ToDurationString());

                sb.AppendLine("+Mobile calls " + country.MobileType.CallsCount
                                               + " calls " + country.MobileType.CallsDuration.ToDurationString());
                foreach (var carrier in country.MobileType.Carriers)
                {
                    sb.AppendLine("++" + carrier.CarrierName + " " + carrier.CallsCount
                                  + " calls " + carrier.CallsDuration.ToDurationString());
                    foreach (var call in carrier.Calls)
                    {
                        sb.AppendLine("+++" + call.Title);
                        sb.AppendLine("   " + call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                                      + ((int)call.Duration).ToDurationString());
                    }
                }

                sb.AppendLine("+Fixed Line calls " + country.FixedLineType.CallsCount
                                               + " calls " + country.FixedLineType.CallsDuration.ToDurationString());
                foreach (var area in country.FixedLineType.Areas)
                {
                    sb.AppendLine("++" + area.AreaName + " " + area.CallsCount
                                  + " calls " + area.CallsDuration.ToDurationString());
                    foreach (var call in area.Calls)
                    {
                        sb.AppendLine("+++" + call.Title);
                        sb.AppendLine("   " + call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                                      + ((int)call.Duration).ToDurationString());
                    }
                }

                sb.AppendLine("+Other calls " + country.OtherType.CallsCount
                                               + " calls " + country.OtherType.CallsDuration + " sec");
                foreach (var call in country.OtherType.Calls)
                {
                    sb.AppendLine("+++" + call.Title);
                    sb.AppendLine("   " + call.Type + " " + call.DateTime.ToString("dd/MM HH:mm ")
                                  + ((int)call.Duration).ToDurationString());
                }
            }

            return sb.ToString();
        }
    }

    public class DetailedCallViewModel
    {
        public string CountryName => _phoneNumberInfo.CountryName;
        public string AreaName => _phoneNumberInfo.AreaName;
        public string CarrierName => _phoneNumberInfo.CarrierName;
        public PhoneNumberUtil.PhoneNumberType PhoneNumberType => _phoneNumberInfo.PhoneNumberType;

        public CallInfo CallInfo { get; set; }
        private readonly PhoneNumberInfo _phoneNumberInfo;

        public DetailedCallViewModel(CallInfo callInfo)
        {
            CallInfo = callInfo;
            _phoneNumberInfo = new PhoneNumberInfo(callInfo.Number);
        }
    }
}