using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CollectionJson;
using ExpenseApprovalApp;
using ExpenseApprovalApp.Links;
using ExpenseApprovalAppLogic.Links;
using ExpenseApprovalAppLogic.Tools;

namespace ExpenseApprovalAppLogic.ViewModels
{
    public class ListPageViewModel : INotifyPropertyChanged
    {
        private readonly ExpenseAppClientState _clientState;
        public ObservableCollection<object> Items { get; set; }
        public IDelegateCommand ApproveCommand { protected set; get; }
        public IDelegateCommand RejectCommand { protected set; get; }
        public IDelegateCommand DetailsCommand { protected set; get; }
        public IDelegateCommand ReceiptCommand { protected set; get; }

        public event PropertyChangedEventHandler PropertyChanged;
       
        public ListPageViewModel(ExpenseAppClientState clientState)
        {
            _clientState = clientState;

            Items = new ObservableCollection<Object>();
            RefreshItems();

            // create a DeleteCommand instance 
            this.ApproveCommand = new DelegateCommand((o)=> ExecuteActionCommand("Approve",o),(o)=> HasLink("Approve",o));
            this.RejectCommand = new DelegateCommand((o) => ExecuteActionCommand("Reject", o), (o) => HasLink("Reject", o));
            this.DetailsCommand = new DelegateCommand((o) => ExecuteShowCommand("Details", o), (o) => HasLink("Details", o));
            this.ReceiptCommand = new DelegateCommand((o) => ExecuteShowCommand("Receipt", o), (o) => HasLink("Receipt", o)); 
        }


        public void RefreshItems()
        {
            LoadItems(_clientState.CurrentCollection);  
        }

 
        async Task ExecuteActionCommand(string instance, object param) 
        { 
            dynamic griditem = (ExpandoObject)param;
            Dictionary<string,Tavis.Link> links = griditem.Links;
            var link = links[instance];

            await _clientState.FollowLinkAsync(link);

            await _clientState.FollowLinkAsync(new ShowLink() { Target = link.Context });
        }


        async Task ExecuteShowCommand(string instance, object param)
        {
            dynamic griditem = (ExpandoObject)param;
            Dictionary<string, Tavis.Link> links = griditem.Links;
            var link = links[instance];

            await _clientState.FollowLinkAsync(link);
        }

        private void LoadItems(Collection collection)
        {
            Items.Clear();
            foreach (var item in collection.Items)
             {
                 var eo = new ExpandoObject();
                 var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

                 foreach (var kvp in item.Data)
                 {
                     eoColl.Add(new KeyValuePair<string, object>(kvp.Name, kvp.Value));
                 }

                 var links = new Dictionary<string, Tavis.Link>();
                 foreach (var link in item.Links)
                 {
                     switch (link.Rel)
                     {
                         case "urn:tavis:show":
                             links.Add(link.Prompt, new ShowLink() {Target = link.Href, Context = collection.Href});
                             break;
                         case "urn:tavis:action":
                             links.Add(link.Prompt, new ActionLink() { Target = link.Href, Context = collection.Href });
                            break;
                     }
                 }
                 eoColl.Add(new KeyValuePair<string, object>("Links",links));
                 Items.Add(eo);
             }
            OnPropertyChanged("Items");
        }


        private bool HasLink(string instance, object param)
        {
            dynamic griditem = (ExpandoObject)param;
            if (griditem == null) return false;
            Dictionary<string, Tavis.Link> links = griditem.Links;
            return links.ContainsKey(instance);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
}
