using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClippySharp.Models
{
	public class AgentModel
	{
		[JsonProperty ("overlayCount")]
		public int OverlayCount { get; set; }

		[JsonProperty ("sounds")]
		public string[] Sounds { get; set; }

		[JsonProperty ("framesize")]
		public int[] FrameSize { get; set; }

		[JsonProperty ("animations")]
		public Dictionary<string, Dictionary<string, AgentFrameModel[]>> Animations { get; set; }
	}
}
