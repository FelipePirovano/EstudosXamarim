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

namespace EstudosXamarim.Adapter
{
    public interface ItemClickListener
    {
        void Onclick(View itemView, int position, bool isLongClick);
    }
}