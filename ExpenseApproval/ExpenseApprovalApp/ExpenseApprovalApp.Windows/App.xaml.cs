using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using ExpenseApprovalAppLogic;
using ExpenseApprovalAppLogic.Links;
using ExpenseApprovalAppLogic.ViewModels;

namespace ExpenseApprovalApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {

        private ExpenseAppClientState _ClientState;
        private HttpClient _httpClient;

        public App()
        {
            this.InitializeComponent();
      
            _httpClient = new HttpClient();
            _ClientState = new ExpenseAppClientState(_httpClient);
            _ClientState.PropertyChanged += _ClientState_PropertyChanged;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
        
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Restore ClientState object
                }

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                _ClientState.FollowLinkAsync(new HomeLink());  // Application starts
                
            }
            
            Window.Current.Activate();
        }

        async void _ClientState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            switch (e.PropertyName)
            {
                case "UserMessage":
                    ShowMessage(_ClientState.UserMessage);
                    break;
                case "CurrentCollection":

                    // If we are already displaying a collection, then simply refresh, don't navigate to a new page
                    if (rootFrame.CurrentSourcePageType == typeof(ListPage))
                    {
                        var listPage = rootFrame.Content as ListPage;
                        listPage.ViewModel.RefreshItems();
                    }
                    else
                    {
                        // Create the ListViewModel
                        rootFrame.Navigate(typeof (ListPage), _ClientState);
                    }
                    break;
                case "CurrentImage":
                {
                    var model = new ImagePageViewModel(_ClientState);
                    rootFrame.Navigate(typeof(ImagePage), model);
                }
                    break;
                case "CurrentFile":
                    await ShowFile(_ClientState.CurrentFile);
                    break;
            }
        }

        private async Task ShowFile(Stream currentFile)
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;

            var file = await folder.CreateFileAsync(Guid.NewGuid().ToString() + ".pdf", CreationCollisionOption.ReplaceExisting);
            var ms = currentFile as MemoryStream;

            await FileIO.WriteBytesAsync(file, ms.ToArray());

            Windows.System.Launcher.LaunchFileAsync(file);
        }

        private void ShowMessage(string message)
        {
            var msgDialog = new MessageDialog(message, "Alert");

            //OK Button
            var okBtn = new UICommand("OK");
            msgDialog.Commands.Add(okBtn);

            //Show message
            msgDialog.ShowAsync();
        }

    }
}