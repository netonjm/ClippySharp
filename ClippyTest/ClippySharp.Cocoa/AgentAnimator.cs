using System;
using System.Collections.Generic;
using AppKit;
using ClippySharp;
using ClippySharp.Models;
using Newtonsoft.Json;
using System.Linq;
using CoreGraphics;

namespace ClippyTest
{
    public class AgentAnimator
    {
        public event EventHandler NeedsRefresh;

        readonly NSImage imageSheet;
        private AgentAnimation _currentAnimation;

        //private string path;
        //private AgentData[] data;
        //private AgentSounds[] sounds;
        internal string currentAnimationName;
        //internal string[] animations;
        public List<SoundData> Sounds { get; }
        public List<AgentAnimation> Animations { get; }

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
           return CurrentFrame?.GetImage();
        }

        void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            aTimer.Stop();
            _step();
        }

        System.Timers.Timer aTimer;

        public NSImage GetImage(int x, int y)
        {
            var currentY = imageSheet.CGImage.Height - (agent.ImageSize.Height + y);
            var image = imageSheet.CGImage.WithImageInRect(new CGRect(x, currentY, agent.ImageSize.Width, agent.ImageSize.Height));
            return new NSImage(image, new CGSize(agent.ImageSize.Width, agent.ImageSize.Height));
        }

        public bool HasAnimation(string name)
        {
            return Animations.Any(s => s.Name == name);
        }

        bool _exiting;
        private int _currentFrameIndex;

        internal void ExitAnimation()
        {
            _exiting = true;
        }


        #region Queue and Idle handling

        public bool IsIdleAnimation()
        {
            return _currentAnimation?.IsIdle() ?? false;
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

            this._currentAnimation = Animations.FirstOrDefault(s => s.Name == animationName);
            this.currentAnimationName = animationName;


             
            //if (!this._started)
            //{
                this._step();
               //this._started = true;
            //}

            this._currentFrameIndex = -1;
            return true;
        }


        bool _atLastFrame ()
        {
            return this._currentFrameIndex >= this._currentAnimation.Frames.Count - 1;
        }

        private void _step()
        {
            if (this._currentAnimation == null) return;

            var newFrameIndex = (int)Math.Min(this._getNextAnimationFrame(), this._currentAnimation.Frames.Count - 1);
            var frameChanged = this.CurrentFrame != null || this._currentFrameIndex != newFrameIndex;
            this._currentFrameIndex = newFrameIndex;

            NeedsRefresh?.Invoke(this, EventArgs.Empty);
            this.PlaySound();

            if (frameChanged && this._atLastFrame())
            {
                if (this._currentAnimation.useExitBranching && !this._exiting)
                {
                    AnimationEnded?.Invoke(this, new AnimationStateEventArgs ( currentAnimationName, AnimationStates.Waiting));
                }
                else
                {
                    AnimationEnded?.Invoke(this, new AnimationStateEventArgs(currentAnimationName, AnimationStates.Exited));

                }
                return;
            }

            aTimer.Interval = CurrentFrame.Duration;
            aTimer.Start();
        }

        public event EventHandler<AnimationStateEventArgs> AnimationEnded;

        AgentAnimationFrame CurrentFrame
        {
            get
            {
                if (_currentAnimation == null || _currentFrameIndex < 0 || _currentFrameIndex >= _currentAnimation.Frames.Count)
                {
                    return null;
                }
                return _currentAnimation.Frames[this._currentFrameIndex];
            }
        }

        public void PlaySound ()
        {
            if (string.IsNullOrEmpty (CurrentFrame.Sound))
            {
                return;
            }
            Sounds.FirstOrDefault(s => s.Id == CurrentFrame.Sound)?.Play ();
        }

        int _getNextAnimationFrame ()
        {
            if (this.CurrentFrame == null) 
                return 0;
            if (_currentFrameIndex >= _currentAnimation.Frames.Count)
                return 0;
            //if (this._exiting && currentFrame.exitBranch !== undefined) 
            //{
            //    return currentFrame.exitBranch;
            //}
            //else if (branching)
            //{
            //    var rnd = Math.random() * 100;
            //    for (var i = 0; i < branching.branches.length; i++)
            //    {
            //        var branch = branching.branches[i];
            //        if (rnd <= branch.weight)
            //        {
            //            return branch.frameIndex;
            //        }

            //        rnd -= branch.weight;
            //    }
            //}

            return this._currentFrameIndex + 1;
        }

        internal void Pause()
        {
           
        }

        internal void Resume()
        {
            this._step();
        }

        bool _started;
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
