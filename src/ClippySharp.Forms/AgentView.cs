using System;
using Xamarin.Forms;

namespace ClippySharp.Forms
{
    public class AgentView : Image
    {
		public Agent Agent { get; private set; }

		public void ConfigureAgent (Agent agent)
		{
			if (this.Agent != null) {
				this.Agent.NeedsRender -= Agent_NeedsRender;
				this.Agent.Dispose ();
			}

			this.Agent = agent;
			this.Agent.NeedsRender += Agent_NeedsRender;
			this.Agent.RefreshImage ();
		}

		public AgentView ()
		{
			//Initialize ();
		}

		void Agent_NeedsRender(object sender, EventArgs e)
        {
			Device.BeginInvokeOnMainThread (() =>
			{
			    Source = Agent.GetCurrentImage()?.NativeObject as ImageSource;
			});
		}

   //     protected override void Dispose(bool disposing)
   //     {
			//if (Agent != null) {
			//	Agent.NeedsRender -= Agent_NeedsRender;
			//}

        //    base.Dispose(disposing);
        //}
    }
}
