using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zengo.WP8.FAS.Models
{
    
    public class LeagueAnswerResponses
    {
        [JsonProperty("leagues")]
        public List<LeagueAnswer> Leagues { get; set; }
    }

    public class LeagueAnswer
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("leaguename")]
        public string LeagueName { get; set; }
    }

    public class StadiumAnswerResponses
    {
        [JsonProperty("stadiums")]
        public List<StadiumAnswer> Stadiums { get; set; }
    }

    public class StadiumAnswer
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("stadiumname")]
        public string StadiumName { get; set; }
    }

    public class SportAnswerResponses
    {
        [JsonProperty("sports")]
        public List<SportAnswer> Sports { get; set; }
    }

    public class SportAnswer
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sportname")]
        public string SportName { get; set; }
    }
}
