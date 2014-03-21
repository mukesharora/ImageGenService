using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using RFIDServiceConfig.Web.Models;
using RFIDServiceConfig.Web.Services;

namespace RFIDServiceConfig.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class RFIDReaderViewModel : NotificationObject, IConfirmNavigationRequest
    {
        private readonly RFIDServiceConfigDomainContext _domainContext;
        private readonly IRegionManager _regionManager;
        private readonly DelegateCommand _newItemCmd;
        private readonly DelegateCommand _editItemCmd;
        private readonly DelegateCommand _deleteItemCmd;
        private readonly ObservableCollection<Reader> _itemsCollection;

        public RFIDReaderViewModel(RFIDServiceConfigDomainContext context, IRegionManager regionManager)
        {
            this._regionManager = regionManager;
            this._domainContext = context;

            _newItemCmd = new DelegateCommand(OnNewItemCmd);
            _editItemCmd = new DelegateCommand(OnEditItemCmd);
            //_deleteItemCmd = new DelegateCommand(OnDeleteItemCmd);

            _itemsCollection = new ObservableCollection<Reader>();
            LoadData(); 
        }

        /// <summary>
        /// List of items
        /// </summary>
        public ICollection ItemsCollectionView
        {
            get { return _itemsCollection; }
        }

        private Reader _selectedItem;
        public Reader SelectedItem
        {
            get 
            {
                return _selectedItem;
            }

            set 
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                }
            }
        }

        /// <summary>
        /// Add item command
        /// </summary>
        public ICommand NewItemCommand
        {
            get { return _newItemCmd; }
        }

        /// <summary>
        /// Edit item command
        /// </summary>
        public ICommand EditItemCommand
        {
            get { return _editItemCmd; }
        }

        /// <summary>
        /// Delete item command
        /// </summary>
        public ICommand DeleteItemCommand
        {
            get { return _deleteItemCmd; }
        }

        /// <summary>
        /// Called when the NewItemCommand is executed
        /// </summary>
        private void OnNewItemCmd()
        {
            // if reader is selected then navigate to new reader view
            // otherwise, navigate to new antenna view            
            _regionManager.RequestNavigate(
                "MainContentRegion",
                new Uri("ReaderEditView", UriKind.Relative));
        }

        /// <summary>
        /// Called when EditItemCommand is executed
        /// </summary>
        private void OnEditItemCmd()
        {
            if (this.SelectedItem != null)
            {
                var builder = new StringBuilder();
                builder.Append("ReaderEditView");

                var query = new UriQuery { { "ID", this.SelectedItem.ID.ToString(CultureInfo.InvariantCulture) } };
                builder.Append(query);

                _regionManager.RequestNavigate("MainContentRegion", new Uri(builder.ToString(), UriKind.Relative));
            }
        }

        public void LoadData()
        {
            _domainContext.Load(_domainContext.GetReadersAndAtennasQuery(), operation =>
            {
                if (operation.HasError)
                {
                    operation.MarkErrorAsHandled();
                }
                else
                {
                    _itemsCollection.Clear();
                    _itemsCollection.AddRange(operation.Entities);
                }
            }, null);
        }

        #region Implementation of INavigationAware

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// Called to determine if this instance can handle the navigation request.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <returns>
        ///   <see langword="true" /> if this instance accepts the navigation request; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        /// <summary>
        /// Called when the implementer is being navigated away from.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// Determines whether this instance accepts being navigated away from.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <param name="continuationCallback">The callback to indicate when navigation can proceed.</param>
        /// <remarks>
        /// Implementors of this method do not need to invoke the callback before this method is completed,
        /// but they must ensure the callback is eventually invoked.
        /// </remarks>
        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            _domainContext.SubmitChanges(operation =>
            {
                if (operation.HasError)
                {
                    _domainContext.RejectChanges();
                }
                else
                {
                    continuationCallback(true);
                }
            }, null);
        }

        #endregion
    }
}
