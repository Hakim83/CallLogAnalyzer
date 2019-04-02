using System;
using Android.App;
using Android.OS;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace CallLogAnalyzer.Dialogs
{
    public class RawSortDialog:DialogFragment
    {
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            builder.SetTitle(Resource.String.sort_calls_by)
                .SetItems(Resource.Array.raw_sort_options, (se, ev) =>
                {
                    switch (ev.Which)
                    {
                        case 0:
                            SortMethodSelected?.Invoke("DateTime");
                            break;
                        case 1:
                            SortMethodSelected?.Invoke("CallsDuration");
                            break;
                    }
                    
                });
            return builder.Create();
        }

        public event Action<string> SortMethodSelected;
    }
}