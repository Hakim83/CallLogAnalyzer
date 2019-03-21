using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MultilevelView
{
    public abstract class MultiLevelAdapter : RecyclerView.Adapter
    {
        public IList<RecyclerViewItem> RecyclerViewItemList { get; set; }

        protected MultiLevelAdapter(IList<RecyclerViewItem> recyclerViewItemList)
        {
            RecyclerViewItemList = recyclerViewItemList;
        }

        public override int ItemCount => RecyclerViewItemList.Count;

        public override int GetItemViewType(int position)
        {
            return RecyclerViewItemList[position].Level;
        }
    }
}