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
using CallLogAnalyzer.ViewModel;
using MultilevelView;

namespace CallLogAnalyzer
{
    public class RawFragment : Fragment
    {
        private IList<RecyclerViewItem> itemList;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.fragment_layout, null);

            var callsViewModel = new CallsViewModel(AnalysisActivity.AllCalls, "DateTime");
            itemList = new ListViewItemsBuilder().GetItems(callsViewModel);

            //listview and updates
            MultiLevelRecyclerView multiLevelRecyclerView = (MultiLevelRecyclerView)view.FindViewById(Resource.Id.MultiLevelView);
            multiLevelRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));

            //itemList = recursivePopulateFakeData(0, 24);

            MyAdapter myAdapter = new MyAdapter(Activity, itemList, multiLevelRecyclerView);

            multiLevelRecyclerView.SetAdapter(myAdapter);
            multiLevelRecyclerView.ToggleItemOnClick = false;
            multiLevelRecyclerView.Accordion = true;
            multiLevelRecyclerView.OpenTill(0);

            FloatingActionButton fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (se, ev) =>
            {
                Toast.MakeText(Activity,"clicked!",ToastLength.Long).Show();
            };
            return view;
        }
    }
}