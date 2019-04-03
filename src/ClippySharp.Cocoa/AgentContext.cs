using System;
using System.IO;
using System.Linq;

namespace ClippySharp
{
	public class AgentContext
	{
		readonly string AgentsDirectoryPath;

		AgentContext ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			AgentsDirectoryPath = Path.Combine (home, ".cache", "MonoDevelop.Agent", "agents");
		}

		public void Initialize ()
		{
			//we want create basic resources in the case was not created
			Console.WriteLine ("Loading agents from: {0}", AgentsDirectoryPath);
			if (!Directory.Exists (AgentsDirectoryPath)) {
				Directory.CreateDirectory (AgentsDirectoryPath);
			}

			var modulesDirectories = Directory.EnumerateDirectories (AgentsDirectoryPath);
			Console.WriteLine ("{0} agent/s found.", modulesDirectories.Count ());
			foreach (var module in modulesDirectories) {
				Console.WriteLine ("- {0}", module);
			}
		}

		//TODO: change this to use a directory thing
		static readonly string[] agents = {
			"bonzi", "clippy", "f1", "genius", "links", "merlin", "peedy", "rocky", "rover"
		};

		public string[] GetAgents ()
		{
			return agents;
		}

		static AgentContext current;
		public static AgentContext Current {
			get {
				if (current == null) {
					current = new AgentContext ();
				}
				return current;
			}
		}
	}
}
