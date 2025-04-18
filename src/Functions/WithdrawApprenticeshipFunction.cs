using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;
using SFA.DAS.Apprenticeships.Functions.Extensions;
using SFA.DAS.Apprenticeships.Domain;
using Microsoft.Azure.Functions.Worker;

namespace SFA.DAS.Apprenticeships.Functions;

public class WithdrawApprenticeshipFunction
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<WithdrawApprenticeshipFunction> _logger;

    private const string ServiceBearerTokenKey = "ServiceBearerToken";

    public WithdrawApprenticeshipFunction(
        ICommandDispatcher commandDispatcher,
        ILogger<WithdrawApprenticeshipFunction> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    [Function("WithdrawApprenticeship")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("WithdrawApprenticeship triggered");

        req.HttpContext.MarkAsBackOfficeRequest();

        WithdrawApprenticeshipCommand command;
        try
        {
            command = await GetCommandFromRequest(req);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to parse request body");
            return new BadRequestObjectResult("Invalid request body");
        }

        var withdrawResult = await _commandDispatcher.Send<WithdrawApprenticeshipCommand, Outcome>(command);

        if(!withdrawResult.IsSuccess)
        {
            return new BadRequestObjectResult(withdrawResult.GetResult<string>());
        }

        return new OkObjectResult("Completed");
    }

    private async Task<WithdrawApprenticeshipCommand> GetCommandFromRequest(HttpRequest req)
    {
        using (var reader = new StreamReader(req.Body))
        {
            var body = await reader.ReadToEndAsync();
            var command = JsonConvert.DeserializeObject<WithdrawApprenticeshipCommand>(body);
            command.ServiceBearerToken = req.Headers[ServiceBearerTokenKey];
            return command;
        }
    }
}
