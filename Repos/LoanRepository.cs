using BankApp.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankApp.Repos
{
    public class LoanRepository
    {
        private readonly AppDbContext database;

        public LoanRepository(AppDbContext database)
        {
            this.database = database;
        }

        public ICollection<Loan> GetLoansForId(int id)
        {
            return database.Loans.Where(loan => loan.OwnerUserId == id).ToList();
        }

        public int AddLoan(Loan loan)
        {
            var result = database.Loans.Add(loan);
            Save();
            return result.Entity.Id;

        }
        public bool RemoveLoan(int id)
        {
            database.Loans.Remove(database.Loans.SingleOrDefault(loan => loan.Id == id));
            return Save();

        }
        public bool UpdateLoan(Loan loan)
        {
            database.Loans.Update(loan);
            return Save();

        }
        public bool Save()
        {
            return database.SaveChanges() >= 0 ? true : false;
        }
    }
}
