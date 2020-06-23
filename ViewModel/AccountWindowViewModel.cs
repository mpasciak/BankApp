using BankApp.Models;
using BankApp.Repos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace BankApp.ViewModel
{
    public class AccountWindowViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext database;
        public TransferBroker broker;
        private readonly IServiceProvider serviceProvider;
        private decimal balance;
        private decimal atm;
        private readonly BankAccountRepository bankRepo;
        private readonly UserRepository userRepo;
        private InvestmentViewModel selectedInvestment;
        private LoanViewModel selectedLoan;
        private readonly InvestmentRepository investRepo;
        private readonly LoanRepository loanRepo;
        private Visibility adminTab;
        private int loanToTake;
        private int investmentToPut;
        private User currentUser;

        public event PropertyChangedEventHandler PropertyChanged;
        public int InvestmentToPut
        {
            get { return investmentToPut; }
            set { investmentToPut = value; RaisePropertyChanged(nameof(InvestmentToPut)); }
        }
        public int LoanToTake
        {
            get { return loanToTake; }
            set { loanToTake = value; RaisePropertyChanged(nameof(LoanToTake)); }
        }
        public LoanViewModel SelectedLoan
        {
            get { return selectedLoan; }
            set { selectedLoan = value; RaisePropertyChanged(nameof(Loans)); }
        }
        public ObservableCollection<LoanViewModel> Loans { get; set; }
        public InvestmentViewModel SelectedInvestment
        {
            get { return selectedInvestment; }
            set { selectedInvestment = value; RaisePropertyChanged(nameof(SelectedInvestment)); }
        }
        public User AdminSelectedUser { get; set; }
        public ObservableCollection<User> LoggedUsers { get; set; }
        public ObservableCollection<InvestmentViewModel> Investments { get; set; }
        public decimal Balance
        {
            get { return balance; }
            set { balance = value; RaisePropertyChanged(nameof(Balance)); }
        }

        public decimal Atm
        {
            get { return atm; }
            set { atm = value; RaisePropertyChanged(nameof(Atm)); }
        }
        public string TransferName { get; set; }
        public User CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; RaisePropertyChanged(nameof(CurrentUser)); RaisePropertyChanged(nameof(AdminTab)); Broker_Update(null, null); }
        }
        public BankAccount CurrentBankAccount { get; set; }
        public Visibility AdminTab
        {
            get
            {
                if (CurrentUser == null)
                    return Visibility.Collapsed;
                else
                    return CurrentUser.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public RelayCommand AddBalanceCommand { get; set; }
        public RelayCommand RemoveBalanceCommand { get; set; }
        public RelayCommand MoneyTransferCommand { get; set; }
        public RelayCommand AddLoanCommand { get; set; }
        public RelayCommand RemoveLoanCommand { get; set; }
        public RelayCommand AddInvestmentCommand { get; set; }
        public RelayCommand RemoveInvestmentCommand { get; set; }
        public RelayCommand KickUser { get; set; }
        public AccountWindowViewModel(IServiceProvider serviceProvider, TransferBroker broker, BankAccountRepository bankRepo, UserRepository userRepo, InvestmentRepository investRepo, LoanRepository loanRepo)
        {
            this.broker = broker;
            this.serviceProvider = serviceProvider;
            this.bankRepo = bankRepo;
            this.userRepo = userRepo;
            this.investRepo = investRepo;
            this.loanRepo = loanRepo;
            broker.TransferStarted += Broker_TransferStarted;
            broker.TimePassed += Broker_TimePassed;
            broker.Update += Broker_Update;
            Loans = new ObservableCollection<LoanViewModel>();
            Investments = new ObservableCollection<InvestmentViewModel>();
            LoggedUsers = new ObservableCollection<User>();
            InitializeCommands();
        }

        public void Broker_Update(object sender, EventArgs e)
        {
            var users = userRepo.GetAllLoggedUser().ToList();
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                LoggedUsers.Clear();
                users.ForEach(user => LoggedUsers.Add(user));
            });
        }

        private void Broker_TimePassed(object sender, EventArgs e)
        {
            UpdateLoans();
            UpdateInvestments();
        }

        private void UpdateLoans()
        {
            foreach (var loan in Loans)
            {
                loan.ToPay += loan.ToPay / 1000;
                loanRepo.UpdateLoan(loan.Loan);
            }
        }

        private void UpdateInvestments()
        {
            foreach (var invest in Investments)
            {
                invest.Invested += invest.Invested / 100;
                investRepo.UpdateInvestment(invest.Investment);
            }
        }

        private void Broker_TransferStarted(object sender, EventArgs e)
        {
            CurrentBankAccount = bankRepo.GetBankAccountByOwnerName(CurrentUser.Login);
            Balance = CurrentBankAccount.Deposit;
        }
        public void OnExit()
        {
            CurrentUser.IsLogged = false;
            userRepo.UpdateUser(CurrentUser);
            broker.UpdateLoggedUsers();
        }
        private void InitializeCommands()
        {
            AddBalanceCommand = new RelayCommand((x) =>
            {
                Balance += Atm;
                CurrentBankAccount.Deposit = Balance;
                bankRepo.UpdateBankAccount(CurrentBankAccount);
            });
            RemoveBalanceCommand = new RelayCommand((x) =>
            {
                if (Balance - Atm <= 0)
                {
                    MessageBox.Show("You don't enoguh money", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Balance -= Atm;
                CurrentBankAccount.Deposit = Balance;
                bankRepo.UpdateBankAccount(CurrentBankAccount);
            });
            MoneyTransferCommand = new RelayCommand((x) =>
            {
                if (!userRepo.DoesUserExist(TransferName))
                {
                    MessageBox.Show("There is no user with this login", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Balance - Atm <= 0)
                {
                    MessageBox.Show("You don't enoguh money", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Balance -= Atm;
                CurrentBankAccount.Deposit = Balance;
                bankRepo.UpdateBankAccount(CurrentBankAccount);

                var transferBankAccount = bankRepo.GetBankAccountByOwnerName(TransferName);
                transferBankAccount.Deposit += Atm;
                broker.OnSendMoneyTransfer();

            });
            AddInvestmentCommand = new RelayCommand((x) =>
            {
                if (InvestmentToPut > Balance)
                {
                    MessageBox.Show("You don't enoguh money", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var inv = new Investment()
                {
                    OwnerUserId = CurrentUser.Id,
                    Invested = InvestmentToPut
                };
                investRepo.AddInvestment(inv);
                Balance -= InvestmentToPut;
                CurrentBankAccount.Deposit = Balance;
                bankRepo.UpdateBankAccount(CurrentBankAccount);
                PopulateInvestments();

            });
            RemoveInvestmentCommand = new RelayCommand((x) =>
            {
                if (SelectedInvestment == null)
                    return;


                investRepo.RemoveInvestment(SelectedInvestment.Id);
                Balance += SelectedInvestment.Invested;
                CurrentBankAccount.Deposit = Balance;
                bankRepo.UpdateBankAccount(CurrentBankAccount);
                Investments.Remove(SelectedInvestment);
                SelectedInvestment = null;

            });
            AddLoanCommand = new RelayCommand((x) =>
            {
                if (LoanToTake > Balance * 2)
                {
                    MessageBox.Show("You cannot take loan with more than 2 times your current balance", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var loan = new Loan()
                {
                    OwnerUserId = CurrentUser.Id,
                    ToPay = LoanToTake
                };
                loanRepo.AddLoan(loan);
                Balance += LoanToTake;
                CurrentBankAccount.Deposit = Balance;
                bankRepo.UpdateBankAccount(CurrentBankAccount);
                PopulateLoans();

            });
            RemoveLoanCommand = new RelayCommand((x) =>
            {
                if (SelectedLoan == null)
                    return;
                if (SelectedLoan.ToPay > Balance)
                {
                    MessageBox.Show("You don't enoguh money", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Balance -= SelectedLoan.ToPay;
                CurrentBankAccount.Deposit = Balance;
                bankRepo.UpdateBankAccount(CurrentBankAccount);
                loanRepo.RemoveLoan(SelectedLoan.Id);
                Loans.Remove(SelectedLoan);
                SelectedLoan = null;
            });
            KickUser = new RelayCommand((x) =>
            {
                if (AdminSelectedUser == null)
                    return;

                broker.ForceLogout(AdminSelectedUser.Id);
                Broker_Update(null, null);
                AdminSelectedUser = null;
            });

        }

        public void PopulateLoans()
        {

            Loans.Clear();
            var loanss = loanRepo.GetLoansForId(CurrentUser.Id).ToList();
            loanss.ForEach(x => Loans.Add(new LoanViewModel(x)));
        }

        public void PopulateInvestments()
        {
            Investments.Clear();
            var inv = investRepo.GetInvestmentsForId(CurrentUser.Id).ToList();
            inv.ForEach(x => Investments.Add(new InvestmentViewModel(x)));
        }

        private void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

