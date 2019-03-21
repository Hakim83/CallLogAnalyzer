using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using CallLogAnalyzer.Helpers;
using CallLogAnalyzer.Model;
using Com.Multilevelview;
using Java.Util;

namespace CallLogAnalyzer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button SubmitButton;
        private EditText FromEditText;
        private EditText ToEditText;
        private Spinner methodSpinner;
        private LinearLayout contactCallLayout;
        private CheckBox contactCheckBox;
        private LinearLayout contactLayout;
        private RadioButton contactDateRadioButton;
        private RadioButton contactCountRadioButton;
        private RadioButton contactDurationRadioButton;
        private LinearLayout callLayout;
        private RadioButton callDateRadioButton;
        private RadioButton callDurationRadioButton;
        private LinearLayout carrierAreaLayout;
        private Spinner countrySpinner;
        //private TextView codeTextView;

        private const int CallLogRequest = 66;
        private List<CountryInfos> countriesInfo = CountryInfos.GetAllCountries();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            SubmitButton = FindViewById<Button>(Resource.Id.submitButton);
            FromEditText = FindViewById<EditText>(Resource.Id.FromEditText);
            ToEditText = FindViewById<EditText>(Resource.Id.ToEditText);
            methodSpinner = FindViewById<Spinner>(Resource.Id.methodSpinner);
            contactCallLayout = FindViewById<LinearLayout>(Resource.Id.contactsCallsLayout);
            contactCheckBox = FindViewById<CheckBox>(Resource.Id.contactsCheckBox);

            contactLayout = FindViewById<LinearLayout>(Resource.Id.contactsLayout);
            contactDateRadioButton = FindViewById<RadioButton>(Resource.Id.contactDateRadioButton);
            contactCountRadioButton = FindViewById<RadioButton>(Resource.Id.contactCountRadioButton);
            contactDurationRadioButton = FindViewById<RadioButton>(Resource.Id.contactDurationRadioButton);

            callLayout = FindViewById<LinearLayout>(Resource.Id.callsLayout);
            callDateRadioButton = FindViewById<RadioButton>(Resource.Id.callDateRadioButton);
            callDurationRadioButton = FindViewById<RadioButton>(Resource.Id.callDurationRadioButton);

            carrierAreaLayout = FindViewById<LinearLayout>(Resource.Id.carrierAreaLayout);
            countrySpinner = FindViewById<Spinner>(Resource.Id.countrySpinner);
            //codeTextView = FindViewById<TextView>(Resource.Id.codeTextView);

            FromEditText.Click += FromEditText_Click;
            ToEditText.Click += ToEditText_Click;
            SubmitButton.Click += SubmitButton_Click;

            var methodAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem,
                new string[]
                {
                    "All Calls",
                    "Carrier / Area"
                });
            methodSpinner.Adapter = methodAdapter;
            methodSpinner.ItemSelected += MethodSpinner_ItemSelected;

            contactCheckBox.CheckedChange += ContactCheckBox_CheckedChange;

            PhoneNumberInfo.SetDefaultRegionCodeFromDevice(this);
            var countryAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem,
                countriesInfo.Select(i => i.CountryName).ToArray());
            countrySpinner.Adapter = countryAdapter;
            countrySpinner.ItemSelected += CountrySpinner_ItemSelected;
            var initialIndex =
                countriesInfo.FindIndex(i => i.RegionCode.ToUpper() == PhoneNumberInfo.DefaultRegionCode.ToUpper());
            countrySpinner.SetSelection(initialIndex);

            ContextHolder.Context = this;//used to access resources

            EnsureCallPermission();
            //runTest();
            //testImojis();
        }

        private void ContactCheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (contactCheckBox.Checked)
            {
                contactLayout.Visibility = ViewStates.Visible;
                callLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                contactLayout.Visibility = ViewStates.Gone;
                callLayout.Visibility = ViewStates.Visible;
            }
        }

        private void FromEditText_Click(object sender, System.EventArgs e)
        {
            var dateDialog = DatePickerFragment.NewInstance((dateTime) =>
                {
                    FromEditText.Text = dateTime.ToString("dd/MM/yyyy");
                });
            dateDialog.Show(FragmentManager, dateDialog.Tag);
        }

        private void ToEditText_Click(object sender, System.EventArgs e)
        {
            var dateDialog = DatePickerFragment.NewInstance((dateTime) =>
            {
                ToEditText.Text = dateTime.ToString("dd/MM/yyyy");
            });
            dateDialog.Show(FragmentManager, dateDialog.Tag);
        }

        private void SubmitButton_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(FromEditText.Text) || string.IsNullOrEmpty(ToEditText.Text))
            {
                Toast.MakeText(this, "Please select dates!", ToastLength.Long).Show();
                return;
            }

            DateTime fromDate = DateTime.ParseExact(FromEditText.Text, "dd/MM/yyyy", null);
            DateTime toDate = DateTime.ParseExact(ToEditText.Text, "dd/MM/yyyy", null);

            if (toDate < fromDate)
            {
                Toast.MakeText(this, "Please select valid dates!", ToastLength.Long).Show();
                return;
            }

            Intent intent = new Intent(this, typeof(DateAnalysisActivity));

            intent.PutExtra("FromDate", FromEditText.Text);
            intent.PutExtra("ToDate", ToEditText.Text);

            if (methodSpinner.SelectedItemPosition == 0)  // all
            {
                var method = contactCheckBox.Checked ? "Contacts" : "All";
                intent.PutExtra("Method", method);
                string sortBy;
                if (method == "Contacts")
                {
                    if (contactDateRadioButton.Checked)
                    {
                        sortBy = "DateTime";
                    }
                    else if (contactCountRadioButton.Checked)
                    {
                        sortBy = "CallsCount";
                    }
                    else
                    {
                        sortBy = "CallsDuration";
                    }
                }

                else
                {
                    if (callDateRadioButton.Checked)
                    {
                        sortBy = "DateTime";
                    }
                    else
                    {
                        sortBy = "CallsDuration";
                    }
                }

                intent.PutExtra("SortBy", sortBy);
            }
            else        //carrier/area
            {
                intent.PutExtra("Method", "CarrierArea");
                string defaultRegionCode = countriesInfo[countrySpinner.SelectedItemPosition].RegionCode;
                //intent.PutExtra("DefaultRegionCode", defaultRegionCode);
                PhoneNumberInfo.DefaultRegionCode = defaultRegionCode;
            }
            StartActivity(intent);

        }

        private void MethodSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position == 0)
            {
                contactCallLayout.Visibility = ViewStates.Visible;
                carrierAreaLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                contactCallLayout.Visibility = ViewStates.Gone;
                carrierAreaLayout.Visibility = ViewStates.Visible;
            }
        }

        private void CountrySpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //codeTextView.Text = countriesInfo[e.Position].CountryCode.ToString();
        }

        private void EnsureCallPermission()
        {

            // Check whether we don't already have granted a permission
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadCallLog)
                != Permission.Granted)
            {

                // Since we don't granted permission, request permission from user
                ActivityCompat.RequestPermissions(this,
                    new[] { Manifest.Permission.ReadCallLog },
                    CallLogRequest);
            }
            else
            {
                SubmitButton.Enabled = true;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case CallLogRequest:
                    {
                        // If request is cancelled, the result arrays are empty.
                        if (grantResults.Length > 0
                            && grantResults[0] == Permission.Granted)
                        {

                            // permission was granted, yay! we can use it,
                            SubmitButton.Enabled = true;

                        }
                        else
                        {

                            // permission denied, boo! We should keep the call functionality
                            // disabled.. We also my inform user
                            Toast.MakeText(this, "We need call log permission !",
                                ToastLength.Long).Show();
                        }
                        return;
                    }

                    // other 'case' lines to check for other
                    // permissions this app might request
            }
        }

        //void runTest()
        //{
        //    var textView = FindViewById<TextView>(Resource.Id.TestTextView);
        //    //var tm = GetSystemService(TelephonyService) as TelephonyManager;

        //    //PhoneNumberInfo.DefaultRegionCode = tm.NetworkCountryIso;
        //    PhoneNumberInfo.DefaultRegionCode = "SD";
        //    string[] numbers = {"0912874941","+249123233320","00966 563456789","187232323","999","01234"};
        //    var values = numbers.Select(n =>new PhoneNumberInfo(n));
        //    var sb = new StringBuilder();
        //    foreach (var value in values)
        //    {
        //        sb.Append("\nNumber: " + value.Number)
        //            .Append("\nNational Number: " + value.NationalNumber)
        //            .Append("\nCountry Name: "+value.CountryName)
        //            .Append("\nCountry Code: " + value.CountryCode)
        //            .Append("\nArea Name: "+value.AreaName)
        //            .Append("\nNumber Type: " + value.PhoneNumberType.ToString())
        //            .Append("\nCarrier Name: " + value.CarrierName)
        //            .AppendLine();
        //    }

        //    textView.Text = sb.ToString();
        //}

        //void testImojis()
        //{
        //    var textView = FindViewById<TextView>(Resource.Id.TestTextView);
        //    textView.Text = CountryInfos.GetCountriesFlags();
        //}
    }
}