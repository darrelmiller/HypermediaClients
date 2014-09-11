using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using ExpenseApprovalApp.Links;
using ExpenseApprovalApp.Tools;
using Newtonsoft.Json;
using Tavis;
using Tavis.Home;
using WebApiContrib.CollectionJson;
using Link = Tavis.Link;

namespace ExpenseApprovalApp
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientState : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private BitmapImage _currentImage;

        public string UserMessage
        {
            get { return _userMessage; }
            private set
            {
                _userMessage = value; 
                OnPropertyChanged();
            }
        }

        public HomeDocument HomeDocument
        {
            get { return _homeDocument; }
            set
            {
                _homeDocument = value;
                OnPropertyChanged();
            }
        }

        public List<ProblemDocument> Problems {get; set; }

        public Collection CurrentCollection
        {
            get { return _currentCollection; }
            set
            {
                _currentCollection = value; 
                OnPropertyChanged();
            }
        }

        public BitmapImage CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                OnPropertyChanged();
            }
        }

        private readonly LinkFactory _linkFactory = new LinkFactory();
        private string _userMessage;
        private HomeDocument _homeDocument;
        private Collection _currentCollection;


        public ClientState(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _linkFactory.AddLinkType<HomeLink>();
            _linkFactory.AddLinkType<ShowLink>();
            _linkFactory.AddLinkType<ActionLink>();
            
            Problems = new List<ProblemDocument>();
        }

        public Task FollowLinkAsync(Link link)
        {
           return ProcessResponseAsync(_httpClient.SendAsync(link.CreateRequest()));
        }

        public async Task ProcessResponseAsync(Task<HttpResponseMessage> responseTask)
        {

            try
            {
                var response = await responseTask;

                var contextLink = response.RequestMessage.ExtractLink();

                if (IsServerError(response.StatusCode)) { ProcessServerErrors(response); return; }
                if (IsClientError(response.StatusCode)) { ProcessClientError(response, contextLink); return; }
                if (IsRedirect(response.StatusCode)) { await ProcessRedirect(response); return; }


                switch (contextLink.Relation)
                {
                    case TavisLinkTypes.Show:
                        await ProcessShowLinkResponse(response, contextLink);
                        break;
                    case TavisLinkTypes.Action:
                        await ProcessActionLinkResponse(response, contextLink);
                        break;
                    case TavisLinkTypes.Home:
                        await ProcessHomeLinkResponse(response, contextLink);
                        break;
                }

            }
            catch (Exception ex)
            {
                UserMessage = " Sorry, the client is having issues :" + ex.Message;
            }
        }

        private async Task ProcessHomeLinkResponse(HttpResponseMessage response, Link contextLink)
        {
            if (!HasContent(response)) return;
 
            var contentStream = await response.Content.ReadAsStreamAsync();

            ShowLink showLink = null;  // Need to find a showlink to follow

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/home+json":
                    HomeDocument = HomeDocument.Parse(contentStream, _linkFactory);
                    showLink = HomeDocument.GetResource(LinkHelper.GetLinkRelationTypeName<ShowLink>()) as ShowLink;
                    break;
            }

            if (showLink != null)
            {
                await ProcessResponseAsync(_httpClient.SendAsync(showLink.CreateRequest()));
            }
        }

        private bool HasContent(HttpResponseMessage response)
        {
            return (response.Content != null && response.Content.Headers.ContentLength != 0);
        }

        private async Task ProcessActionLinkResponse(HttpResponseMessage response, Link contextLink)
        {
            if (!HasContent(response)) return;
 
            var contentStream = await response.Content.ReadAsStreamAsync();

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/vnd.collection+json":
                   CurrentCollection = ParseCollectionJson(contentStream);
                   break;
            }
        }

        private async Task ProcessShowLinkResponse(HttpResponseMessage response, Link contextLink)
        {
            if (!HasContent(response) && response.Content.Headers.ContentType != null) return;  // If we don't know the content-type, we can't show it
 
             var contentStream = await response.Content.ReadAsStreamAsync();

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/vnd.collection+json":
                    CurrentCollection = ParseCollectionJson(contentStream);
                    break;
                case "image/jpeg":
                    var bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(contentStream.AsRandomAccessStream());
                    CurrentImage = bitmap;
                    break;

                case "image/tiff":

                    break;
                case "application/pdf":
                    break;
            }
        }

        private static Collection ParseCollectionJson(Stream contentStream)
        {
            var jsons = JsonSerializer.Create();
            return jsons.Deserialize<ReadDocument>(new JsonTextReader(new StreamReader(contentStream))).Collection;
        }

        private bool IsClientError(System.Net.HttpStatusCode httpStatusCode)
            {
                return (int)httpStatusCode >= 400 && (int)httpStatusCode > 500 ;
            }
        private bool IsServerError(System.Net.HttpStatusCode httpStatusCode)
        {
            return (int) httpStatusCode >= 500;
        }
        private bool IsRedirect(System.Net.HttpStatusCode httpStatusCode)
        {
            return (int)httpStatusCode >= 300 && (int)httpStatusCode < 400;
        }


        private async Task ProcessRedirect(HttpResponseMessage response)
        {
            if ((int)response.StatusCode >= 300)
            {
                // Process redirect
                if (response.Content.Headers.ContentType.MediaType == "application/http-problem+json")
                {
                    // Parse content into error document
                    var problemdoc = ProblemDocument.Parse(await response.Content.ReadAsStreamAsync());
                    Problems.Add(problemdoc);
                }
            }
        }

        private bool ProcessClientError(HttpResponseMessage response, Link contextLink)
        {
            if ((int)response.StatusCode >= 400)
            {
                // Server claims we made a bad request
                // Don't change client state other than to record an error has occured
                // Create error instance, add it to the list of errors
                UserMessage = string.Format("{0} error returned while following {1} to {2}", response.ReasonPhrase,
                    contextLink.Relation, contextLink.Target.OriginalString);
                return true;
            }
            return false;
        }

        private void ProcessServerErrors(HttpResponseMessage response)
        {
            if ((int)response.StatusCode >= 500)
            {
                // Server failed to process request correctly.
                // Don't change client state other than to record an error has occured
                // Create error instance, add it to the list of errors
                // If we keep getting these, make sure the user can exit the app or select another server
                UserMessage = "Server failed to process request successfully - " + response.ReasonPhrase;
            }
        }

       
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
