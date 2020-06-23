using BankApp.Models;
using BankApp.Repos;
using BankApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace BankApp.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly UserRepository userRepo;
        private Visibility loginWindowsVisibility;
        private Visibility registereWindowVisibility;
        private string loginTextBox;
        private string passwordTextBox;
        private string registerLoginTextBox;
        private string registerPasswordBox;
        private bool registerIsAdmin;
        private readonly TransferBroker broker;
        private readonly IServiceProvider serviceProvider;
        private readonly BankAccountRepository bankRepo;
        private string registerUserName;

        public event PropertyChangedEventHandler PropertyChanged;
        public string LoginTextBox
        {
            get { return loginTextBox; }
            set { loginTextBox = value; RaisePropertyChanged(nameof(LoginTextBox)); }
        }
        public string RegisterUserName
        {
            get { return registerUserName; }
            set { registerUserName = value; RaisePropertyChanged(nameof(RegisterUserName)); }
        }
        public string PasswordTextBox
        {
            get { return passwordTextBox; }
            set { passwordTextBox = value; RaisePropertyChanged(nameof(PasswordTextBox)); }
        }

        public string RegisterLoginTextBox
        {
            get { return registerLoginTextBox; }
            set { registerLoginTextBox = value; RaisePropertyChanged(nameof(RegisterLoginTextBox)); }
        }
        public string RegisterPasswordBox
        {
            get { return registerPasswordBox; }
            set { registerPasswordBox = value; RaisePropertyChanged(nameof(RegisterPasswordBox)); }
        }
        public bool RegisterIsAdmin
        {
            get { return registerIsAdmin; }
            set { registerIsAdmin = value; RaisePropertyChanged(nameof(RegisterIsAdmin)); }
        }
        public Visibility LoginWindowVisibility
        {
            get { return loginWindowsVisibility; }
            set { loginWindowsVisibility = value; RaisePropertyChanged(nameof(LoginWindowVisibility)); }
        }
        public Visibility RegisterWindowVisibility
        {
            get { return registereWindowVisibility; }
            set { registereWindowVisibility = value; RaisePropertyChanged(nameof(RegisterWindowVisibility)); }
        }
        public MainWindowViewModel(UserRepository userRepo,BankAccountRepository bankRepo, TransferBroker broker, IServiceProvider serviceProvider)
        {
            InitCommands();
            this.userRepo = userRepo;
            RegisterWindowVisibility = Visibility.Collapsed;
            LoginWindowVisibility = Visibility.Visible;
            this.broker = broker;
            this.serviceProvider = serviceProvider;
            this.bankRepo = bankRepo;
        }

        private void InitCommands()
        {
            LoginCommand = new RelayCommand((x) =>
            {
                if (!userRepo.DoesUserExist(LoginTextBox))
                {
                    MessageBox.Show("There is no user with this login","Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = userRepo.GetUserByName(LoginTextBox);

                if (user.Password != PasswordTextBox)
                {
                    MessageBox.Show("Wrong password", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (user.IsLogged)
                {
                    MessageBox.Show("User is online", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                user.IsLogged = true;
                userRepo.UpdateUser(user);
                var bankAccount = bankRepo.GetBankAccountByOwnerId(user.Id);
                var window = serviceProvider.GetRequiredService<AccountWindow>();
                var vm = window.DataContext as AccountWindowViewModel;
                vm.CurrentUser = user;
                vm.CurrentBankAccount = bankAccount;
                vm.Balance = vm.CurrentBankAccount.Deposit;
                vm.PopulateInvestments();
                vm.PopulateLoans();
                vm.broker.UpdateLoggedUsers();
                window.Show();

            });
            RegisterCommand = new RelayCommand((x) =>
            {
                RegisterWindowVisibility = Visibility.Visible;
                LoginWindowVisibility = Visibility.Collapsed;

            });
            RegisterButtonCommand = new RelayCommand((x) =>
            {
                if (userRepo.DoesUserExist(RegisterLoginTextBox))
                {
                    MessageBox.Show("User already registered", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var user = new User()
                {
                    Login = RegisterLoginTextBox,
                    Password = RegisterPasswordBox,
                    IsAdmin = RegisterIsAdmin,
                    Name = RegisterUserName

                };

                userRepo.AddUser(user);
                var userId = userRepo.GetUserByName(user.Login).Id;

                var bankAccount = new BankAccount()
                {
                    Deposit = 1000,
                    OwnerUserId = userId
                };
                bankRepo.AddBankAccount(bankAccount);
                CleanRegisterePage();
                RegisterWindowVisibility = Visibility.Collapsed;
                LoginWindowVisibility = Visibility.Visible;
            });
            CancelButtonCommand = new RelayCommand((x) =>
            {
                RegisterWindowVisibility = Visibility.Collapsed;
                LoginWindowVisibility = Visibility.Visible;
                CleanRegisterePage();
            });

        }

        private void CleanRegisterePage()
        {
            RegisterUserName = RegisterLoginTextBox = RegisterPasswordBox = string.Empty;
            RegisterIsAdmin = false;
        }

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand RegisterButtonCommand { get; set; }
        public RelayCommand CancelButtonCommand { get; set; }

        private void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
