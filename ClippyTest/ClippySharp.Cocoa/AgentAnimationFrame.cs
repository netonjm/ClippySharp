using AppKit;
using ClippySharp.Models;

namespace ClippyTest
{
    public class AgentAnimationFrame
    {
        readonly AgentAnimator animator;
        readonly AgentFrameModel model;

        public int Duration => model.Duration;
        public string Sound => model.Sound;

        public AgentAnimationFrame(AgentAnimator animator, AgentFrameModel mode)
        {
            this.animator = animator;
            this.model = mode;
        }

        public NSImage GetImage ()
        {
            var image = model.Images?[0];
            if (image != null)
            {
                return animator.GetImage(image[0], image[1]);
            }
            return null;
        }
    }
}
