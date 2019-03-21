using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MultilevelView
{
    public class MultiLevelRecyclerView : RecyclerView
    {
        Context mContext;

        public bool Accordion { get; set; } = false;

        private int prevClickedPosition = -1, numberOfItemsAdded = 0;

        private MultiLevelAdapter mMultiLevelAdapter;

        public bool ToggleItemOnClick { get; set; }= true;

        //private GestureDetector gestureDetector;
        public MultiLevelRecyclerView(Context context) : base(context)
        {
            mContext = context;
            SetUp(context);
        }

        public MultiLevelRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetUp(context);
        }

        public MultiLevelRecyclerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            SetUp(context);
        }

        private void SetUp(Context context)
        {
            ItemClick += MultiLevelRecyclerView_ItemClick;
        }

        class SimpleGestureListener : GestureDetector.SimpleOnGestureListener
        {
            public override bool OnSingleTapUp(MotionEvent e)
            {
                return true;
            }
        }

        public override void SetAdapter(Adapter adapter)
        {
            mMultiLevelAdapter = (MultiLevelAdapter)adapter;
            base.SetAdapter(adapter);
        }

        private void MultiLevelRecyclerView_ItemClick(View view, RecyclerViewItem item, int position)
        {
            if (ToggleItemOnClick)
            {
                ToggleItemsGroup(position);
            }
        }

        public void RemoveAllChildren(IList<RecyclerViewItem> list)
        {
            foreach (RecyclerViewItem i in list)
            {
                if (i.Expanded)
                {
                    i.Expanded=false;
                    RemoveAllChildren(i.Children);
                    RemovePrevItems(mMultiLevelAdapter.RecyclerViewItemList, i.Position, i.Children.Count);
                }
            }
        }

        private void RemovePrevItems(IList<RecyclerViewItem> tempList, int position, int numberOfItemsAdded)
        {
            for (int i = 0; i < numberOfItemsAdded; i++)
            {
                tempList.RemoveAt(position + 1);
            }
            
            mMultiLevelAdapter.RecyclerViewItemList = tempList;
            mMultiLevelAdapter.NotifyItemRangeRemoved(position + 1, numberOfItemsAdded);

            RefreshPosition();
        }

        public void RefreshPosition()
        {
            int position = 0;
            foreach (RecyclerViewItem i in mMultiLevelAdapter.RecyclerViewItemList)
            {
                i.Position =position++;
            }
        }

        private int GetExpandedPosition(int level)
        {
            IList<RecyclerViewItem> adapterList = mMultiLevelAdapter.RecyclerViewItemList;
            foreach (RecyclerViewItem i in adapterList)
            {
                if (level == i.Level && i.Expanded)
                {
                    return adapterList.IndexOf(i);
                }
            }

            return -1;
        }

        private int GetItemsToBeRemoved(int level)
        {
            IList<RecyclerViewItem> adapterList = mMultiLevelAdapter.RecyclerViewItemList;
            int itemsToRemove = 0;
            foreach (RecyclerViewItem i in adapterList)
            {
                if (level < i.Level)
                {
                    itemsToRemove++;
                }
            }
            return itemsToRemove;
        }

        public void OpenTill(params int[] positions)
        {
            if (mMultiLevelAdapter == null)
            {
                return;
            }
            IList<RecyclerViewItem> adapterList = mMultiLevelAdapter.RecyclerViewItemList;
            if (adapterList == null || positions.Length <= 0)
            {
                return;
            }
            int posToAdd = 0;
            int insidePosStart = -1;
            int insidePosEnd = adapterList.Count;
            foreach (int position in positions)
            {
                posToAdd += position;
                if (posToAdd > insidePosStart && posToAdd < insidePosEnd)
                {
                    AddItems(adapterList[posToAdd], adapterList, posToAdd);
                    insidePosStart = posToAdd;
                    if (adapterList[posToAdd].Children == null)
                    {
                        break;
                    }
                    insidePosEnd = adapterList[posToAdd].Children.Count;
                    posToAdd += 1;
                }
            }
        }

        private void AddItems(RecyclerViewItem clickedItem, IList<RecyclerViewItem> tempList, int position)
        {

            if (clickedItem.HasChildren)
            {
                prevClickedPosition = position;

                for (int i=0;i< clickedItem.Children.Count;i++)
                {
                    tempList.Insert(position+1+i,clickedItem.Children[i]);
                }
                //tempList.AddAll(position + 1, clickedItem.Children);

                clickedItem.Expanded=true;

                numberOfItemsAdded = clickedItem.Children.Count;

                mMultiLevelAdapter.RecyclerViewItemList=tempList;

                mMultiLevelAdapter.NotifyItemRangeInserted(position + 1, clickedItem.Children.Count);

                SmoothScrollToPosition(position);
                RefreshPosition();

            }
        }


        public void ToggleItemsGroup(int position)
        {
            if (position == -1) return;

            IList<RecyclerViewItem> adapterList = mMultiLevelAdapter.RecyclerViewItemList;

            RecyclerViewItem clickedItem = adapterList[position];

            if (Accordion)
            {
                if (clickedItem.Expanded)
                {
                    clickedItem.Expanded=false;
                    RemoveAllChildren(clickedItem.Children);
                    RemovePrevItems(adapterList, position, clickedItem.Children.Count);
                    prevClickedPosition = -1;
                    numberOfItemsAdded = 0;
                }
                else
                {
                    int i = GetExpandedPosition(clickedItem.Level);
                    int itemsToRemove = GetItemsToBeRemoved(clickedItem.Level);

                    if (i != -1)
                    {
                        RemovePrevItems(adapterList, i, itemsToRemove);

                        adapterList[i].Expanded=false;

                        if (clickedItem.Position > adapterList[i].Position)
                        {
                            AddItems(clickedItem, adapterList, position - itemsToRemove);
                        }
                        else
                        {
                            AddItems(clickedItem, adapterList, position);
                        }
                    }
                    else
                    {
                        AddItems(clickedItem, adapterList, position);
                    }
                }
            }
            else
            {
                if (clickedItem.Expanded)
                {
                    clickedItem.Expanded=false;
                    RemoveAllChildren(clickedItem.Children);
                    RemovePrevItems(adapterList, position, clickedItem.Children.Count);
                    prevClickedPosition = -1;
                    numberOfItemsAdded = 0;
                }
                else
                {
                    if (clickedItem.Expanded)
                    {
                        RemovePrevItems(adapterList, prevClickedPosition, numberOfItemsAdded);
                        AddItems(clickedItem, adapterList, clickedItem.Position);
                    }
                    else
                    {
                        AddItems(clickedItem, adapterList, position);
                    }
                }
            }
        }



        public delegate void ItemClickDelegate(View view, RecyclerViewItem item, int position);

        public event ItemClickDelegate ItemClick;

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            View childView = FindChildViewUnder(ev.GetX(), ev.GetY());
            if (childView != null )
            {
                childView.PerformClick();
                int position = GetChildAdapterPosition(childView);
                ItemClick?.Invoke(childView, mMultiLevelAdapter.RecyclerViewItemList[position], position);
                
                return ToggleItemOnClick;
            }
            return false;
        }
        
    }
}