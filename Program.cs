using FlightsParsing;
using Json_structure;
using NewException;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        await GetAllFlight();

        Console.Write("Press any key to exit...");
        Console.ReadKey();
    }

    private static readonly HttpClient httpClient = new HttpClient();

    static async Task GetAllFlight()
    {
        var allFlights = await GetAllForwardFlight(1);
        Console.WriteLine($"Total Forward Flights Retrieved: {allFlights.Count}");

        var simpleFlight = allFlights[0];
        try
        {
            FlightInformation? flightInformation = null;

            int number = 0;
            Console.WriteLine($"N::Fl.Num===TimeSources Name::: flight Time::::::UTC");
            foreach (var flights in allFlights)
            {
                try
                {
                    number++;

                    flightInformation = flights.ToObject<FlightInformation>() ?? throw new FlighNumberIsMissingException();

                    string flightNumber = flightInformation?.Flight?.Identification?.Number?.identificationCode ?? "Missing";

                    var Times = flightInformation?.Flight?.Time;

                    var source = new (string Time, string? Value)[]
                    {
                        ("Real", Times?.Real?.time),
                        ("Estimated", Times?.Estimated?.time),
                        ("Scheduled", Times?.Scheduled?.time)
                    };

                    var timeSources = source.FirstOrDefault(s => s.Value != null);
                    if (timeSources.Value == null)
                        timeSources = ("Unknown", "0");

                    string UTC = flightInformation?.Flight?.Airport?.Origin?.TimeZone?.UTC ?? "0"; //Time is UTC+N

                    int unixTimeInUtcPlusN = int.Parse(timeSources.Value);

                    if (int.TryParse(UTC, out int utc)){}
                    else
                    {
                        utc = UTC switch
                        {
                            "WET" => 0,
                            "WEST" or "CET" => 1,
                            "CEST" or "EET" => 2,
                            "EEST" => 3,
                            "EDT" => -4,
                            "EST" => -5,
                            "AST" => -4,
                            "CST" => -6,
                            "MST" => -7,
                            "PST" => -8,
                            "AKST" => -9,
                            "HAST" => -10,
                            "SST" => -11,
                            "ChST" => 10,
                            "WAKT" => 12,
                            _ => throw new ArgumentException($"Unknown timezone code: {UTC}")
                        };
                    }

                    if (timeSources.Time != "0")
                    {
                        DateTime flightTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeInUtcPlusN).ToOffset(TimeSpan.FromHours(utc)).DateTime;
                        Console.WriteLine($"{number}::{flightNumber}====={timeSources.Time}:::{flightTime}::::{UTC} UTC");
                    }
                    else Console.WriteLine($"{number}::{flightNumber}====={timeSources.Time}::::{UTC} UTC");
                }
                catch (FlighNumberIsMissingException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static async Task<List<JObject>> GetAllForwardFlight(int startPage)
    {
        var allForwardDepartures = new List<JObject>();
        var allBackwardDepartures = new List<JObject>();

        int current = startPage;
        int total = 1;
        bool positivePhase = true;
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        while (true)
        {
            int pageForUrl = positivePhase ? current : -current;

            string url = $"https://api.flightradar24.com/common/v1/airport.json?code=evn&plugin[]=&plugin-setting[schedule][mode]=departures&plugin-setting[schedule][timestamp]={timestamp}&page={pageForUrl}&limit=100&fleet=&token=";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);

                        double secondsToWait = retryAfter.TotalSeconds;

                        Console.ForegroundColor = ConsoleColor.DarkCyan;

                        Console.WriteLine($"Parsing continues please wait, It will take {secondsToWait} seconds");
                        for (double i = secondsToWait, seconds = 0; i > 0; i--)
                        {
                            Console.Write($"\r{seconds} seconds out of {secondsToWait} have passed");
                            await Task.Delay(1000);
                            seconds++;
                        }
                        Console.WriteLine();
                        Console.ResetColor();

                        continue;
                    }

                    string json = await response.Content.ReadAsStringAsync();

                    json_structure json_structure = JsonConvert.DeserializeObject<json_structure>(json) ?? throw new Exception(("Failed to deserialize JSON."));

                    int curPage = json_structure?.Result?.Response?.Airport?.PluginData?.Schedule?.Departures?.Page?.current ?? 0;
                    int totPage = json_structure?.Result?.Response?.Airport?.PluginData?.Schedule?.Departures?.Page?.total ?? 0;

                    total = totPage;

                    var departuresData = json_structure?.Result?.Response?.Airport?.PluginData?.Schedule?.Departures?.data ?? new List<JObject>();

                    if (departuresData.Count <= 0)
                    {
                        Console.WriteLine("Departures data is missing in the JSON.");
                    }
                    else
                    {
                        Console.WriteLine($"Page {curPage} of {totPage} — Departures: {departuresData.Count}");
                        if (positivePhase)
                        {
                            //departuresData.Reverse();
                            allForwardDepartures.AddRange(departuresData);
                        }
                        else
                        {
                            allBackwardDepartures.AddRange(departuresData);
                        }
                    }
                    if (positivePhase)
                    {
                        if (current >= total)
                        {
                            positivePhase = false;
                            current = 1;
                        }
                        else
                        {
                            current++;
                        }
                    }
                    else
                    {
                        if(current >= total)
                        {
                            break;
                        }
                        else
                        {
                            current++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] {ex.Message}");
                    break;
                }
            }
        }

        allForwardDepartures.Reverse();
        return allForwardDepartures.Concat(allBackwardDepartures).ToList();
    }
}
