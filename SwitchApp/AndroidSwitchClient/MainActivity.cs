using System;
using System.Net.Http;
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

            _model = new SwitchViewModel(new SwitchService(new HttpClient() {BaseAddress = new Uri("http://pecan:9090/")}));
            
            SetContentView(Resource.Layout.Main);

            _model.PropertyChanged += _model_PropertyChanged;
            RefreshControls();
		}

        void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshControls();
        }

	    private void RefreshControls()
	    {
	        // Get references to UI controls and we update state of controls
            // TODO
	    }

	}
}


