using System;
using AppKit;
using ClippySharp;
using Foundation;

namespace ClippyTest
{
    public class AgentView : NSImageView
    {
        readonly Agent agent;
        public AgentView(Agent agent)
        {
            this.agent = agent;

            agent.NeedsRender += Agent_NeedsRender;
            agent.RefreshImage();
        }

        NSObject clickMonitor;

        bool ismoving;

        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);
            if (theEvent.ClickCount == 1)
            {
                agent.Animate();
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
                Image = agent.GetCurrentImage();
            });
        }

        protected override void Dispose(bool disposing)
        {
            agent.NeedsRender -= Agent_NeedsRender;
            base.Dispose(disposing);
        }
    }
}
