using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankApp.Repos
{
    public class BankAccountRepository
    {
        private readonly AppDbContext database;
        public BankAccountRepository(AppDbContext appContext)
        {
            database = appContext;
        }

        public BankAccount GetBankAccountById(int id)
        {
            return database.BankAccounts.SingleOrDefault(bankAccount => bankAccount.Id == id);
        }
        public BankAccount GetBankAccountByOwnerId(int id)
        {
            return database.BankAccounts.SingleOrDefault(bankAccount => bankAccount.OwnerUserId == id);
        }
        public BankAccount GetBankAccountByOwnerName(string name)
        {
            var user = database.Users.SingleOrDefault(user => user.Login == name);
            return database.BankAccounts.SingleOrDefault(bankAccount => bankAccount.OwnerUserId == user.Id);
        }

        public bool Save()
        {
            return database.SaveChanges() >= 0 ? true : false;
        }
        public bool UpdateBankAccount(BankAccount bankAccount)
        {
            database.BankAccounts.Update(bankAccount);
            return Save();
        }
        public bool AddBankAccount(BankAccount bankAccountToAdd)
        {
            database.BankAccounts.Add(bankAccountToAdd);
            return Save();
        }
    }
}
