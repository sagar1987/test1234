using Lakeshore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lakeshore.Infrastructure.EntityModelConfiguration
{
    public class SalesAccountEntityConfiguration : IEntityTypeConfiguration<Domain.Models.SalesAccount>
    {
        public void Configure(EntityTypeBuilder<Domain.Models.SalesAccount> entity)
        {
            // Set the table name and schema for the SalesAccount entity.
            entity.ToTable("Account_Sales__c_Upsert", "dbo");

            // Configure the mapping between SalesAccount properties and database columns.
            entity.Property(e => e.AccountId).HasColumnName("Account_ID__c");
            entity.Property(e => e.AccountCategoryTerritory).HasColumnName("Acct_Cat_Territory__c"); // not in json
            entity.Property(e => e.Id).HasColumnName("Id"); // not in json
            entity.Property(e => e.LakeshoreCustomerNumber).HasColumnName("Lakeshore_Customer_Number_del__c"); // not in json
            entity.Property(e => e.MtdSales).HasColumnName("MTD_Sales__c");
            entity.Property(e => e.Name).HasColumnName("Name");  // not in json
            entity.Property(e => e.OwnerId).HasColumnName("OwnerId");
            entity.Property(e => e.PrevYearCurrMonth).HasColumnName("Previous_Year_Current_Month__c");
            entity.Property(e => e.PrevYearSales).HasColumnName("Previous_Year_Sales__c");
            entity.Property(e => e.PrevYearToDate).HasColumnName("Previous_Year_To_Date__c");
            entity.Property(e => e.RollCurrYearSales).HasColumnName("Rolling_Current_Year_Sales__c");
            entity.Property(e => e.RollPrevYearSales).HasColumnName("Rolling_Previous_Year_Sales__c");
            entity.Property(e => e.Territory).HasColumnName("Territory__c");
            entity.Property(e => e.YtdSales).HasColumnName("YTD_Sales__c");
            entity.Property(e => e.Error).HasColumnName("Error"); // not in json
        }
    }
}
