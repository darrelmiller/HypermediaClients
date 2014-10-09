﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CollectionJson;
using ExpenseApprovalApp.Links;
using ExpenseApprovalAppLogic.Links;
using ExpenseApprovalAppLogic.Tools;
using Tavis;
using Tavis.Home;
using Link = Tavis.Link;

namespace ExpenseApprovalAppLogic
{

    public class ExpenseAppClientState : INotifyPropertyChanged
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

        public async Task FollowLinkAsync(Link link)
        {
            var request = link.BuildRequestMessage();
           await ApplyResponseAsync(_httpClient.SendAsync(request));

        }

        public async Task ApplyResponseAsync(Task<HttpResponseMessage> responseTask)
        {

            try
            {
                var response = await responseTask;

                var contextLink = response.RequestMessage.ExtractLink();

                if (response.StatusCode.IsServerError())
                {
                    HttpClientHelper.ProcessServerErrors(response, this);
                    return;
                }
                if (response.StatusCode.IsClientError())
                {
                    HttpClientHelper.ProcessClientError(response, this, contextLink);
                    return;
                }
                if (response.StatusCode.IsRedirect())
                {
                    await HttpClientHelper.ProcessRedirect(response, this, contextLink);
                    return;
                }

                if (response.Headers.Contains("Link"))
                {
                    var links = response.Headers.ParseLinkHeaders(
                        new Uri(response.RequestMessage.RequestUri.Authority), LinkFactory);
                    CurrentBackLink = links.FirstOrDefault(l => l.Title == "back");
                }

                switch (contextLink.Relation)
                {
                    case TavisLinkTypes.Show:
                        var showLink = (ShowLink) contextLink;
                        await showLink.ProcessShowLinkResponse(response, this);
                        break;
                    case TavisLinkTypes.Action:
                        var actionLink = (ActionLink) contextLink;
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

        public async Task BackAsync()
        {
            if (CurrentBackLink == null) return;
            FollowLinkAsync(CurrentBackLink);
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
