using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CollectionJson;
using ExpenseApprovalAppLogic.Links;
using ExpenseApprovalAppLogic.Tools;
using Tavis;
using Tavis.Home;
using Link = Tavis.Link;

namespace ExpenseApprovalAppLogic
{

    public class ExpenseAppClientState : INotifyPropertyChanged, IResponseHandler
    {

        public LinkFactory LinkFactory
        {
            get { return _linkFactory; }
        }
 
        public string UserMessage
        {
            get { return _userMessage; }
             set
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
        public Collection CurrentCollection
        {
            get { return _currentCollection; }
            set
            {
                _currentCollection = value; 
                OnPropertyChanged();
            }
        }
        public Stream CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                OnPropertyChanged();
            }
        }
        public Stream CurrentFile
        {
            get { return _currentFile; }
            set
            {
                _currentFile = value;
                OnPropertyChanged();
            }
        }
        public Link CurrentBackLink { get; set; }

        public ExpenseAppClientState(HttpClient httpClient)
        {
            _httpClient = httpClient;
            LinkFactory.AddLinkType<HomeLink>();
            LinkFactory.AddLinkType<ShowLink>();
            LinkFactory.AddLinkType<ActionLink>();          
        }

        public async Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage response)
        {

            try
            {
        
                if (response.StatusCode.IsServerError())
                {
                    ProcessServerErrors(response, this,linkRelation);
                    return response;
                }
                if (response.StatusCode.IsClientError())
                {
                    ProcessClientError(response, this, linkRelation);
                    return response;
                }
                if (response.StatusCode.IsRedirect())
                {
                    await ProcessRedirect(response, this, linkRelation);
                    return response;
                }

                if (response.Headers.Contains("Link"))
                {
                    var links = response.Headers.ParseLinkHeaders(
                        new Uri(response.RequestMessage.RequestUri.Authority), LinkFactory);
                    CurrentBackLink = links.Cast<Link>().FirstOrDefault(l => l.Title == "back");
                }

                switch (linkRelation)
                {
                    case TavisLinkTypes.Show:
                        await ProcessShowLinkResponse(response);
                        break;
                    case TavisLinkTypes.Action:
                        await ProcessActionLinkResponse(response);
                        break;
                    case TavisLinkTypes.Home:
                        await ProcessHomeLinkResponse(response);
                        break;
                }

            }
            catch (Exception ex)
            {
                UserMessage = " Sorry, the client is having issues :" + ex.Message;
            }
            return response;
        }

        public async Task BackAsync()
        {
            if (CurrentBackLink == null) return;
            _httpClient.FollowLinkAsync(CurrentBackLink,this);
        }

        private async Task ProcessShowLinkResponse(HttpResponseMessage response)
        {
            if (!response.HasContent() && response.Content.Headers.ContentType != null) return;  // If we don't know the content-type, we can't show it


            var contentStream = await response.Content.ReadAsStreamAsync();

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/vnd.collection+json":
                    CurrentCollection = CollectionJsonHelper.ParseCollectionJson(contentStream);
                    break;
                case "image/jpeg":
                    CurrentImage = contentStream;
                    break;

                case "image/tiff":

                    break;
                case "application/pdf":
                    CurrentFile = contentStream;

                    break;
            }



        }

        private async Task ProcessHomeLinkResponse(HttpResponseMessage response)
        {
            if (!response.HasContent()) return;

            var contentStream = await response.Content.ReadAsStreamAsync();

            ShowLink showLink = null;  // Need to find a showlink to follow

            switch (response.Content.Headers.ContentType.MediaType)
            {
                // Currently only support application/home+json
                case "application/home+json":
                    HomeDocument = HomeDocument.Parse(contentStream, LinkFactory);
                    showLink = HomeDocument.GetResource(LinkHelper.GetLinkRelationTypeName<ShowLink>()) as ShowLink;
                    break;

                // Add more media types here as desired.  WADL, SWAGGER, API Blueprint, WSDL, etc
            }

            if (showLink != null)
            {
                await _httpClient.FollowLinkAsync(showLink,this);
            }
        }

        private async Task ProcessActionLinkResponse(HttpResponseMessage response)
        {
            if (!response.HasContent()) return;

            var contentStream = await response.Content.ReadAsStreamAsync();

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/vnd.collection+json":
                    CurrentCollection = CollectionJsonHelper.ParseCollectionJson(contentStream);
                    break;
            }
        }


        private static async Task ProcessRedirect(HttpResponseMessage response, ExpenseAppClientState clientState, string contextRelation)
        {
            if ((int)response.StatusCode >= 300)
            {
                // Process redirect
                // TODO 
            }
        }

        private static bool ProcessClientError(HttpResponseMessage response, ExpenseAppClientState clientState, string contextRelation)
        {
            if ((int)response.StatusCode >= 400)
            {
                // Server claims we made a bad request
                // Don't change client state other than to record an error has occured
                // Create error instance, add it to the list of errors
                clientState.UserMessage = String.Format("{0} error returned while following {1} to {2}", response.ReasonPhrase,
                    contextRelation, response.RequestMessage.RequestUri.OriginalString);
                return true;
            }
            return false;
        }

        private static void ProcessServerErrors(HttpResponseMessage response, ExpenseAppClientState clientState, string contextRelation)
        {
            if ((int)response.StatusCode >= 500)
            {
                // Server failed to process request correctly.
                // Don't change client state other than to record an error has occured
                // Create error instance, add it to the list of errors
                // If we keep getting these, make sure the user can exit the app or select another server
                clientState.UserMessage = "Server failed to process request successfully - " + response.ReasonPhrase;
            }
        }

        
        public Task<HttpResponseMessage> FollowLinkAsync(Link link)
        {
            return _httpClient.FollowLinkAsync(link, this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly HttpClient _httpClient;
        private readonly LinkFactory _linkFactory = new LinkFactory();

        private Stream _currentImage;
        private Stream _currentFile;
        private string _userMessage;
        private HomeDocument _homeDocument;
        private Collection _currentCollection;
    }
}
