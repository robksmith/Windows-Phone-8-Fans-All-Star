using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    
    public class LeagueResponse
    {
        [JsonProperty("leagues")]
        public List<League> Leagues { get; set; }
    }

    public class League
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("leaguename")]
        public string LeagueName { get; set; }
    }

    public class StadiumResponse
    {
        [JsonProperty("stadiums")]
        public List<Stadium> Stadiums { get; set; }
    }

    public class Stadium
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("stadiumname")]
        public string StadiumName { get; set; }
    }

    public class SportsResponse
    {
        [JsonProperty("sports")]
        public List<Sport> Sports { get; set; }
    }

    public class Sport
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sportname")]
        public string SportName { get; set; }
    }
}
