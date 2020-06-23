using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Timers;

namespace BankApp
{
    /// <summary>
    /// TransferBroker odpowiada za symulacje uplywu czasu oraz informowanie wszystkich zalogowanyc hisntacji o roznych zdarzeniach
    /// </summary>
    public class TransferBroker
    {
        Timer timer;
        public event EventHandler TransferStarted;
        public event EventHandler TimePassed;
        public event EventHandler Update;
        public event TransferBrokerEventHandler ForcedLogut;

        public delegate void TransferBrokerEventHandler(object? sender, TransferBrokerEventArgs e);
        public TransferBroker()
        {
            StartTimer();
        }
        /// <summary>
        /// Uruchamia zegar dla lokat i kredytow
        /// </summary>
        private void StartTimer()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 10000;
            timer.Start();

        }

        public void UpdateLoggedUsers()
        {
            Update?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Informuje o obliczeniu zmian dla lokat i kredytow
        /// </summary>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimePassed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Informuje o tym, ze byl wykonany przelew i trzeba odswiezyc konta
        /// </summary>
        public void OnSendMoneyTransfer() => TransferStarted?.Invoke(this, EventArgs.Empty);
        public void ForceLogout(int id) => ForcedLogut?.Invoke(this, new TransferBrokerEventArgs(id));
    }
    /// <summary>
    /// specjalen event args, ktore przenosi informacje ktory user ma byc wylogowany
    /// </summary>
    public class TransferBrokerEventArgs : EventArgs
    {
        public int Id { get; set; }
        public TransferBrokerEventArgs(int id)
        {
            Id = id;
        }
    }

}
