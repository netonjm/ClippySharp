using System;
using System.Threading;
using AppKit;
using ClippySharp.Models;
using CoreGraphics;
using Newtonsoft.Json;
using System.Linq;

namespace ClippySharp
{

	public class Agent : IDisposable
    {
		public event EventHandler NeedsRender;
		public event EventHandler Paused;
		public event EventHandler Resumed;
		public event EventHandler BalloonClosed;

		public bool Sound { get; set; } = true;
		public bool Hidden { get; internal set; }
		public CGSize ImageSize { get; }
        public CGRect Offset { get; set; } = CGRect.Empty;

		internal AgentAnimator Animator { get; }
        internal AgentModel Model { get; }

		readonly QueueProcessor queue;

		public Agent(string agent)
        {
			//we initialize context
			AgentContext.Current.Initialize ();

            queue = new QueueProcessor();

            var agentJson = AssemblyHelper.ReadResourceString(agent, "agent.json");
            Model = JsonConvert.DeserializeObject<AgentModel>(agentJson);


            ImageSize = new CGSize(Model.FrameSize[0], Model.FrameSize[1]);
            Animator = new AgentAnimator(agent, this);

            Animator.NeedsRefresh += Animator_NeedsRefresh;
		}

		#region Public API

		public void PlaySound (string id)
		{
			if (!Sound || Hidden || string.IsNullOrEmpty (id)) {
				return;
			}
			var sound = Animator.Sounds.FirstOrDefault (s => s.Id == id);
			if (sound == null) {
				throw new Exception ("");
			}

			SoundPlayer.Current.Play (sound);
		}

		public void Speak (string text, string hold)
		{
			if (Hidden) {
				return;
			}
			AddToQueue (() => {
				//balloon.Speak(complete, text, hold);
			});
		}

		public void Stop ()
		{
			this.queue.Clear ();

			Animator.ExitAnimation ();
			CloseBalloon ();
		}

		public void Show (bool fast)
		{
			Hidden = false;
			if (fast) {
				Resume ();
				Animator.OnQueueEmpty ();
				return;
			}

			Resume ();
			Play ("Show");
		}

		public string[] GetAnimations ()
		{
			return Animator.Animations.Select (s => s.Name).ToArray ();
		}

		public bool Animate ()
		{
			if (Hidden) {
				return false;
			}

			var animations = Animator.Animations.Where (s => !s.IsIdle ())
				.ToList ();

			var animation = AgentAnimator.GetRandomAnimation (animations);
			return Play (animation.Name);
		}

		public bool GestureAt (float x, float y)
		{
			if (Hidden) {
				return false;
			}
			var d = GetDirection (x, y);
			var gAnim = "Gesture" + d.ToString ();
			var lookAnim = "Look" + d.ToString ();

			var animation = Animator.HasAnimation (gAnim) ? gAnim : lookAnim;
			return Play (animation);
		}

		public void MoveTo (float x, float y, int duration)
		{

		}

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

		public void RefreshImage ()
		{
			NeedsRender?.Invoke (this, EventArgs.Empty);
		}

		public bool Play (string animation, int timeout = 5000)
		{
			if (Hidden) {
				return false;
			}

			if (!Animator.HasAnimation (animation)) return false;

			//addt
			AddToQueue (() => {
				var completed = false;

				EventHandler<AnimationStateEventArgs> handler = null;
				handler = (s, e) => {
					Animator.AnimationEnded -= handler;

					if (timeout > 0) {
						//window.setTimeout($.proxy(function() {
						//    if (completed) return;
						//    // exit after timeout
						//    this._animator.exitAnimation();
						//}, this), timeout)
					}
				};

				Animator.AnimationEnded += handler;

				PlayInternal (animation);
			});
			return true;
		}

		public void CloseBalloon ()
		{
			BalloonClosed?.Invoke (this, EventArgs.Empty);
		}

		#endregion

		void Animator_NeedsRefresh (object sender, EventArgs e)
		{
			NeedsRender?.Invoke (this, e);
		}

		internal NSImage GetCurrentImage ()
		{
			return Animator.GetCurrentImage ();
		}

		void AddToQueue (Action p)
		{
			queue.Enqueue (p);
		}

		void AddDelay (int miliseconds = 250)
		{
			AddToQueue (() =>
			{
				this.Animator.OnQueueEmpty ();
				Thread.Sleep (miliseconds);
			});
		}

		void PlayInternal (string animation)
		{
			if (Animator.IsIdleAnimation ()) {
				//this._idleDfd.done($.proxy(function() {
				//    this._playInternal(animation, callback);
				//}, this))
			}

			this.Animator.ShowAnimation (animation);
		}

		AgentDirection GetDirection (float x, float y)
		{
			var offset = Offset;
			var h = ImageSize.Height;
			var w = ImageSize.Width;

			var centerX = (offset.Left + w / 2);
			var centerY = (offset.Top + h / 2);

			var a = centerY - y;
			var b = centerX - x;

			var r = Math.Round ((180 * Math.Atan2 (a, b)) / Math.PI);

			// Left and Right are for the character, not the screen :-/
			if (-45 <= r && r < 45) return AgentDirection.Right;
			if (45 <= r && r < 135) return AgentDirection.Up;
			if (135 <= r && r <= 180 || -180 <= r && r < -135) return AgentDirection.Left;
			if (-135 <= r && r < -45) return AgentDirection.Down;

			// sanity check
			return AgentDirection.Top;
		}

		public void Dispose ()
		{
			Animator.NeedsRefresh -= Animator_NeedsRefresh;
		}

	}
}
