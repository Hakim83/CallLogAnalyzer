
using Android.Content;
using Android.Telephony;
using Com.Google.I18n.Phonenumbers;
using Com.Google.I18n.Phonenumbers.Geocoding;
using Java.Util;

namespace CallLogAnalyzer.Model
{
    public class PhoneNumberInfo
    {
        public string Number { get; set; }
        public int CountryCode { get; set; }    //e.g. 249 for Sudan
        public string RegionCode { get; set; }  //e.g. "SD" for Sudan
        public string CountryName { get; set; }
        public long NationalNumber { get; set; }
        public string AreaName { get; set; }
        public string CarrierName { get; set; }
        public PhoneNumberUtil.PhoneNumberType PhoneNumberType { get; set; }

        public static string DefaultRegionCode { get; set; } = Locale.Default.Country;

        public PhoneNumberInfo(string number)
        {
            //initialization

            Number = number;
            Phonenumber.PhoneNumber phoneNumber;
            try
            {
                phoneNumber = PhoneNumberUtil.Instance.Parse(number, DefaultRegionCode);
                CountryCode = phoneNumber.CountryCode;
                NationalNumber = phoneNumber.NationalNumber;
                PhoneNumberType = PhoneNumberUtil.Instance.GetNumberType(phoneNumber);
                AreaName = PhoneNumberOfflineGeocoder.Instance.GetDescriptionForNumber(phoneNumber, Locale.English);
                CarrierName = PhoneNumberToCarrierMapper.Instance.GetNameForNumber(phoneNumber, Locale.English);
            }
            catch (NumberParseException ex)
            {
                PhoneNumberType = PhoneNumberUtil.PhoneNumberType.Unknown;
                CountryCode = PhoneNumberUtil.Instance.GetCountryCodeForRegion(DefaultRegionCode);
                return;
            }
            finally
            {
                RegionCode = PhoneNumberUtil.Instance.GetRegionCodeForCountryCode(CountryCode);
                CountryName = new Locale("", RegionCode).GetDisplayCountry(Locale.Default);
            }
        }

        public static void SetDefaultRegionCodeFromDevice(Context context)
        {
            var telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
            var code = telephonyManager.NetworkCountryIso;
            if (string.IsNullOrEmpty(code))
            {
                code = Locale.Default.Country;
            }

            DefaultRegionCode = code;
        }
    }
}