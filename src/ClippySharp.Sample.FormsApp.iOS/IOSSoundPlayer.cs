using ClippySharp;
using ClippySharp.Sample.FormsApp.iOS;
using Xamarin.Forms;

[assembly: Dependency (typeof (IOSSoundPlayer))]

namespace ClippySharp.Sample.FormsApp.iOS
{
	public class IOSSoundPlayer : SoundPlayer, ISoundPlayer
	{

	}
}
