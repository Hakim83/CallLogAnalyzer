using MultilevelView;

namespace CallLogAnalyzer
{
    public class Item : RecyclerViewItem
    {
        public string Text { get; set; }
        public string SecondText { get; set; }
        

        public Item(int level) : base(level)
        {
        }
    }
}