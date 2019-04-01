using System;
using System.Collections.Generic;
using AppKit;
using ClippySharp;
using ClippySharp.Models;
using Newtonsoft.Json;
using System.Linq;
using CoreGraphics;

namespace ClippySharp
{
    public class AgentAnimator
    {
        public event EventHandler<AnimationStateEventArgs> AnimationEnded;
        public event EventHandler NeedsRefresh;

        readonly NSImage imageSheet;
        private AgentAnimation currentAnimation;

        bool _started;
        AgentAnimationFrame currentFrame;
        internal string currentAnimationName;
        public List<SoundData> Sounds { get; }
        public List<AgentAnimation> Animations { get; }

        System.Timers.Timer aTimer;

        readonly Agent agent;

        public AgentAnimator(string name, Agent agent)
        {
            this.agent = agent;
            //sound processing
            var soundJson = AssemblyHelper.ReadResourceString(name, "sounds-mp3.json");
            var soundData = JsonConvert.DeserializeObject<Dictionary<string, string>>(soundJson);

            Sounds = new List<SoundData>();
            foreach (var data in soundData)
            {
                Sounds.Add(new SoundData(data.Key, data.Value));
            }

            //image processing
            imageSheet = AssemblyHelper.ReadResourceImage(name, "map.png");

            Animations = new List<AgentAnimation>();
            foreach (var animation in agent.Model.Animations)
            {
                if (animation.Value.TryGetValue("frames", out AgentFrameModel[] frames))
                {
                    Animations.Add(new AgentAnimation(this, animation.Key, frames));
                };
            }

            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += ATimer_Elapsed;
        }

        internal NSImage GetCurrentImage()
        {
           return currentFrame?.GetImage();
        }

        void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            aTimer.Stop();
            _step();
        }

        public NSImage GetImage(int x, int y)
        {
            var currentY = y;// imageSheet.CGImage.Height - agent.ImageSize.Height - y;
            var image = imageSheet.CGImage.WithImageInRect(new CGRect(x, currentY, agent.ImageSize.Width, agent.ImageSize.Height));
            return new NSImage(image, new CGSize(agent.ImageSize.Width, agent.ImageSize.Height));
        }

        public bool HasAnimation(string name)
        {
            return Animations.Any(s => s.Name == name);
        }

        bool _exiting;
        private int currentFrameIndex;

        internal void ExitAnimation()
        {
            _exiting = true;
        }

        #region Queue and Idle handling

        public bool IsIdleAnimation()
        {
            return currentAnimation?.IsIdle() ?? false;
        }

        static readonly Random rnd = new Random(DateTime.Now.Millisecond);
        float GetRandom() => rnd.Next(0, 1);

        public static AgentAnimation GetRandomAnimation(List<AgentAnimation> animations)
        {
            return animations[(int)rnd.Next(0, animations.Count - 1)];
        }

        public AgentAnimation GetRandomAnimation ()
        {
            return GetRandomAnimation(Animations);
        }

        public AgentAnimation GetIdleAnimation()
        {
            var r = new List<AgentAnimation>();
            foreach (var animation in Animations)
            {
                if (animation.IsIdle ())
                {
                    r.Add(animation);
                }
            }

            return GetRandomAnimation(r);
        }

        public void OnQueueEmpty ()
        {
            if (agent.Hidden || this.IsIdleAnimation()) return;
            var idleAnim = this.GetIdleAnimation();
            this.ShowAnimation(idleAnim.Name);
        }

        #endregion

        public bool ShowAnimation (string animationName)
        {
            this._exiting = false;
            if (!this.HasAnimation(animationName))
            {
                return false;
            }

            this.currentAnimation = Animations.FirstOrDefault(s => s.Name == animationName);
            this.currentAnimationName = animationName;

            if (!this._started)
            {
                this._step();   
                this._started = true;
            }

            this.currentFrame = null;
            this.currentFrameIndex = 0;
            return true;
        }

        bool IsAtLastFrame ()
        {
            return this.currentFrameIndex >= this.currentAnimation.Frames.Count - 1;
        }

        private void _step()
        {
			//this is under a timer an needs

			AgentAnimation animation = this.currentAnimation;
			var frame = this.currentFrame;

			if (animation == null) return;

            var newFrameIndex = Math.Min(this.GetNextAnimationFrame(), animation.Frames.Count - 1);
            var frameChanged = frame != null && this.currentFrameIndex != newFrameIndex;
            this.currentFrameIndex = newFrameIndex;

            // always switch frame data, unless we're at the last frame of an animation with a useExitBranching flag.
            if (!(this.IsAtLastFrame() && animation.useExitBranching))
            {
				currentFrame = frame = animation.Frames[this.currentFrameIndex];
            }

            NeedsRefresh?.Invoke(this, EventArgs.Empty);
            this.PlaySound();

            aTimer.Interval = frame.Duration;
            aTimer.Start();

            if (frameChanged && this.IsAtLastFrame())
            {
                if (animation.useExitBranching && !this._exiting)
                {
                    AnimationEnded?.Invoke(this, new AnimationStateEventArgs ( currentAnimationName, AnimationStates.Waiting));
                }
                else
                {
                    AnimationEnded?.Invoke(this, new AnimationStateEventArgs(currentAnimationName, AnimationStates.Exited));
                }
            }

        }

        public void PlaySound ()
        {
            var sound = currentFrame?.Sound;
            if (string.IsNullOrEmpty (sound))
            {
                return;
            }
            Sounds.FirstOrDefault(s => s.Id == sound)?.Play ();
        }

        int GetNextAnimationFrame ()
        {
            if (currentFrame == null) 
                return 0;
            if (currentFrameIndex >= currentAnimation.Frames.Count)
                return 0;

            var branching = currentFrame.Branching;

            if (this._exiting && currentFrame.ExitBranch != null) 
            {
                return int.Parse (currentFrame.ExitBranch);
            }
            else if (branching != null)
            {
                var random = GetRandom () * 100;
                var branches = branching["branches"];

                for (var i = 0; i < branches.Count (); i++)
                {
                    var branch = branches[i];
                    if (random <= branch.Weight)
                    {
                        return branch.FrameIndex;
                    }

                    random -= branch.Weight;
                }
            }

            return this.currentFrameIndex + 1;
        }

        internal void Pause()
        {
            aTimer.Stop();
        }

        internal void Resume()
        {
            aTimer.Start();
        }

    }

    public class AnimationStateEventArgs : EventArgs
    {
        public AnimationStateEventArgs(string name, AnimationStates states)
        {
            Name = name;
            State = states;
        }

        public string Name { get; }
        public AnimationStates State { get; }
    }
}
