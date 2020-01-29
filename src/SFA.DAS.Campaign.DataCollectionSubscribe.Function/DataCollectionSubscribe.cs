using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Campaign.Functions.Domain.DataCollection;
using SFA.DAS.Campaign.Functions.Framework;
using SFA.DAS.Campaign.Functions.Framework.Attributes;
using SFA.DAS.Campaign.Functions.Models.DataCollection;

namespace SFA.DAS.Campaign.Functions.DataCollection
{
    public static class DataCollectionSubscribe
    {
        [FunctionName("DataCollectionSubscribe")]
        public static async Task Run([QueueTrigger(QueueNames.DataCollectionSubscribe)]string message, ILogger log, [Inject]IRegisterHandler handler)
        {
            try
            {
                var userData = JsonConvert.DeserializeObject<UserData>(message);

                await handler.Handle(userData);

                log.LogTrace($"C# Queue trigger function processed message: {message}");
            }
            catch (Exception e)
            {
                
                log.LogError(e, "Error in Function.  Message: {0}  Stack: {1}", e.Message, e.StackTrace);
                
                throw;
            }
        }
    }
}
