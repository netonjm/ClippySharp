using System;
using System.Threading;
using AppKit;
using ClippySharp.Models;
using CoreGraphics;
using Newtonsoft.Json;
using System.Linq;

namespace ClippySharp
{
    public class Agent
    {
        public event EventHandler HideBalloon;
      
        public CGSize ImageSize { get; }
      
        public event EventHandler NeedsRender;

        public CGRect Offset { get; set; } = CGRect.Empty;

        QueueProcessor queue;

        public AgentAnimator Animator { get; }
        public bool Hidden { get; internal set; }

        internal AgentModel Model { get; } 

        public Agent(string agent)
        {
            queue = new QueueProcessor();

            var agentJson = AssemblyHelper.ReadResourceString(agent, "agent.json");
            Model = JsonConvert.DeserializeObject<AgentModel>(agentJson);

            ImageSize = new CGSize(Model.FrameSize[0], Model.FrameSize[1]);

            Animator = new AgentAnimator(agent, this);

            Animator.NeedsRefresh += (sender, e) =>
            {
                NeedsRender?.Invoke(this, e);
            };
        }

        public void RefreshImage ()
        {
            NeedsRender?.Invoke(this, EventArgs.Empty);
        }

        internal NSImage GetCurrentImage()
        {
            return Animator.GetCurrentImage();
        }

        public void PlaySound (string id)
        {
            var sound = Animator.Sounds.FirstOrDefault(s => s.Id == id);
            if (sound == null)
            {
                throw new Exception("");
            }

            SoundPlayer.Current.Play(sound);
        }

        public void Speak(string text, string hold)
        {
            AddToQueue(() =>
            {
                //balloon.Speak(complete, text, hold);
            });
        }

        private void AddToQueue(Action p)
        {
            queue.Enqueue(p);
        }

        public bool Play (string animation, int timeout = 0, Action cb = null)
        {
            //var selected = Animations.FirstOrDefault(s => s.Name == animation);
            //if (selected == null)
            //{
            //    return;
            //}
          

            if (!Animator.HasAnimation(animation)) return false;
            if (timeout == 0) timeout = 5000;

            //addt
            AddToQueue(() =>
            {
                var completed = false;

                Animator.AnimationEnded += (sender, e) =>
                {
                    if (e.State == AnimationStates.Exited)
                    {
                        completed = true;
                        cb?.Invoke();
                    }
                };

                if (timeout > 0)
                {
                    //window.setTimeout($.proxy(function() {
                    //    if (completed) return;
                    //    // exit after timeout
                    //    this._animator.exitAnimation();
                    //}, this), timeout)
                }

                PlayInternal(animation);
            });
            return true;
        }

        public void CloseBalloon ()
        {
            BalloonClosed?.Invoke(this, EventArgs.Empty);
        }

        public EventHandler BalloonClosed;

        public void AddDelay (int miliseconds = 250)
        {
            AddToQueue(() =>
          {
              this.Animator.OnQueueEmpty();
              Thread.Sleep(miliseconds);
          });
        }

        private void PlayInternal(string animation)
        {
            if (Animator.IsIdleAnimation() )
            {
                    //this._idleDfd.done($.proxy(function() {
                    //    this._playInternal(animation, callback);
                    //}, this))
            }

            this.Animator.ShowAnimation(animation);
        }

        public void Stop()
        {
            this.queue.Clear();
            //this.animator.exitAnimation();
            //this.balloon.Hide();
            HideBalloon?.Invoke(this, EventArgs.Empty);
        }

        public bool Animate()
        {
            var animations = Animator.Animations.Where(s => !s.IsIdle())
                .ToList ();

            var animation = AgentAnimator.GetRandomAnimation(animations);
            return Play(animation.Name);
        }

        #region API

        public bool GestureAt(float x, float y)
        {
            var d = GetDirection(x, y);
            var gAnim = "Gesture" + d.ToString();
            var lookAnim = "Look" + d.ToString();

            var animation = Animator.HasAnimation(gAnim) ? gAnim : lookAnim;
            return Play(animation);
        }

        public void MoveTo(float x, float y, int duration)
        {

        }

        AgentDirection GetDirection(float x, float y)
        {
            var offset = Offset;
            var h = ImageSize.Height;
            var w = ImageSize.Width;

            var centerX = (offset.Left + w / 2);
            var centerY = (offset.Top + h / 2);

            var a = centerY - y;
            var b = centerX - x;

            var r = Math.Round((180 * Math.Atan2(a, b)) / Math.PI);

            // Left and Right are for the character, not the screen :-/
            if (-45 <= r && r < 45) return AgentDirection.Right;
            if (45 <= r && r < 135) return AgentDirection.Up;
            if (135 <= r && r <= 180 || -180 <= r && r < -135) return AgentDirection.Left;
            if (-135 <= r && r < -45) return AgentDirection.Down;

            // sanity check
            return AgentDirection.Top;
        }

        #endregion

        #region Pause Resume

        public event EventHandler Paused;
        public event EventHandler Resumed;

        public void Pause ()
        {
            Animator.Pause();
            Paused?.Invoke(this, EventArgs.Empty);
        }

        public void Resume()
        {
            Animator.Resume();
            Resumed?.Invoke(this, EventArgs.Empty);
        }

        #endregion


    }
}
