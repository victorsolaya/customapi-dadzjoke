using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using VictorSanchez_CustomAPI;

namespace CustomAPIUtilities
{
    public class CreateDadzJoke : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            tracingService.Trace($"vso_CreateDadzJoke - {context.MessageName} - {context.Stage}");
            if (context.MessageName.Equals("vso_CreateDadzJoke") && context.Stage.Equals(30))
            {

                try
                {
                    string searchTerm = (string)context.InputParameters["vso_searchterm"];
                    var joke = GetJokeAsync(service, searchTerm);
                    if (joke != null)
                    {
                        context.OutputParameters["vso_joke"] = joke;
                    }
                    else
                    {
                        context.OutputParameters["vso_joke"] = "There was no joke possible to retrieve";
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in vso_CreateDadzJoke.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("vso_CreateDadzJoke: {0}", ex.ToString());
                    throw;
                }
            }
            else
            {
                tracingService.Trace("vso_CreateDadzJoke plug-in is not associated with the expected message or is not registered for the main operation.");
            }
        }

        public string GetJokeAsync(IOrganizationService service, string searchTerm)
        {
            string URL = "https://icanhazdadjoke.com/search";
            string parameterSearch = $"?term={searchTerm}";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(parameterSearch).Result;
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var jokesObject = response.Content.ReadAsStringAsync().Result;
                DadzJokeResponse jokes = DeserializeJson.DeserializeJSON(jokesObject);
                var totalJokes = jokes.total_jokes;
                var randomObject = new Random();
                var getNumberRandomBetween0AndTotalJokes = randomObject.Next(0, totalJokes);
                var jokeSelected = jokes.results[getNumberRandomBetween0AndTotalJokes].joke;
                return jokeSelected;
            }
            else
            {
                return null;
            }
        }
    }
}
