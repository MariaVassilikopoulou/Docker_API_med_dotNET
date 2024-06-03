global using Microsoft.EntityFrameworkCore;
using API_med_dotNET.Migrations;
using API_med_dotNET.Models;
using Microsoft.Azure.Cosmos.Core.Collections;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
using System.Drawing;
using System;

namespace API_med_dotNET
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Products> Products { get; set; }

      

    }
}


