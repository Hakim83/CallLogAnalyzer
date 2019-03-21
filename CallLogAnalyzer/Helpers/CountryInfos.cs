using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.I18n.Phonenumbers;
using Com.Google.I18n.Phonenumbers.Geocoding;
using Java.Util;

namespace CallLogAnalyzer.Helpers
{
    public class CountryInfos
    {
        public string RegionCode { get; set; } //e.g. "SD" for sudan
        public int CountryCode { get; set; } //e.g. 249 for sudan
        public string CountryName { get; set; } 
        public string FlagEmojiCode => GetFlagEmoji(RegionCode);

        public static string GetCountriesFlags()
        {
            
            StringBuilder sb = new StringBuilder();
            //Locale[] locales = Locale.GetAvailableLocales()
            //    .Where(l => !string.IsNullOrEmpty(l.Country) && l.Country.All(char.IsLetter))
            //    .GroupBy(l => l.Country, (country, ll) => new Locale("", country))
            //    .OrderBy(l=>l.DisplayName).ToArray();
            //sb.AppendLine("Locales: " + locales.Length);
            //foreach (var locale in locales)
            //{
            //    sb.AppendLine(locale.DisplayName + " (" + locale.Country + ") " + GetFlagEmoji(locale.Country));
            //}
            var locales = PhoneNumberUtil.Instance.SupportedRegions
                .Select(s=>new Locale("",s))
                .OrderBy(l => l.DisplayName)
                .ToArray();
            sb.AppendLine("Locales: " + locales.Length);
            foreach (var locale in locales)
            {
                sb.AppendLine(locale.GetDisplayCountry(new Locale("ar")) + " (" + locale.Country + ") " + GetFlagEmoji(locale.Country));
            }
            return sb.ToString();
        }

        private static string GetFlagEmoji(string regionCode)
        {
            int firstChar = regionCode[0] - 'A' + 0x1F1E6;
            int secondChar = regionCode[1] - 'A' + 0x1F1E6;
            return char.ConvertFromUtf32(firstChar) + char.ConvertFromUtf32(secondChar);
        }

        public static List<CountryInfos> GetAllCountries(string displayLanguage="en")
        {
            var countries = PhoneNumberUtil.Instance.SupportedRegions
                .Select(s => new Locale("", s))
                .Select(l=>new CountryInfos
                {
                    CountryName = l.GetDisplayCountry(new Locale(displayLanguage)),
                    CountryCode = PhoneNumberUtil.Instance.GetCountryCodeForRegion(l.Country),
                    RegionCode = l.Country
                })
                .OrderBy(c => c.CountryName)
                .ToList();
            return countries;
        }
    }
}