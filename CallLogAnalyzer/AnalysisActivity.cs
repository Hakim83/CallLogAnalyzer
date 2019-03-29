using System;
using System.Collections.Generic;
using Android.OS;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CallLogAnalyzer.Model;
using CallLogAnalyzer.ViewModel;
using MultilevelView;
using CallType = CallLogAnalyzer.Model.CallType;

namespace CallLogAnalyzer
{
    [Android.App.Activity(Label = "@string/analysis_result")]
    public class AnalysisActivity : AppCompatActivity
    {
        private int callCount = 0;
        public static List<CallInfo> AllCalls = null;
        private TabLayout tabLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_data_analysis);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);


            string fromDateString = Intent.GetStringExtra("FromDate");
            string toDateString = Intent.GetStringExtra("ToDate");
            DateTime fromDate = DateTime.ParseExact(fromDateString, "dd/MM/yyyy", null);
            DateTime toDate = DateTime.ParseExact(toDateString, "dd/MM/yyyy", null);
            toDate += new TimeSpan(23, 59, 59); //include whole date!
            AllCalls = GetCallDetails(fromDate, toDate);

            _fragments = new Fragment[]
            {
                new RawFragment(),
                new ContactsFragment()
            };


            tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.TabSelected += TabLayout_TabSelected;

            AddTabToActionBar(Resource.String.all_calls, Resource.Drawable.ic_list);
            AddTabToActionBar(Resource.String.by_contacts, Resource.Drawable.ic_contacts);
            //IList<RecyclerViewItem> itemList;

            //string method = Intent.GetStringExtra("Method");
            //if (method == "All")
            //{
            //    var sortBy = Intent.GetStringExtra("SortBy");
            //    var callsViewModel = new CallsViewModel(allCalls,sortBy);
            //    itemList = new ListViewItemsBuilder().GetItems(callsViewModel);
            //    //textView.Text = callsViewModel.ToString();
            //}
            //else if(method=="Contacts")
            //{
            //    var sortBy = Intent.GetStringExtra("SortBy");
            //    var callersViewModel = new CallersViewModel(allCalls, sortBy);
            //    itemList = new ListViewItemsBuilder().GetItems(callersViewModel);
            //    //textView.Text = callersViewModel.ToString();
            //}
            //else
            //{
            //   var detailedCallsViewModel = new DetailedCallsViewModel(allCalls);
            //    itemList = new ListViewItemsBuilder().GetItems(detailedCallsViewModel);
            //   //textView.Text = detailedCallsViewModel.ToString();
            //}

            ////listview and updates
            //MultiLevelRecyclerView multiLevelRecyclerView = (MultiLevelRecyclerView)FindViewById(Resource.Id.MultiLevelView);
            //multiLevelRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

            ////itemList = recursivePopulateFakeData(0, 24);

            //MyAdapter myAdapter = new MyAdapter(this, itemList, multiLevelRecyclerView);

            //multiLevelRecyclerView.SetAdapter(myAdapter);
            //multiLevelRecyclerView.ToggleItemOnClick = false;
            //multiLevelRecyclerView.Accordion = true;
            //multiLevelRecyclerView.OpenTill(0);
        }

        private void TabLayout_TabSelected(object sender, TabLayout.TabSelectedEventArgs e)
        {
            Fragment frag = _fragments[e.Tab.Position];
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout, frag).Commit();
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
                    Type = (CallType)(int.Parse(callType))
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

        private IList<RecyclerViewItem> recursivePopulateFakeData(int levelNumber, int depth)
        {
            IList<RecyclerViewItem> itemList = new List<RecyclerViewItem>();

            String title;
            switch (levelNumber)
            {
                case 1:
                    title = "PQRST ";
                    break;
                case 2:
                    title = "XYZ ";
                    break;
                default:
                    title = "ABCDE ";
                    break;
            }

            for (int i = 0; i < depth; i++)
            {
                Item item = new Item(levelNumber);
                item.Text = title + i;
                item.SecondText = title.ToLower() + i;
                if (depth % 2 == 0)
                {
                    item.AddChildren(recursivePopulateFakeData(levelNumber + 1, depth / 2));
                }

                itemList.Add(item);
            }

            return itemList;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        void AddTabToActionBar(int labelResourceId, int iconResourceId)
        {
            tabLayout.AddTab(tabLayout.NewTab()
                .SetText(labelResourceId)
                .SetIcon(iconResourceId));
        }
        
        Fragment[] _fragments;

        
    }
}