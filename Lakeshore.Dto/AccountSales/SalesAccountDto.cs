using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lakeshore.Dto.AccountSales
{

    public class SalesAccountDto
    {
        public List<Account> CloudSalesTransactions { get; set; }
    }


    public class Account
    {
        public string? AccountId { get; set; } = string.Empty;
        public string? OwnerId { get; set; } = string.Empty;
        public decimal? Territory { get; set; }
        public decimal? MtdSales { get; set; }
        public decimal? YtdSales { get; set; }
        public decimal? PreviousYearCurrentMonth { get; set; }
        public decimal? PreviousYearToDate { get; set; }
        public decimal? PreviousYearSales { get; set; }
        public decimal? RollingCurrentYearSales { get; set; }
        public decimal? RollingPreviousYearSales { get; set; }
        public DateTime TargetDate { get; set; }
    }
}




