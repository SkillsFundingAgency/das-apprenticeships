using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;
using SFA.DAS.Apprenticeships.Functions.Extensions;

namespace SFA.DAS.Apprenticeships.Functions;

public class WithdrawApprenticeshipFunction
{
    private readonly ICommandDispatcher _commandDispatcher;

    public WithdrawApprenticeshipFunction(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [FunctionName("WithdrawApprenticeship")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("WithdrawApprenticeship triggered");

        req.HttpContext.MarkAsBackOfficeRequest();

        WithdrawApprenticeshipCommand command;
        try
        {
            command = GetCommandFromRequest(req);
        }
        catch(Exception ex)
        {
            log.LogError(ex, "Failed to parse request body");
            return new BadRequestObjectResult("Invalid request body");
        }

        var withdrawResult = await _commandDispatcher.Send<WithdrawApprenticeshipCommand, WithdrawApprenticeshipResponse>(command);

        if(!withdrawResult.IsSuccess)
        {
            return new BadRequestObjectResult(withdrawResult.Message);
        }

        return new OkObjectResult("Completed");
    }

    private WithdrawApprenticeshipCommand GetCommandFromRequest(HttpRequest req)
    {
        using (var reader = new StreamReader(req.Body))
        {
            var body = reader.ReadToEnd();
            var command = JsonConvert.DeserializeObject<WithdrawApprenticeshipCommand>(body);
            return command;
        }
    }
}