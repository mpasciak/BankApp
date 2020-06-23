using BankApp.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankApp.Repos
{
    public class UserRepository
    {
        private readonly AppDbContext database;
        public UserRepository(AppDbContext appContext)
        {
            database = appContext;
        }

        public ICollection<User> GetAllLoggedUser()
        {
            return database.Users.Where(x => x.IsLogged && !x.IsAdmin).ToList();
        }
        public User GetUserById(int id)
        {
            return database.Users.SingleOrDefault(user => user.Id == id);
        }
        public User GetUserByName(string username)
        {
            return database.Users.SingleOrDefault(user => user.Login == username);
        }
        public bool DoesUserExist(string username)
        {
            var user = database.Users.SingleOrDefault(user => user.Login == username);
            if (user == null)
                return false;
            else
                return true;
        }
        public bool IsUserLoggedIn(string username)
        {
            var user = database.Users.SingleOrDefault(user => user.Login == username);
            if (user == null)
                return false;

            return user.IsLogged;
        }
        public bool Save()
        {
            return database.SaveChanges() >= 0 ? true : false;
        }
        public bool UpdateUser(User user)
        {
            database.Users.Update(user);
            return Save();
        }
        public bool AddUser(User userToadd)
        {
            database.Users.Add(userToadd);
            return Save();
        }
    }
}
