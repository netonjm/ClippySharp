using System.Collections.Generic;
using ClippySharp.Models;

namespace ClippySharp
{
    internal class AgentAnimationFrame
    {
        readonly AgentAnimator animator;
        readonly AgentFrameModel model;

        public string ExitBranch => model.ExitBranch;

        public Dictionary<string, AgentFrameBranchModel[]> Branching => model.Branching;

        public int Duration => model.Duration;
        public string Sound => model.Sound;

        public AgentAnimationFrame(AgentAnimator animator, AgentFrameModel mode)
        {
            this.animator = animator;
            this.model = mode;
        }

        public IImageWrapper GetImage ()
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
