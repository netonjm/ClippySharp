using System.IO;
using AppKit;
using CoreGraphics;

namespace ClippySharp
{
	public class AgentDelegate : IAgentDelegate
	{
		public IImageWrapper GetImage (IImageWrapper imageWrapper, int x, int y, IAgent agent)
		{
			var currentY = y;// imageSheet.CGImage.Height - agent.ImageSize.Height - y;

			var image = ((NSImage)imageWrapper.NativeObject).CGImage.WithImageInRect (new CGRect (x, currentY, agent.ImageSize.Width, agent.ImageSize.Height));
			var nativeImage = new NSImage (image, new CGSize (agent.ImageSize.Width, agent.ImageSize.Height));
			return new ImageWrapper (nativeImage);
		}

		public IImageWrapper GetImage (Stream stream)
		{
			var image = NSImage.FromStream (stream);
			return new ImageWrapper (image);
		}

		public IImageWrapper GetImageSheet (string agentName, string resourceName)
		{
			return AssemblyHelper.ReadResourceImage (agentName, resourceName);
		}
	}
}
