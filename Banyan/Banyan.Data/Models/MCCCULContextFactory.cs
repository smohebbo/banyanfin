
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Banyan.Data.Models;

namespace Banyan.Data.Models
{
    public class MCCCULContextFactory : IDesignTimeDbContextFactory<MCCCULContext>
    {
        public MCCCULContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MCCCULContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=Northwind;Trusted_Connection=True;");
            return new MCCCULContext(optionsBuilder.Options);
        }
    }
}