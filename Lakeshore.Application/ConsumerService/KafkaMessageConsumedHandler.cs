using Lakeshore.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Lakeshore.Domain.Models;
//using Lakeshore.Dto.NotificationOfLabor;
using Lakeshore.Dto.AccountSales;
using Lakeshore.Domain.SalesAccountRepository;

namespace Lakeshore.Application.ConsumerService
{
    public class KafkaMessageConsumedHandler : INotificationHandler<KafkaMessageConsumedNotification>
    {
        private readonly ILogger<KafkaMessageConsumedHandler> _logger;
        private readonly ICommandUnitOfWork _unitWork;
        private readonly ISalesAccountCommandRepository _salesAccountCommandRepository;
        private readonly ISalesAccountQueryRepository _salesAccountQueryRepository;

        public KafkaMessageConsumedHandler(
            ILogger<KafkaMessageConsumedHandler> logger, 
            ICommandUnitOfWork commandUnitOfWork,
            ISalesAccountCommandRepository salesAccountCommandRepository,
            ISalesAccountQueryRepository salesAccountQueryRepository
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitWork = commandUnitOfWork ?? throw new ArgumentNullException(nameof(commandUnitOfWork));
            _salesAccountCommandRepository = salesAccountCommandRepository ?? throw new ArgumentNullException(nameof(salesAccountCommandRepository));
            _salesAccountQueryRepository = salesAccountQueryRepository ?? throw new ArgumentNullException(nameof(salesAccountQueryRepository));
        }

        public Task Handle(KafkaMessageConsumedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to process message");

            try
            {
                var newSalesAccount = JObject.Parse(notification.Message).ToObject<SalesAccountDto>();

                var salesAccountList = _salesAccountQueryRepository.GetAllSalesAccount(cancellationToken).Result.ToList();

                foreach (var account in newSalesAccount.CloudSalesTransactions)
                {

                    var existingSalesAccount = salesAccountList.FirstOrDefault(x => x.AccountId == account.AccountId);

                    if (existingSalesAccount == null)
                    {

                        // apply some transformation
                        SalesAccount salesAccount = SalesAccount.CreateSalesAccount(
                           account.AccountId,
                           "",
                           "",
                           null,
                           account.MtdSales,
                           "",
                           account.OwnerId,
                           account.PreviousYearCurrentMonth,
                           account.PreviousYearSales,
                           account.PreviousYearToDate,
                           account.RollingCurrentYearSales,
                           account.RollingPreviousYearSales,
                           account.Territory,
                           account.YtdSales,
                           ""
                            );

                        _salesAccountCommandRepository.Add(salesAccount, cancellationToken);
                    }
                    else
                    {
                        existingSalesAccount.AccountCategoryTerritory = "";
                        existingSalesAccount.Id = "";
                        existingSalesAccount.LakeshoreCustomerNumber = null;
                        existingSalesAccount.MtdSales = account.MtdSales;
                        existingSalesAccount.Name = "";
                        existingSalesAccount.OwnerId = account.OwnerId;
                        existingSalesAccount.PrevYearCurrMonth = account.PreviousYearCurrentMonth;
                        existingSalesAccount.PrevYearSales = account.PreviousYearSales;
                        existingSalesAccount.PrevYearToDate = account.PreviousYearToDate;
                        existingSalesAccount.RollCurrYearSales = account.RollingCurrentYearSales;
                        existingSalesAccount.RollPrevYearSales = account.RollingPreviousYearSales;
                        existingSalesAccount.Territory = account.Territory;
                        existingSalesAccount.YtdSales = account.YtdSales;

                        _salesAccountCommandRepository.Update(existingSalesAccount, cancellationToken);
                    }


                }
                // TODO : Uncomment save changes so changes actually happen.
                 _unitWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully processed and saved message in database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: ");
            }

            return Task.CompletedTask;
        }
    }
}
