using System;
using System.Collections.Generic;
using ClippySharp.Models;

namespace ClippySharp
{
    public class AgentAnimation
    {
        const string Idle = "Idle";

        public string Name { get; }

        public List<AgentAnimationFrame> Frames { get; }
        internal bool useExitBranching;

        public AgentAnimation(AgentAnimator animator, string name, AgentFrameModel[] frames)
        {
            Name = name;
            Frames = new List<AgentAnimationFrame>();

            foreach (var frame in frames)
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
