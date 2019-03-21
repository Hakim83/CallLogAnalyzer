using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CallLogAnalyzer.Helpers
{
    public class ContextHolder
    {
        public static Context Context { get; set; }
        public static Resources Resources => Context.Resources;
    }
}