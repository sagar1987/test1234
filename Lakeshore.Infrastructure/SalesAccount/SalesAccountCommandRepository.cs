using Lakeshore.Domain.SalesAccountRepository;
using Lakeshore.Infrastructure.EntityModelConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lakeshore.Infrastructure.SalesAccount
{
    public class SalesAccountCommandRepository : ISalesAccountCommandRepository
    {
        private readonly SalesAccountContext _context;

        public SalesAccountCommandRepository(SalesAccountContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task Add(Domain.Models.SalesAccount salesAccount, CancellationToken cancellationToken)
        {
            _context.SalesAccounts.AddAsync(salesAccount, cancellationToken);

            return Task.CompletedTask;
        }

        public Task Update(Domain.Models.SalesAccount salesAccount, CancellationToken cancellationToken)
        {
            var salesAccountNew = _context.SalesAccounts.Attach(salesAccount);
            salesAccountNew.State = EntityState.Modified;

            return Task.CompletedTask;
        }
    }
}
