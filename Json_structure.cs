using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Json_structure
{
    //Level 7
    public class Page
    {
        [JsonProperty("current")]
        public int current { get; set; }
        [JsonProperty("total")]
        public int total { get; set; }

    }
    //Level 6
    public class Departures
    {
        [JsonProperty("page")]
        public Page? Page { get; set; }
        [JsonProperty("data")]
        public List<JObject>? data { get; set; }
    }
    //Level 5
    public class Schedule
    {
        [JsonProperty("departures")]
        public Departures? Departures { get; set; }
    }
    //Level 4
    public class PluginData
    {
        [JsonProperty("schedule")]
        public Schedule? Schedule { get; set; }
    }
    //Level 3
    public class Airport
    {
        [JsonProperty("pluginData")]
        public PluginData? PluginData { get; set; }
    }
    //Level 2
    public class Response 
    {
        [JsonProperty("airport")]
        public Airport? Airport { get; set; }
    }
    //Level 1
    public class Result
    {
        [JsonProperty("response")]
        public Response? Response{ get; set; }
    }
    //Level 0
    public class json_structure
    {
        [JsonProperty("result")]
        public Result? Result { get; set; }
    }
}

namespace FlightsParsing
{
    //Level 4
    public class TimeZone
    {
        [JsonProperty("abbr")]
        public string? UTC { get; set; }
    }

    //Level 3
    public class Number
    {
        [JsonProperty("default")]
        public string? identificationCode { get; set; }
    }

    public class ScheduleTime
    {
        [JsonProperty("departure")]
        public string? time { get; set; }
    }

    public class  Origin
    {
        [JsonProperty("timezone")]

        public TimeZone? TimeZone { get; set; }
    }

    //Level 2
    public class Identification
    {
        [JsonProperty("number")]
        public Number? Number { get; set; }
    }
    public class Time
    {
        [JsonProperty("real")]

        public ScheduleTime? Real { get; set; }

        [JsonProperty("estimated")]
        public ScheduleTime? Estimated { get; set; }
        [JsonProperty("scheduled")]
        public ScheduleTime? Scheduled { get; set; }
    }
    public class Airport
    {
        [JsonProperty("origin")]
        public Origin? Origin { get; set; }
    }

    //Level 1
    public class Flight
    {
        [JsonProperty("identification")]
        public Identification? Identification { get; set; } 

        [JsonProperty("time")]
        public Time? Time { get; set; }
        [JsonProperty("airport")]
        public Airport? Airport { get; set; }

}
    //Level 0
    public class FlightInformation
    {
        [JsonProperty("flight")]
        public Flight? Flight { get; set; } 
    }
}