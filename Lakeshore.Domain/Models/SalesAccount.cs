using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lakeshore.Domain.Models
{
    public class SalesAccount : Entity
    {
        public string? AccountId { get; set; }
        public string? AccountCategoryTerritory { get; set; }
        public string? Id { get; set; }
        public decimal? LakeshoreCustomerNumber { get; set; }
        public decimal? MtdSales { get; set; }
        public string? Name { get; set; }
        public string? OwnerId { get; set; }
        public decimal? PrevYearCurrMonth { get; set;}
        public decimal? PrevYearSales { get; set; }
        public decimal? PrevYearToDate { get; set; }
        public decimal? RollCurrYearSales { get; set; }
        public decimal? RollPrevYearSales { get; set; }
        public decimal? Territory { get; set; }
        public decimal? YtdSales { get; set; }
        public string? Error { get; set; }


        private SalesAccount(string? accountId, string? accountCategoryTerritory, string? id, decimal? lakeshoreCustomerNumber, decimal? mtdSales, string? name, string? ownerId, decimal? prevYearCurrMonth, decimal? prevYearSales, decimal? prevYearToDate, decimal? rollCurrYearSales, decimal? rollPrevYearSales, decimal? territory, decimal? ytdSales, string? error)
        {
            AccountId = accountId;
            AccountCategoryTerritory = accountCategoryTerritory;
            Id = id;
            LakeshoreCustomerNumber = lakeshoreCustomerNumber;
            MtdSales = mtdSales;
            Name = name;
            OwnerId = ownerId;
            PrevYearCurrMonth = prevYearCurrMonth;
            PrevYearSales = prevYearSales;
            PrevYearToDate = prevYearToDate;
            RollCurrYearSales = rollCurrYearSales;
            RollPrevYearSales = rollPrevYearSales;
            Territory = territory;
            YtdSales = ytdSales;
            Error = error;
        }

        public static SalesAccount CreateSalesAccount(string? AccountId, string? AccountCategoryTerritory, string? Id, decimal? LakeshoreCustomerNumber, decimal? MtdSales, string? Name, string? OwnerId, decimal? PrevYearCurrMonth, decimal? PrevYearSales, decimal? PrevYearToDate, decimal? RollCurrYearSales, decimal? RollPrevYearSales, decimal? Territory, decimal? YtdSales, string? Error)
        {
            // create new SalesAccount record
            var salesAccount = new SalesAccount(AccountId, AccountCategoryTerritory, Id, LakeshoreCustomerNumber, MtdSales, Name, OwnerId, PrevYearCurrMonth, PrevYearSales, PrevYearToDate, RollCurrYearSales, RollPrevYearSales, Territory, YtdSales, Error);

            // if we need to trigger some event when creating a new SalesAccount record add it here.

            return salesAccount;
        }

    }
}
