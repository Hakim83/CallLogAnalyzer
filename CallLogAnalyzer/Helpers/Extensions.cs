using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CallLogAnalyzer.Helpers
{
    public static class Extensions
    {
        public static T GetOrAdd<T>(this List<T> list,string propertyName, T obj)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            var element = list.FirstOrDefault(i =>
            (
                propertyInfo.GetValue(i) == propertyInfo.GetValue(obj)
            ));
            if (element==null)
            {
                element = obj;
                list.Add(element);
            }

            return element;
        }

        public static string ToDurationString(this int sec)
        {
            var context = ContextHolder.Context;
            var timeSpan = TimeSpan.FromSeconds(sec);
            string h = context.GetString(Resource.String.h);
            string m = context.GetString(Resource.String.m);
            string s = context.GetString(Resource.String.s);

            StringBuilder sb = new StringBuilder();
            if (timeSpan.TotalHours >= 1)
            {
                sb.Append((int) timeSpan.TotalHours + h + " ");
            }

            if (timeSpan.Minutes > 0)
            {
                sb.Append(timeSpan.Minutes + m + " ");
            }

            sb.Append(timeSpan.Seconds + s);

            return sb.ToString();
        }
    }
}