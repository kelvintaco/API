﻿using APIFinal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace APIFinal.Context
{
    public class DataContext : DbContext
    {
        public DbSet<Custodian> Custodians { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<PAR> PAR { get; set; }
        public DbSet<ICS> ICS { get; set; }
        public DbSet<Transfer> Transfer { get; set; }
        public DbSet<ArchivedTransactions> ArchievedTransactions { get; set; }
        public DbSet<ItemDisposal> ItemDisposal { get; set; }
        public DbSet<PendingPAR> PendingPAR { get; set; }
        public DbSet<PendingICS> PendingICS { get; set; }
        public DbSet<PendingSur> PendingSur { get; set; }
        public DbSet<PendingTrans> PendingTrans { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        
    }
}