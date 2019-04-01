using System;
using System.Collections;
using System.Collections.Generic;
using AppKit;
using CoreGraphics;
using System.Linq;

namespace ClippyTest
{
    public enum AnimationStates
    {
        Waiting = 1,
        Exited = 0
    }

    public class AgentData
    {

    }

    public class AgentSounds
    {

    }

    public class AgentBalloon
    {
        private Agent agent;

        public AgentBalloon(Agent agent)
        {
            this.agent = agent;
        }

        internal void Hide()
        {

        }

        internal void Speak(object complete, string text, string hold)
        {
           
        }
    }

    public enum AgentDirection
    {
        Right, Up, Left, Down, Top
    }

}
