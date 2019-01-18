using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace EuRates
{
    public class Program
    {
        private static string url = "http://jsonvat.com";
        public static async Task<string> RetrieveRates ()
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await client.SendAsync(request, new CancellationToken()))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }
        public static void Main(string[] args)
        {
            ProvideHighestLowestRates().GetAwaiter().GetResult();
        }

        public static async Task ProvideHighestLowestRates()
        {
            //sample check string textJSON = File.ReadAllText(@"rates.json");
            string textJSON = await RetrieveRates();

            var vatData = JsonConvert.DeserializeObject<VatData>(textJSON);

            if (vatData != null)
            {
                var euRates = vatData.Rates;

                // could be set by user
                DateTime today = DateTime.Now;
                Dictionary<string, double> standardRates = new Dictionary<string, double>();

                foreach (var euCountry in euRates)
                {
                    var latestDate = new DateTime();
                    var standardCountryRate = 0.0;

                    foreach (var period in euCountry.Periods)
                    {
                        //choose most recent and valid rate
                        if (period.EffectiveFrom.CompareTo(latestDate) >= 0 && period.EffectiveFrom.CompareTo(today) <= -1)
                        {
                            //change to more safe
                            standardCountryRate = period.Rates["standard"];
                            latestDate = period.EffectiveFrom;
                        }
                    }
                    standardRates.Add(euCountry.Name, standardCountryRate);
                }

                var result = standardRates.OrderByDescending(x => x.Value);
                int size = result.Count();

                if (size > 6)
                {
                    Console.WriteLine("Highest rates: ");
                    for (var i = 0; i < 3; i++)
                    {
                        Console.WriteLine(result.ElementAt(i).ToString());
                    }
                    Console.WriteLine("Lowest rates: ");
                    for (var i = 1; i <= 3; i++)
                    {
                        Console.WriteLine(result.ElementAt(size - i).ToString());
                    }
                }
                else
                {
                    //print all
                }

                Console.ReadLine();
            }
        }
    }
}
