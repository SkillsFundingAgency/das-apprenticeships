using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Apprenticeships.Functions
{
    public class EarningsFunctions
    {
        [FunctionName("EarningsHttpTrigger")]
        public async Task<IActionResult> EarningsHttpTrigger(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GenerateEarnings")] HttpRequest request,
            [DurableClient] IDurableOrchestrationClient client)
        {
            var requestId = Guid.NewGuid();

            //string requestBody = await new StreamReader(request.Body).ReadToEndAsync();

            string instanceId = await client.StartNewAsync(nameof(EarningsOrchestrator), $"{requestId}");


            return client.CreateCheckStatusResponse(request, instanceId);
        }

        [FunctionName("EarningsOrchestrator")]
        public async Task<string> EarningsOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            //todo process earnings
            return context.InstanceId;
        }

        [FunctionName("Function1")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("Function1_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("Function1_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("Function1_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("Function1_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        //[FunctionName("Function1_HttpStart")]
        //public static async Task<HttpResponseMessage> HttpStart(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
        //    [DurableClient] IDurableOrchestrationClient starter,
        //    ILogger log)
        //{
        //    // Function input comes from the request content.
        //    string instanceId = await starter.StartNewAsync("Function1", null);

        //    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        //    return starter.CreateCheckStatusResponse(req, instanceId);
        //}

        [FunctionName("Function1_HttpStart")]
        public static async Task HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Function1", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}