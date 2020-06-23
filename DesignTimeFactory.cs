using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BankApp
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            var builder = new DbContextOptionsBuilder<AppDbContext>();

            var connectionString = "Server = localhost,1433; Initial Catalog = bank; User ID = sa; Password = Passwordxd!23";

            builder.UseSqlServer(connectionString);

            return new AppDbContext(builder.Options);
        }
    }
}
