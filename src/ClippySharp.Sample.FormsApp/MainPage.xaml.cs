using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ClippySharp.Forms;

namespace ClippySharp.Sample.FormsApp
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible (true)]
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent ();
			var soundPlayer = DependencyService.Get<ISoundPlayer> ();
			var agentDelegate = DependencyService.Get<IAgentDelegate> ();

			AgentEnvirontment.Current.Initialize (agentDelegate, soundPlayer);

			var agentView = new AgentView ();
			agentView.WidthRequest = 200;
			agentView.HeightRequest = 200;

			var agent = new Agent ("clippy");

			agentView.ConfigureAgent (agent);

			agent.Animate ();

			container.Children.Add (agentView);
		}
	}
}
