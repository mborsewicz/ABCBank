using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Common.Enums;

namespace Infrastructure.Contexts
{

    internal class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder
                .ToTable("Accounts", "Banking")
                .HasIndex(a => a.AccountNumber)
                .IsUnique()
                .HasDatabaseName("IX_Accounts_AccountNumber");

            builder
                .Property(a => a.Type)
                .HasConversion(new EnumToStringConverter<AccountType>());
        }
    }

    internal class AccountHolderCongfig : IEntityTypeConfiguration<AccountHolder>
    {
        public void Configure(EntityTypeBuilder<AccountHolder> builder)
        {
            builder
                .ToTable("AccountHolders", "Banking");
                
        }
    }

    internal class TransactionConfig : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder
                .ToTable("Transactions", "Banking")
                .Property(t => t.Type)
                .HasConversion(new EnumToStringConverter<TransactionType>());
        }
    }

    internal class BankDbConfigurations
    {

    }
}
