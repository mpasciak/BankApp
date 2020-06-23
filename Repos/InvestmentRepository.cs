using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankApp.Repos
{
    public class InvestmentRepository
    {
        private readonly AppDbContext database;

        public InvestmentRepository(AppDbContext database)
        {
            this.database = database;
        }
        public ICollection<Investment> GetInvestmentsForId(int id)
        {
            return database.Investments.Where(investment => investment.OwnerUserId == id).ToList();
        }

        public bool AddInvestment(Investment investment)
        {
            database.Investments.Add(investment);
            return Save();

        }
        public bool RemoveInvestment(int id)
        {
            database.Investments.Remove(database.Investments.SingleOrDefault(investment => investment.Id == id));
            return Save();

        }
        public bool UpdateInvestment(Investment investment)
        {
            database.Investments.Update(investment);
            return Save();

        }
        public bool Save()
        {
            return database.SaveChanges() >= 0 ? true : false;
        }
    }
}
