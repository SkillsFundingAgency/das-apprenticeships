using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.Command.WithdrawLearning;
using SFA.DAS.Learning.Domain;
using SFA.DAS.Learning.Functions.Extensions;

namespace SFA.DAS.Learning.Functions;

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

        WithdrawLearningCommand command;
        try
        {
            command = await GetCommandFromRequest(req);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to parse request body");
            return new BadRequestObjectResult("Invalid request body");
        }

        var withdrawResult = await _commandDispatcher.Send<WithdrawLearningCommand, Outcome>(command);

        if(!withdrawResult.IsSuccess)
        {
            return new BadRequestObjectResult(withdrawResult.GetResult<string>());
        }

        return new OkObjectResult("Completed");
    }

    private async Task<WithdrawLearningCommand> GetCommandFromRequest(HttpRequest req)
    {
        using (var reader = new StreamReader(req.Body))
        {
            var body = await reader.ReadToEndAsync();
            var command = JsonConvert.DeserializeObject<WithdrawLearningCommand>(body);
            command.ServiceBearerToken = req.Headers[ServiceBearerTokenKey];
            return command;
        }
    }
}
