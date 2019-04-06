using System;
using AppKit;
using CoreGraphics;
using Foundation;

namespace ClippySharp
{
    public class AgentView : NSImageView
    {
		public override bool MouseDownCanMoveWindow => true;

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

        NSObject clickMonitor;

        bool ismoving;

        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);
			if (Agent == null) {
				return;
			}

            if (theEvent.ClickCount == 2)
            {
                Agent.Animate();
            }

            //if (ismoving)
            //{
            //    StartMouseMonitoring();
            //} else
            //{
            //    StopMouseMonitoring();
            //}
            //ismoving = !ismoving;
        }

        void StartMouseMonitoring ()
        {
            StopMouseMonitoring();

            clickMonitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.MouseMoved, (NSEvent theEvent) => {

                var currentX = theEvent.LocationInWindow.X - (Window.Frame.Width/2);
                var currentY = theEvent.LocationInWindow.Y - (Window.Frame.Height/2);
                Window.SetFrame(new CoreGraphics.CGRect(currentX, currentY, Window.Frame.Width, Window.Frame.Height), true);
                return theEvent;
            });
        }

        void StopMouseMonitoring ()
        {
            if (clickMonitor != null)
            {
                NSEvent.RemoveMonitor(clickMonitor);
                clickMonitor = null;
            }
        }

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
        }

        void Agent_NeedsRender(object sender, EventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                Image = Agent.GetCurrentImage()?.NativeObject as NSImage;
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
