using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PatientSky.ProductCatalogue.ActorCharacter.MHealth.DigitalTherapeutics.None.HoleInOneEnterprises.RandomishNumberGenerator;
using PatientSky.ProductCatalogue.ActorCharacter.MHealth.DigitalTherapeutics.None.HoleInOneEnterprises.RandomishNumberGenerator_1_0.Functionality.Composition;
using PatientSky.ProductCatalogue.ActorCharacter.IotDevices.WearableAndWirelessDevices.None.HoleInOneEnterprises.ShoppingList2;
using ListFunctions_group_1_0 = PatientSky.ProductCatalogue.ActorCharacter.IotDevices.WearableAndWirelessDevices.None.HoleInOneEnterprises.ShoppingList2_1_0.Functionality.Embedded.ListFunctions_group_1_0;
using PatientSky.ProductCatalogue.Platform.Fundamentals;
using PatientSky.Core;
using PatientSky.Core.Models;
using Microsoft.WindowsAzure.Storage.Table;
using PatientSky.Core.Models.Features;
using PatientSky.Core.Session.Platform;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace HoleInOneEnterprises.ShoppingList2Dev
{
    public static class ShoppingList2_dev
    {
        private static readonly Dictionary<Guid,List<ListItem_1_0>> tenants = new(); 

        [FunctionName("ShoppingList2_dev")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] Microsoft.AspNetCore.Http.HttpRequest req,
            ILogger log)
        {
            // Parse our request
            var env = req.Query.ContainsKey("env") ? req.Query["env"].First<string>() : "dev";
            var body = IncomingRequest
                .From(await new StreamReader(req.Body).ReadToEndAsync());

            // Find our tenant
            if (!tenants.ContainsKey(body.TenantId))
                tenants.Add(body.TenantId, new());
            var listItems = tenants[body.TenantId];

            // Handle features
            switch (body.FeatureId) {
                case ListFunctions_group_1_0.NewListItem_1_0.FeatureId:
                    var itemData = body.To<NewItemRequest_1_0>(); 
                    var fruitResult = await (await GetPmpConnection(env))
                        .ForwardAsActorCharacter(body)
                        .Request(GetRandomFruit_1_0.With(new Empty()));
                    if (!fruitResult.Success) {
                        return new OkObjectResult(new Result(
                            new Error() {
                                Code = "ActorCharacter.IotDevices.WearableAndWirelessDevices.None.HoleInOneEnterprises.ShoppingList2.FruityError",
                                Details = new ErrorDetails() { TechnicalError = "Failed while trying to get random fruit" },
                                Parent = fruitResult.Error
                            },
                            System.Net.HttpStatusCode.FailedDependency
                        ));
                    }
                    var item = new ListItem_1_0 {
                        Id = listItems.Count > 0 ? listItems.Select(x => x.Id).Max() + 1 : 1,
                        Name = itemData.Name == "fruit" ? fruitResult.Value.FruitName : itemData.Name,
                        Picked = false
                    };
                    listItems.Add(item);
                    return new OkObjectResult(Result<ListItem_1_0>.Ok(item));
                
                case ListFunctions_group_1_0.GetAllItems_1_0.FeatureId:
                    return new OkObjectResult(Result<List<ListItem_1_0>>.Ok(listItems));
                
                case ListFunctions_group_1_0.SetAsPicked_1_0.FeatureId:
                    listItems
                        .Where(l => l.Id == body.To<ListItemId_1_0>().Id)
                        .ToList()
                        .ForEach(l => l.Picked = true);
                    return new OkObjectResult(Result<Empty>.Ok(new Empty()));
                
                case ListFunctions_group_1_0.SetAsUnpicked_1_0.FeatureId:
                    listItems
                        .Where(l => l.Id == body.To<ListItemId_1_0>().Id)
                        .ToList()
                        .ForEach(l => l.Picked = false);
                    return new OkObjectResult(Result<Empty>.Ok(new Empty()));

                case ListFunctions_group_1_0.RemoveAllItems_1_0.FeatureId:
                    listItems.Clear();
                    return new OkObjectResult(Result<Empty>.Ok(new Empty()));
                
                default:
                    return new OkObjectResult(Result<object>.NotFound(
                        "random.code.from.my.service", 
                        "It seems you have triggered something I didn't expect!! Got: " + featureId + " => " + requestBody));
            }
        }

        private static PatientSky.Core.Session.Platform.PlatformSession _session;
        private static async Task<PatientSky.Core.Session.Platform.PlatformSession> GetPmpConnection(string env) { 
            if (_session == null) {
                var url  = "";
                var username = "";
                var password = "";
                if (env == "dev") {
                    url  = "https://pmp-staging-feature-gateway-eu-instance-dev.patientsky.cloud";
                    username = "iXnv61qssby7Oi7";
                    password = "^HGgS92hwB%z7t#";
                }
                if (env == "qa") {
                    url  = "https://pmp-staging-feature-gateway-eu-instance-qa.patientsky.cloud";
                    username = "";
                    password = "";
                }
                if (env == "demo") {
                    url  = "https://pmp-staging-feature-gateway-eu-instance-demo.patientsky.cloud";
                    username = "";
                    password = "";
                }
                if (env == "staging") {
                    url  = "https://pmp-staging-feature-gateway-eu-instance-staging.patientsky.cloud";
                    username = "";
                    password = "";
                }
                if (env == "prod") {
                    url  = "https://pmp-staging-feature-gateway-eu-instance-prod.patientsky.cloud";
                    username = "";
                    password = "";
                }
                _session = await Platform.Environment(url)
                    .OnCredentialsRequest(() => new CredentialsInsecure(username, password))
                    .Connect();
            }
            return _session;
        }
    } 

    // Comments
    // 1. Solution id should be uuid in AsSolutionSession
    // 2. LegalCustomerTenant Should be string in assign method
    // 3. Should hav a forward call that also sets the transaction id. Something like
    //
    // _session.ForwardAsActorCharacter(body)
    //     * Acquires a solution session with the current solution id
    //     * Sets the legal customer tenant
    //     * Sets the current locale
    //     * Sets the transaction id
    //
    // _session.Forward(body)
    //     .AsActorCharacter()
    //     .Request(GetRandomFruit_1_0.With(new Empty()));

    public static class ForwardingSessionExtensions {
        public static PatientSky.Core.Session.Utilisation.Level.ActorCharacterSession ForwardAsActorCharacter(this PlatformSession session, IncomingRequest body) {
            return session.AsSolutionSession(body.SolutionId.ToString())
                .AsActorCharacter()
                .LegalCustomerTenant(body.LegalCustomerTenant.ToString())
                .WithLocale(body.Locale)
                .TransactionId(body.TransactionId);
        }
    }
}
