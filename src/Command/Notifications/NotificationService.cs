﻿using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.Apprenticeships.Command.Notifications
{
    public interface INotificationService
    {
        Task<CommitmentsApprenticeshipDetails> GetApprenticeship(GetCurrentPartyIdsResponse currentPartyIds);
        Task<GetCurrentPartyIdsResponse> GetCurrentPartyIds(Guid apprenticeshipKey);
        Task<IEnumerable<Recipient>> GetEmployerRecipients(long accountId);
        Task<IEnumerable<Recipient>> GetProviderRecipients(long providerId);
        Task<bool> Send(Recipient recipient, string templateId, Dictionary<string, string> tokens);
    }

    public class NotificationService : INotificationService
    {
        protected ILogger<NotificationService> _logger;
        private readonly IAccountsApiClient<AccountsConfiguration> _accountsApiClient;
        private readonly IApprenticeshipsApiClient<ApprenticeshipsApiConfiguration> _apprenticeshipsApiClient;
        private readonly ICommitmentsV2ApiClient<CommitmentsV2ApiConfiguration> _apiCommitmentsClient;
        private readonly IProviderAccountApiClient<ProviderAccountApiConfiguration> _providerAccountApiClient;
        private readonly IEncodingService _encodingService;
        private readonly IMessageSession _messageSession;

        public NotificationService(
            ILogger<NotificationService> logger,
            IAccountsApiClient<AccountsConfiguration> accountsApiClient,
            IApprenticeshipsApiClient<ApprenticeshipsApiConfiguration> apprenticeshipsApiClient,
            ICommitmentsV2ApiClient<CommitmentsV2ApiConfiguration> apiCommitmentsClient,
            IEncodingService encodingService,
            IProviderAccountApiClient<ProviderAccountApiConfiguration> providerAccountApiClient,
            IMessageSession messageSession)
        {
            _logger = logger;
            _accountsApiClient = accountsApiClient;
            _apprenticeshipsApiClient = apprenticeshipsApiClient;
            _apiCommitmentsClient = apiCommitmentsClient;
            _encodingService = encodingService;
            _providerAccountApiClient = providerAccountApiClient;
            _messageSession = messageSession;
        }

        /// <summary>
        /// Call to apprenticeship inner to get the apprenticeships current provider and employer ids
        /// </summary>
        public async Task<GetCurrentPartyIdsResponse> GetCurrentPartyIds(Guid apprenticeshipKey)
        {
            var partyIds = await _apprenticeshipsApiClient.Get<GetCurrentPartyIdsResponse>(new GetCurrentPartyIdsRequest { ApprenticeshipKey = apprenticeshipKey });
            if (partyIds == null)
            {
                throw new Exception($"No response returned from ApprenticeshipInnerApi for apprenticeship key {apprenticeshipKey} for the {nameof(GetCurrentPartyIdsRequest)}");
            }
            return partyIds;
        }

        public async Task<IEnumerable<Recipient>> GetEmployerRecipients(long accountId)
        {
            var response = await _accountsApiClient.Get<GetAccountTeamMembersWhichReceiveNotificationsResponse>(new GetAccountTeamMembersWhichReceiveNotificationsRequest(accountId));

            if (response == null || !response.Any())
            {
                _logger.LogWarning($"No response returned for the {nameof(GetAccountTeamMembersWhichReceiveNotificationsRequest)} for account id {accountId}");
                return Enumerable.Empty<Recipient>();
            }

            return response.Where(x => x.CanReceiveNotifications).Select(x => new Recipient
            {
                UserRef = x.UserRef,
                Email = x.Email
            });
        }

        public async Task<IEnumerable<Recipient>> GetProviderRecipients(long providerId)
        {
            var users = await _providerAccountApiClient.GetAll<GetProviderUsersListItem>(new GetProviderUsersRequest(providerId));

            var recipients = users.Any(u => !u.IsSuperUser) ?
                users.Where(x => !x.IsSuperUser).ToList() :
                users.ToList();

            var optedOutList = users.Where(x => !x.ReceiveNotifications).Select(x => x.EmailAddress).ToList();

            var finalRecipients = recipients
                .Where(x => !optedOutList.Exists(y => x.EmailAddress.Equals(y, StringComparison.CurrentCultureIgnoreCase)))
                .ToList();

            return finalRecipients.Select(x => new Recipient
            {
                Email = x.EmailAddress,
                UserRef = x.UserRef
            });
        }

        public async Task<CommitmentsApprenticeshipDetails> GetApprenticeship(GetCurrentPartyIdsResponse currentPartyIds)
        {
            _logger.LogInformation("Getting apprenticeship details for apprenticeship id {apprenticeshipId}", currentPartyIds.ApprovalsApprenticeshipId);

            var innerApiRequest = new GetApprenticeshipRequest(currentPartyIds.ApprovalsApprenticeshipId);
            var innerApiResponse = await _apiCommitmentsClient.GetWithResponseCode<GetApprenticeshipResponse>(innerApiRequest);


            if (!ApiResponseErrorChecking.IsSuccessStatusCode(innerApiResponse.StatusCode))
            {
                throw new Exception($"Apprenticeship not found for id {currentPartyIds.ApprovalsApprenticeshipId}");
            }

            return new CommitmentsApprenticeshipDetails
            {
                ProviderName = innerApiResponse.Body.ProviderName,
                EmployerName = innerApiResponse.Body.EmployerName,
                EmployerAccountHashedId = _encodingService.Encode(innerApiResponse.Body.EmployerAccountId, EncodingType.AccountId),
                ApprenticeFirstName = innerApiResponse.Body.FirstName,
                ApprenticeLastName = innerApiResponse.Body.LastName,
                ApprenticeshipHashedId = _encodingService.Encode(innerApiResponse.Body.Id, EncodingType.ApprenticeshipId)
            };
        }

        public async Task<bool> Send(Recipient recipient, string templateId, Dictionary<string, string> tokens)
        {
            try
            {
                var command = new SendEmailCommand(templateId, recipient.Email, tokens);
                await _messageSession.Send(command);
                _logger.LogInformation("Email sent to {userId}", recipient.UserRef);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {userId}", recipient.UserRef);
                return false;
            }

            return true;
        }
    }

    public class CommitmentsApprenticeshipDetails
    {
        public string ProviderName { get; set; } = string.Empty;
        public string EmployerName { get; set; } = string.Empty;
        public string EmployerAccountHashedId { get; set; } = string.Empty;
        public string ApprenticeFirstName { get; set; } = string.Empty;
        public string ApprenticeLastName { get; set; } = string.Empty;
        public string ApprenticeshipHashedId { get; set; } = string.Empty;
    }

    public class Recipient
    {
        public string UserRef { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
