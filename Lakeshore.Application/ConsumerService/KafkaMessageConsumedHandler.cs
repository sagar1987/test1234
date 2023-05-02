using Amazon.Runtime.Internal.Util;
using Lakeshore.Domain;
using Lakeshore.Kafka.Client.Interfaces;
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

        public KafkaMessageConsumedHandler(
            ILogger<KafkaMessageConsumedHandler> logger, 
            ICommandUnitOfWork commandUnitOfWork,
            ISalesAccountCommandRepository salesAccountCommandRepository
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitWork = commandUnitOfWork ?? throw new ArgumentNullException(nameof(commandUnitOfWork));
            _salesAccountCommandRepository = salesAccountCommandRepository ?? throw new ArgumentNullException(nameof(salesAccountCommandRepository));
        }

        public Task Handle(KafkaMessageConsumedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to process message");

            try
            {
                var newSalesAccount = JObject.Parse(notification.Message).ToObject<SalesAccountDto>();


                foreach (var account in newSalesAccount.CloudSalesTransactions)
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
