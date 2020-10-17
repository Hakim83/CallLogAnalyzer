using Android;
using Android.App;
using Android.App.Roles;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Telecom;
using Android.Widget;
using CallLogAnalyzer.Helpers;
using Java.Util;
using System;
using System.Collections.Generic;
using DatePicker = CallLogAnalyzer.Dialogs.DatePicker;

namespace CallLogAnalyzer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    [IntentFilter(new[] { Intent.ActionDial},Categories = new[] { Intent.CategoryDefault})]
    [IntentFilter(new[] { Intent.ActionDial},Categories = new[] { Intent.CategoryDefault},DataScheme ="tel")]
    public class MainActivity : AppCompatActivity
    {
        private Button SubmitButton;
        private EditText FromEditText;
        private EditText ToEditText;

        //private const int CallDefaultHandlerRequest = 64;
        private const int DefaultDialerRequest = 65;
        private const int CallLogRequest = 66;

        private RoleManager roleManager;

        private List<CountryInfos> countriesInfo = CountryInfos.GetAllCountries(Locale.Default.Country);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            SubmitButton = FindViewById<Button>(Resource.Id.submitButton);
            FromEditText = FindViewById<EditText>(Resource.Id.FromEditText);
            ToEditText = FindViewById<EditText>(Resource.Id.ToEditText);

            FromEditText.Click += FromEditText_Click;
            ToEditText.Click += ToEditText_Click;
            SubmitButton.Click += SubmitButton_Click;

            ContextHolder.Context = this;//used to access resources

            EnsureCallPermission();
        }

        private void FromEditText_Click(object sender, System.EventArgs e)
        {
            var dateDialog = DatePicker.NewInstance((dateTime) =>
                {
                    FromEditText.Text = dateTime.ToString("dd/MM/yyyy");
                });
            dateDialog.Show(SupportFragmentManager, dateDialog.Tag);
        }

        private void ToEditText_Click(object sender, System.EventArgs e)
        {
            var dateDialog = DatePicker.NewInstance((dateTime) =>
            {
                ToEditText.Text = dateTime.ToString("dd/MM/yyyy");
            });
            dateDialog.Show(SupportFragmentManager, dateDialog.Tag);
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

            Intent intent = new Intent(this, typeof(AnalysisActivity));

            intent.PutExtra("FromDate", FromEditText.Text);
            intent.PutExtra("ToDate", ToEditText.Text);

            StartActivity(intent);

        }

        private void EnsureCallPermission()
        {

            // Check whether we don't already have granted a permission
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadCallLog)
                != Permission.Granted)
            {

                // before granting permission ensure app default dialer (required by android api)
                RequestDialRole();
            }
            else
            {
                SubmitButton.Enabled = true;
            }
        }

        void RequestCallLogPermission()
        {
            ActivityCompat.RequestPermissions(this,
                new[] { Manifest.Permission.ReadCallLog },
                CallLogRequest);
        }

        private void RequestDialRole()
        {

            //check if requesting default dialer is needed
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                roleManager = (RoleManager)GetSystemService(RoleService);
                if (!roleManager.IsRoleAvailable(RoleManager.RoleDialer) ||
                    roleManager.IsRoleHeld(RoleManager.RoleDialer))
                {
                    RequestCallLogPermission();
                }
                else
                {
                    Intent intent = roleManager.CreateRequestRoleIntent(RoleManager.RoleDialer);
                    StartActivityForResult(intent, DefaultDialerRequest);
                }
            }
            else
            {
                Intent intent = new Intent(TelecomManager.ActionChangeDefaultDialer);
                intent.PutExtra(TelecomManager.ExtraChangeDefaultDialerPackageName, PackageName);
                StartActivityForResult(intent,DefaultDialerRequest);
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
                            Toast.MakeText(this, GetString(Resource.String.we_need_permission),
                                ToastLength.Long).Show();
                        }
                        return;
                    }

            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == DefaultDialerRequest)
            {
                if (resultCode == Result.Ok)
                {
                    RequestCallLogPermission();
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.we_need_default_dialer), ToastLength.Long).Show();
                }
            }
            //base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}