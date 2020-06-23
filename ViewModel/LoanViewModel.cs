using BankApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BankApp.ViewModel
{
    public class LoanViewModel : INotifyPropertyChanged
    {
        private int ownerUserId;
        private decimal toPay;

        public int Id
        {
            get { return Loan.Id; }
            set { Loan.Id = value; RaisePropertyChanged(nameof(Id)); }
        }
        public int OwnerUserId
        {
            get { return Loan.OwnerUserId; }
            set { Loan.OwnerUserId = value; RaisePropertyChanged(nameof(OwnerUserId)); }
        }
        public decimal ToPay
        {
            get { return Loan.ToPay; }
            set { Loan.ToPay = value; RaisePropertyChanged(nameof(ToPay)); }
        }
        public Loan Loan { get; }

        public LoanViewModel(Loan loan)
        {
            Loan = loan;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
