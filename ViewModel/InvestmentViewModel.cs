using BankApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BankApp.ViewModel
{
    public class InvestmentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get { return Investment.Id; }
            set { Investment.Id = value; RaisePropertyChanged(nameof(Id)); }
        }
        public int OwnerUserId
        {
            get { return Investment.OwnerUserId; }
            set { Investment.OwnerUserId = value; RaisePropertyChanged(nameof(OwnerUserId)); }
        }
        public decimal Invested
        {
            get { return Investment.Invested; }
            set { Investment.Invested = value; RaisePropertyChanged(nameof(Invested)); }
        }

        public Investment Investment { get; }
        public InvestmentViewModel(Investment investment)
        {
            Investment = investment;
        }


        private void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
