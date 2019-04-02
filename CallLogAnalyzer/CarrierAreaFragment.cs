using System;
using System.Collections.Generic;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CallLogAnalyzer.Dialogs;
using CallLogAnalyzer.Model;
using CallLogAnalyzer.ViewModel;
using MultilevelView;

namespace CallLogAnalyzer
{
    public class CarrierAreaFragment : Fragment
    {
        private IList<RecyclerViewItem> itemList;
        private MultiLevelRecyclerView multiLevelRecyclerView;
        private MyAdapter myAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.fragment_layout, null);

            var detailedCallsViewModel = new DetailedCallsViewModel(AnalysisActivity.AllCalls);
            itemList = new ListViewItemsBuilder().GetItems(detailedCallsViewModel);
            
            //listview and updates
            multiLevelRecyclerView = (MultiLevelRecyclerView)view.FindViewById(Resource.Id.MultiLevelView);
            multiLevelRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));

            //itemList = recursivePopulateFakeData(0, 24);

            myAdapter = new MyAdapter(Activity, itemList, multiLevelRecyclerView);

            multiLevelRecyclerView.SetAdapter(myAdapter);
            multiLevelRecyclerView.ToggleItemOnClick = false;
            multiLevelRecyclerView.Accordion = true;
            multiLevelRecyclerView.OpenTill(0);

            FloatingActionButton fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.SetImageResource(Resource.Drawable.ic_flag);
            fab.Click += (se, ev) =>
            {
                //Toast.MakeText(Activity,"clicked!",ToastLength.Long).Show();
                var dialog = new CountryPicker();
                dialog.RegionCodeSelected += UpdateItems;
                dialog.Show(Activity.SupportFragmentManager, "Countries dialog");
            };
            return view;
        }
        private void UpdateItems(string regionCode)
        {
            PhoneNumberInfo.DefaultRegionCode = regionCode;
            var detailedCallsViewModel = new DetailedCallsViewModel(AnalysisActivity.AllCalls);

            myAdapter.ListItems = new ListViewItemsBuilder().GetItems(detailedCallsViewModel);
            myAdapter.NotifyDataSetChanged();
            multiLevelRecyclerView.OpenTill(0);
        }
    }
}