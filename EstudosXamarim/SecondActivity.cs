using Android.App;
using Android.OS;


namespace EstudosXamarim
{
    [Activity(Label = "Activity1")]
    public class SecondActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.second_activity);

         

        }
    }
}