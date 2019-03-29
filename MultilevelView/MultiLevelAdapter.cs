
using System.Collections.Generic;
using Android.Support.V7.Widget;

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