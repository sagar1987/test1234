using Lakeshore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lakeshore.Domain.SalesAccountRepository
{
    public interface ISalesAccountCommandRepository
    {
        Task Update(SalesAccount hpt, CancellationToken cancellationToken);

        Task Add(SalesAccount hpt, CancellationToken cancellationToken);
    }
}
