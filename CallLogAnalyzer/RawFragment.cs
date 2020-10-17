using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using CallLogAnalyzer.Dialogs;
using CallLogAnalyzer.ViewModel;
using MultilevelView;
using System.Collections.Generic;
using System.Diagnostics;

namespace CallLogAnalyzer
{
    public class RawFragment : Fragment
    {
        private IList<RecyclerViewItem> itemList;
        private MultiLevelRecyclerView multiLevelRecyclerView;
        private MyAdapter myAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.fragment_layout, null);

            var callsViewModel = new CallsViewModel(AnalysisActivity.AllCalls, "DateTime");
            itemList = new ListViewItemsBuilder().GetItems(callsViewModel);

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
                //Toast.MakeText(Activity,"clicked!",ToastLength.Long).Show();
                RawSortDialog dialog = new RawSortDialog();
                dialog.SortMethodSelected += UpdateItems;
                dialog.Show(Activity.SupportFragmentManager, "SortBy dialog");
            };

            return view;
        }

        private void UpdateItems(string sortBy)
        {
            var callsViewModel = new CallsViewModel(AnalysisActivity.AllCalls, sortBy);

            myAdapter.ListItems = new ListViewItemsBuilder().GetItems(callsViewModel);
            myAdapter.NotifyDataSetChanged();
            multiLevelRecyclerView.OpenTill(0);
        }
    }
}