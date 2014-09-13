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
        private readonly LinkFactory _linkFactory = new LinkFactory();

        private BitmapImage _currentImage;
        private string _userMessage;
        private HomeDocument _homeDocument;
        private Collection _currentCollection;

        public HttpClient HttpClient
        {
            get { return _httpClient; }
        }

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

        public BitmapImage CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                OnPropertyChanged();
            }
        }

        private Stack<Link> _History = new Stack<Link>();

        public ClientState(HttpClient httpClient)
        {
            _httpClient = httpClient;
            LinkFactory.AddLinkType<HomeLink>();
            LinkFactory.AddLinkType<ShowLink>();
            LinkFactory.AddLinkType<ActionLink>();
            
        }

        public async Task FollowLinkAsync(Link link)
        {
           await ProcessResponseAsync(HttpClient.SendAsync(link.CreateRequest()));
           _History.Push(link); 
        }

        public async Task BackAsync()
        {
            _History.Pop();
            var link = _History.Peek();
            FollowLinkAsync(link);
        }

        public async Task ProcessResponseAsync(Task<HttpResponseMessage> responseTask)
        {

            try
            {
                var response = await responseTask;

                var contextLink = response.RequestMessage.ExtractLink();

                if (response.StatusCode.IsServerError()) { HttpClientHelper.ProcessServerErrors(response,this); return; }
                if (response.StatusCode.IsClientError()) { HttpClientHelper.ProcessClientError(response, this,contextLink); return; }
                if (response.StatusCode.IsRedirect()) { await HttpClientHelper.ProcessRedirect(response,this,contextLink); return; }


                switch (contextLink.Relation)
                {
                    case TavisLinkTypes.Show:
                        var showLink = (ShowLink) contextLink;
                        await showLink.ProcessShowLinkResponse(response, this);
                        break;
                    case TavisLinkTypes.Action:
                        var actionLink = (ActionLink)contextLink;
                        await actionLink.ProcessActionLinkResponse(response, this);
                        break;
                    case TavisLinkTypes.Home:
                        var homeLink = (HomeLink) contextLink;
                        await homeLink.ProcessHomeLinkResponse(response, this);
                        break;
                }

            }
            catch (Exception ex)
            {
                UserMessage = " Sorry, the client is having issues :" + ex.Message;
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
