using Newtonsoft.Json;

namespace ClippySharp.Models
{
	public class AgentFrameModel
	{
		[JsonProperty ("duration")]
		public int Duration { get; set; }

		[JsonProperty ("images")]
		public int[][] Images { get; set; }

		[JsonProperty ("sound")]
		public string Sound { get; set; }
	}
}
