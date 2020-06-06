using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace FutbolPlayAdmin.Droid
{
    [Activity(Label = "FutbolPlayAdmin", Icon = "@drawable/icon", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Theme = "@style/AppTheme"
        )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.ActionBarOverlay);
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

