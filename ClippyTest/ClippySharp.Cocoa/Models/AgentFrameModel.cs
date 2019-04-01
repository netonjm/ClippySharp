using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClippySharp.Models
{
    public class AgentFrameModel
    {
        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("images")]
        public int[][] Images { get; set; }

        [JsonProperty("sound")]
        public string Sound { get; set; }

        [JsonProperty("exitBranch")]
        public string ExitBranch { get; set; }

        [JsonProperty("branching")]
        public Dictionary<string,  AgentFrameBranchModel[]> Branching { get; set; }
    }

    public class AgentFrameBranchModel
    {
        [JsonProperty("frameIndex")]
        public int FrameIndex { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }
    }
}