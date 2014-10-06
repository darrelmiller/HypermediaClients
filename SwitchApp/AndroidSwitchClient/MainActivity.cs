using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Org.Apache.Http.Client.Params;
using SwitchClient;
using SwitchClient.Classic;

namespace AndroidSwitchClient
{
	// the ConfigurationChanges flags set here keep the EGL context
	// from being destroyed whenever the device is rotated or the
	// keyboard is shown (highly recommended for all GL apps)
	[Activity (Label = "AndroidSwitchClient",
				#if __ANDROID_11__
				HardwareAccelerated=false,
				#endif
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden,
		MainLauncher = true,
		Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
	    private ISwitchViewModel _model;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            _model = new SwitchViewModel(new SwitchService(new HttpClient() {}));
            SetContentView(Resource.Layout.Main);

		}

	}
}


