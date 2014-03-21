using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RFIDServiceConfig.ViewModels;

namespace RFIDServiceConfig.Views
{
    public partial class RFIDReaderView : UserControl
    {
        private readonly RFIDReaderViewModel _viewModel;

        public RFIDReaderView(RFIDReaderViewModel viewModel)
        {            
            InitializeComponent();
            this._viewModel = viewModel;            
        }
        public void PageLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = _viewModel;
            _viewModel.LoadData();
        }
    }
}
