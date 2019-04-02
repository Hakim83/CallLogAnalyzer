using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using CallLogAnalyzer.Helpers;
using CallLogAnalyzer.Model;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using Java.Util;

namespace CallLogAnalyzer.Dialogs
{
    public class CountryPicker:DialogFragment
    {
        private List<CountryInfos> countriesInfo = CountryInfos.GetAllCountries(Locale.Default.Country);

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            int lastSelected =
                countriesInfo.FindIndex(ci =>
                    string.Compare(ci.RegionCode, PhoneNumberInfo.DefaultRegionCode, 
                        CultureInfo.InvariantCulture,
                        CompareOptions.IgnoreCase) == 0);
            string selectedCode = countriesInfo[lastSelected].RegionCode;
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            builder.SetTitle(Resource.String.default_country)
                .SetSingleChoiceItems(countriesInfo.Select(i => i.CountryName).ToArray(), lastSelected, (se, ev) =>
                    {
                        selectedCode = countriesInfo[ev.Which].RegionCode;
                    })
                .SetPositiveButton(Resource.String.ok, ((sender, args) =>
                {
                    RegionCodeSelected?.Invoke(selectedCode);
                }))
                .SetNegativeButton(Resource.String.cancel, ((sender, args) => { }));
            return builder.Create();
        }

        public event Action<string> RegionCodeSelected;
    }
}