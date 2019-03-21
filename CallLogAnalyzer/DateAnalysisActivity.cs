using System;
using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CallLogAnalyzer.Model;
using CallLogAnalyzer.ViewModel;
using Java.IO;
using Java.Lang;
using MultilevelView;
using CallType = CallLogAnalyzer.Model.CallType;
using Console = Java.IO.Console;
using String = System.String;

namespace CallLogAnalyzer
{
    [Activity(Label = "DateAnalysisActivity")]
    public class DateAnalysisActivity : Activity
    {
        private int callCount = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_data_analysis);
            string fromDateString = Intent.GetStringExtra("FromDate");
            string toDateString = Intent.GetStringExtra("ToDate");
            DateTime fromDate = DateTime.ParseExact(fromDateString, "dd/MM/yyyy", null);
            DateTime toDate = DateTime.ParseExact(toDateString, "dd/MM/yyyy", null);
            toDate += new TimeSpan(23, 59, 59); //include whole date!

            //TextView textView = FindViewById<TextView>(Resource.Id.TextView);
            var allCalls = GetCallDetails(fromDate, toDate);
            Log.Info("count", allCalls.Count.ToString());

            string method = Intent.GetStringExtra("Method");
            if (method == "All")
            {
                var sortBy = Intent.GetStringExtra("SortBy");
                var callsViewModel = new CallsViewModel(allCalls,sortBy);
                textView.Text = callsViewModel.ToString();
            }
            else if(method=="Contacts")
            {
                var sortBy = Intent.GetStringExtra("SortBy");
                var callersViewModel = new CallersViewModel(allCalls, sortBy);
                textView.Text = callersViewModel.ToString();
            }
            else
            {
               var detailedCallsViewModel = new DetailedCallsViewModel(allCalls);
               textView.Text = detailedCallsViewModel.ToString();
            }

            //listview and updates
            MultiLevelRecyclerView multiLevelRecyclerView = (MultiLevelRecyclerView)FindViewById(Resource.Id.MultiLevelView);
            multiLevelRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

            var itemList = recursivePopulateFakeData(0, 24);

            MyAdapter myAdapter = new MyAdapter(this, itemList.Cast<RecyclerViewItem>().ToList(), multiLevelRecyclerView);

            multiLevelRecyclerView.SetAdapter(myAdapter);
        }

        private List<CallInfo> GetCallDetails(DateTime fromDate, DateTime toDate)
        {

            var allCalls = new List<CallInfo>();

            var uri = CallLog.Calls.ContentUri;
            var selection = CallLog.Calls.Date + " BETWEEN ? AND ?";
            var selectionArgs = new string[]
                {DateTimeToJavaMilliSec(fromDate).ToString(), DateTimeToJavaMilliSec(toDate).ToString()};
            var sortOrder = CallLog.Calls.Date + " DESC";

            var cursor = ContentResolver.Query(uri, null, selection, selectionArgs, sortOrder);
            int number = cursor.GetColumnIndex(CallLog.Calls.Number);
            int type = cursor.GetColumnIndex(CallLog.Calls.Type);
            int date = cursor.GetColumnIndex(CallLog.Calls.Date);
            int duration = cursor.GetColumnIndex(CallLog.Calls.Duration);
            int name = cursor.GetColumnIndex(CallLog.Calls.CachedName);
            
            while (cursor.MoveToNext())
            {
                String callNumber = cursor.GetString(number);
                String callType = cursor.GetString(type);
                String callDate = cursor.GetString(date);
                String callDuration = cursor.GetString(duration);
                string callerName = cursor.GetString(name);

                var callInfo = new CallInfo
                {
                    CallerName = callerName,
                    DateTime = new DateTime(1970, 1, 1)
                        .AddMilliseconds(long.Parse(callDate)).ToLocalTime(),
                    Duration = long.Parse(callDuration),
                    Number = callNumber,
                    Type = (CallType) (int.Parse(callType))
                };
                allCalls.Add(callInfo);
            }
            cursor.Close();
            return allCalls;
        }

        long DateTimeToJavaMilliSec(DateTime dateTime)
        {
            var timeSpan = (dateTime - new DateTime(1970, 1, 1));
            return (long)timeSpan.TotalMilliseconds;
        }
    }
}