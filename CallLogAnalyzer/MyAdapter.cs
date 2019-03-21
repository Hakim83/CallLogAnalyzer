using System;
using System.Collections.Generic;
using Android;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using MultilevelView;

//using Com.Multilevelview;

namespace CallLogAnalyzer
{
    class MyAdapter : MultiLevelAdapter
    {
        private Holder mViewHolder;
        public Context Context;
        public IList<RecyclerViewItem> ListItems;
        public Item CurrentItem;
        public MultiLevelRecyclerView MultiLevelRecyclerView;

        //public MyAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        //{

        //}

        public MyAdapter(Context mContext, IList<RecyclerViewItem> mListItems, MultiLevelRecyclerView mMultiLevelRecyclerView) :
            base(mListItems)
        {
            this.ListItems = mListItems;
            this.Context = mContext;
            this.MultiLevelRecyclerView = mMultiLevelRecyclerView;
        }

        private void setExpandButton(ImageView expandButton, bool isExpanded)
        {
            // set the icon based on the current state
            expandButton.SetImageResource(isExpanded
                ? Resource.Drawable.ic_keyboard_arrow_down_black_24dp
                : Resource.Drawable.ic_keyboard_arrow_up_black_24dp);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new Holder(this,
                LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_layout, parent, false));

        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            mViewHolder = (Holder)holder;
            try
            {
            CurrentItem = (Item)ListItems[position];

            }
            catch (Exception)
            {

                Log.Error("Position Error","Pos:" + position.ToString());
                return;
            }

            var level = GetItemViewType(position);
            var grayValue = 255 - 16 * level;
            holder.ItemView.SetBackgroundColor(new Color(grayValue,grayValue,grayValue));
           

            mViewHolder.mTitle.Text = CurrentItem.Text;
            mViewHolder.mSubtitle.Text = CurrentItem.SecondText;

            if (CurrentItem.HasChildren && CurrentItem.Children.Count > 0)
            {
                setExpandButton(mViewHolder.mExpandIcon, CurrentItem.Expanded);
                mViewHolder.mExpandButton.Visibility = ViewStates.Visible;
            }
            else
            {
                mViewHolder.mExpandButton.Visibility = ViewStates.Gone;
            }

            Log.Error("RecyclerLog:", CurrentItem.Level + " " + CurrentItem.Position + " " + CurrentItem.Expanded + "");

            // indent child items
            // Note: the parent item should start at zero to have no indentation
            // e.g. in populateFakeData(); the very first Item shold be instantiate like this: Item item = new Item(0);
            float density = Context.Resources.DisplayMetrics.Density;
            ((ViewGroup.MarginLayoutParams)mViewHolder.mTextBox.LayoutParameters).LeftMargin =
                (int)((GetItemViewType(position) * 20) * density + 0.5f);

        }

        private class Holder : RecyclerView.ViewHolder
        {

            public TextView mTitle, mSubtitle;
            public ImageView mExpandIcon;
            public LinearLayout mTextBox, mExpandButton;
            MyAdapter mParent;

            public Holder(MyAdapter parent, View itemView) : base(itemView)
            {
                mParent = parent;
                mTitle = (TextView)itemView.FindViewById(Resource.Id.title);
                mSubtitle = (TextView)itemView.FindViewById(Resource.Id.subtitle);
                mExpandIcon = (ImageView)itemView.FindViewById(Resource.Id.image_view);
                mTextBox = (LinearLayout)itemView.FindViewById(Resource.Id.text_box);
                mExpandButton = (LinearLayout)itemView.FindViewById(Resource.Id.expand_field);

                // The following code snippets are only necessary if you set multiLevelRecyclerView.removeItemClickListeners(); in MainActivity.java
                // this enables more than one click event on an item (e.g. Click Event on the item itself and click event on the expand button)
                itemView.Click += (sender, ev) =>
                {
                    //Toast.MakeText(mParent.Context,
                    //    $"Item at position {AdapterPosition} was clicked!", ToastLength.Short
                    //).Show();
                };

                mExpandButton.Click += (sender, ev) =>
                {
                    // set click event on expand button here
                    mParent.MultiLevelRecyclerView.ToggleItemsGroup(AdapterPosition);
                    // rotate the icon based on the current state
                    // but only here because otherwise we'd see the animation on expanded items too while scrolling
                    mExpandIcon.Animate().Rotation(mParent.ListItems[AdapterPosition].Expanded ? -180 : 0).Start();

                    //Toast.MakeText(mParent.Context,
                    //    $"Item at position {AdapterPosition} is expanded: {mParent.CurrentItem.Expanded}",
                    //    ToastLength.Short).Show();

                };
            }
        }


    }
}