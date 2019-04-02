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
using CallLogAnalyzer.ViewModel;
using MultilevelView;

namespace CallLogAnalyzer
{
    public class ContactsFragment : Fragment
    {
        private IList<RecyclerViewItem> itemList;
        private MyAdapter myAdapter;
        private MultiLevelRecyclerView multiLevelRecyclerView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.fragment_layout, null);

            var callersViewModel = new CallersViewModel(AnalysisActivity.AllCalls, "DateTime");
            itemList = new ListViewItemsBuilder().GetItems(callersViewModel);
            
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
            fab.Click += (se, ev) =>
            {
                //Toast.MakeText(Activity, "Hello World thank you!", ToastLength.Long).Show();
                var dialog = new ContactsSortDialog();
                dialog.SortMethodSelected += UpdateItems;
                dialog.Show(Activity.SupportFragmentManager, "SortBy dialog");
            };
            return view;
        }

        private void UpdateItems(string sortBy)
        {
            var callersViewModel = new CallersViewModel(AnalysisActivity.AllCalls, sortBy);
            
            myAdapter.ListItems = new ListViewItemsBuilder().GetItems(callersViewModel);
            myAdapter.NotifyDataSetChanged();
            multiLevelRecyclerView.OpenTill(0);
        }
    }
}