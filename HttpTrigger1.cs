using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PatientSky.ProductCatalogue.ActorCharacter.IotDevices.WearableAndWirelessDevices.None.HoleInOneEnterprises.ShoppingList2;
using ListFunctions_group_1_0 = PatientSky.ProductCatalogue.ActorCharacter.IotDevices.WearableAndWirelessDevices.None.HoleInOneEnterprises.ShoppingList2_1_0.Functionality.Embedded.ListFunctions_group_1_0;
using PatientSky.ProductCatalogue.Platform.Fundamentals;
using PatientSky.Core.Models;
using System;
using PatientSky.Core;

namespace azure_function1
{
    public static class HttpTrigger1
    {
        private static readonly Dictionary<Guid,List<ListItem_1_0>> tenants = new();

        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] Microsoft.AspNetCore.Http.HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var body = IncomingRequest.From(requestBody);
            string featureId = body.FeatureId;
            Guid tenantId = body.TenantId;

            if (!tenants.ContainsKey(tenantId))
                tenants.Add(tenantId, new());
            var listItems = tenants[tenantId];
            if (
                ListFunctions_group_1_0.NewListItem_1_0.Empty().Id == featureId ||
                featureId == "o"
            ) {
                var reqest = JsonConvert.DeserializeObject<NewItemRequest_1_0>(body.RequestJson.ToString());
                var item = new ListItem_1_0 {
                    Id = listItems.Count > 0 ? listItems.Select(x => x.Id).Max() + 1 : 1,
                    Name = reqest.Name,
                    Picked = false
                };
                listItems.Add(item);
                return new OkObjectResult(Result<ListItem_1_0>.Ok(item));
            } else if (
                ListFunctions_group_1_0.GetAllItems_1_0.Empty().Id == featureId
            ) {
                return new OkObjectResult(Result<List<ListItem_1_0>>.Ok(listItems));
            } else if (
                ListFunctions_group_1_0.SetAsPicked_1_0.Empty().Id == featureId
            ) {
                var reqest = JsonConvert.DeserializeObject<ListItemId_1_0>(body.RequestJson.ToString());
                listItems
                    .Where(l => l.Id == reqest.Id)
                    .ToList()
                    .ForEach(l => l.Picked = true);
                return new OkObjectResult(Result<Empty>.Ok(new Empty()));
            } else if (
                ListFunctions_group_1_0.SetAsUnpicked_1_0.Empty().Id == featureId
            ) {
                var reqest = JsonConvert.DeserializeObject<ListItemId_1_0>(body.RequestJson.ToString());
                listItems
                    .Where(l => l.Id == reqest.Id)
                    .ToList()
                    .ForEach(l => l.Picked = false);
                return new OkObjectResult(Result<Empty>.Ok(new Empty()));
            } else if (
                ListFunctions_group_1_0.RemoveAllItems_1_0.Empty().Id == featureId
            ) {
                listItems.Clear();
                return new OkObjectResult(Result<Empty>.Ok(new Empty()));
            }

            return new OkObjectResult(Result<object>.NotFound(
                "random.code.from.my.service",
                "It seems you have triggered something I didn't expect!! " + requestBody));
        }

    }
}
