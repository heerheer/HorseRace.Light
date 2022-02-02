using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Amiable.Core
{
    public class RaceApiModels
    {
        public class RaceCreateInfo
        {
            public string raceId { get; set; }

            public Data data { get; set; }

            public class Data
            {
                public DateTime createTime { get; set; }
                public bool started { get; set; }
                public int distance { get; set; }
                public List<object> horses { get; set; }
                public List<object> groundBuffs { get; set; }
            }
        }


        public class RaceStartInfo
        {
            [JsonPropertyName("data")] public StartInfoData Data { get; set; }

            [JsonPropertyName("texts")] public List<string> Texts { get; set; }

            public class StartInfoData
            {
                [JsonPropertyName("horses")] public List<Hors> Horses { get; set; }
            }
        }

        public class RaceNextInfo
        {
            [JsonPropertyName("end")] public bool End { get; set; }

            [JsonPropertyName("logs")] public List<string> Logs { get; set; }

            [JsonPropertyName("race")] public RaceInfo Race { get; set; }

            public class RaceInfo
            {
                [JsonPropertyName("createTime")] public DateTime CreateTime { get; set; }

                [JsonPropertyName("started")] public bool Started { get; set; }

                [JsonPropertyName("distance")] public int Distance { get; set; }

                [JsonPropertyName("horses")] public List<Hors> Horses { get; set; }

                [JsonPropertyName("groundBuffs")] public List<object> GroundBuffs { get; set; }
            }
        }

        public class Hors
        {
            [JsonPropertyName("userId")] public string UserId { get; set; }

            [JsonPropertyName("display")] public string Display { get; set; }

            [JsonPropertyName("step")] public int Step { get; set; }
            
            [JsonPropertyName("stepChange")] public int StepChange { get; set; } = 0;
        }
    }
}