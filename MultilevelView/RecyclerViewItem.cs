using System.Collections.Generic;

namespace MultilevelView
{
    public class RecyclerViewItem
    {
        public IList<RecyclerViewItem> Children;

        public int Level { get; set; }

        public int Position { get; set; }

        public bool Expanded { get; set; } 

        protected RecyclerViewItem(int level)
        {
            Level = level;
        }

        public void AddChildren(IList<RecyclerViewItem> children)
        {
            Children = children;
        }

        public bool HasChildren
        {
            get
            {
                if (Children != null && Children.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}