using BankApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BankApp.Views
{
    /// <summary>
    /// Interaction logic for AccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        private readonly TransferBroker broker;

        public AccountWindow(IServiceProvider serviceProvider, TransferBroker broker)
        {
            DataContext = serviceProvider.GetRequiredService<AccountWindowViewModel>();
            InitializeComponent();
            this.broker = broker;
            this.broker.ForcedLogut += Broker_ForcedLogut;
        }

        private void Broker_ForcedLogut(object sender, TransferBrokerEventArgs e)
        {
            var vm = (DataContext as AccountWindowViewModel);
            if (vm.CurrentUser.Id == e.Id)
                Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            var vm = (DataContext as AccountWindowViewModel);
            vm.OnExit();
            base.OnClosed(e);
        }
    }
}
