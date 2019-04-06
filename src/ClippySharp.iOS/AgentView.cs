using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ClippySharp
{
    public class AgentView : UIImageView
    {
		public event EventHandler TapPressed;
		public event EventHandler TapUnPressed;

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
			Initialize ();
		}

		public AgentView (NSCoder coder) : base (coder)
		{
		}

		protected AgentView (NSObjectFlag t) : base (t)
		{
		}

		protected internal AgentView (IntPtr handle) : base (handle)
		{
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			TapPressed?.Invoke (this, EventArgs.Empty);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			TapUnPressed?.Invoke (this, EventArgs.Empty);
		}

		void Initialize ()
		{
			UserInteractionEnabled = true;
		}

		public AgentView (CGRect frame) : base (frame)
		{
			Initialize ();
		}

		void Agent_NeedsRender(object sender, EventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                Image = Agent.GetCurrentImage()?.NativeObject as UIImage;
            });
        }

        protected override void Dispose(bool disposing)
        {
			if (Agent != null) {
				Agent.NeedsRender -= Agent_NeedsRender;
			}

            base.Dispose(disposing);
        }
    }
}
