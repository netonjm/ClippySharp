using System;
using System.Collections.Generic;
using ClippySharp.Models;

namespace ClippySharp
{
    internal class AgentAnimation
    {
        const string Idle = "Idle";

        public string Name { get; }

        public List<AgentAnimationFrame> Frames { get; }
        internal bool useExitBranching;

		AgentAnimationModel model;

		public AgentAnimation(AgentAnimator animator, string name, AgentAnimationModel model)
        {
			this.model = model;

			Name = name;
            Frames = new List<AgentAnimationFrame>();

            foreach (var frame in model.Frames)
            {
                Frames.Add(new AgentAnimationFrame(animator, frame));
            }
        }

        internal bool IsIdle()
        {
            return Name.IndexOf(Idle, StringComparison.Ordinal) == 0;
        }
    }
}
