using System.IO;
using AppKit;
using Foundation;

namespace ClippySharp
{
	public static class AssemblyHelper
	{
		public static string ReadResourceString (string agent, string resource)
		{
			return ReadResourceString ($"agents/{agent}/{resource}");
		}

		public static string ReadResourceString (string resourceName)
		{
			var assembly = typeof (AssemblyHelper).Assembly;
			var name = assembly.GetName ().Name;
			var fullPath = string.Format ("{0}.Resources/{1}", name, resourceName);
			//ClippySharp.Cocoa.Resources.agents.clippy.agent.json
			//ClippySharp.Cocoa.Resources/agents/clippy/agent.json
			using (Stream stream = assembly.GetManifestResourceStream (fullPath))
			using (StreamReader reader = new StreamReader (stream)) {
				string result = reader.ReadToEnd ();
				return result;
			}
		}

		public static NSImage ReadResourceImage (string agent, string resource)
		{
			return ReadResourceImage ($"agents/{agent}/{resource}");
		}

		public static NSImage ReadResourceImage (string resourceName)
		{
			var assembly = typeof (AssemblyHelper).Assembly;
			var name = assembly.GetName ().Name;
			var fullPath = string.Format ("{0}.Resources/{1}", name, resourceName);
			using (Stream stream = assembly.GetManifestResourceStream (fullPath))
				return NSImage.FromStream (stream);
		}
	}
}
