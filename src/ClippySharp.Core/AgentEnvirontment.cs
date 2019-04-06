using System;
using System.IO;

namespace ClippySharp
{
	public interface ISoundPlayer
	{
		ISoundPlayer Play (SoundData data, int delay = 500);
		ISoundPlayer Sleep (int miliseconds);
	}

	public class Point
	{
		public float X { get; set; }
		public float Y { get; set; }

		public static Point Zero { get; set; } = new Point ();
	}

	public class Size
	{
		public Size ()
		{

		}

		public Size (int width, int height)
		{
			Width = width;
			Height = height;
		}

		public float Width { get; set; }
		public float Height { get; set; }

		public static Size Zero { get; set; } = new Size ();
	}

	public class Rectangle
	{
		public Point Position = new Point ();
		public Size Size = new Size ();

		public float X {
			get => Position.X;
			set => Position.X = value;
		}

		public float Y {
			get => Position.Y;
			set => Position.Y = value;
		}
		public float Width {
			get => Size.Width;
			set => Size.Width = value;
		}
		public float Height {
			get => Size.Height;
			set => Size.Height = value;
		}

		public static Rectangle Zero { get; set; } = new Rectangle { X = 0, Y = 0, Width = 0, Height = 0 };

		public float Top => Y;
		public float Left => X + Width;
	}

	public interface IAgent : IDisposable
	{
		bool Hidden { get; }
		bool Sound { get; set; }
		Size ImageSize { get; }
		void PlaySound (string id);
		void Speak (string text, string hold);
		void Stop ();
		void Show (bool fast);
		bool GestureAt (float x, float y);
		void Pause ();
	}

	public interface IAgentDelegate
	{
		IImageWrapper GetImage (IImageWrapper imageWrapper, int x, int y, IAgent agent);

		IImageWrapper GetImageSheet (string agentName, string resourceName);

		IImageWrapper GetImage (Stream stream);
	}

	public class AgentEnvirontment
	{
		AgentEnvirontment ()
		{

		}


		//TODO: change this to use a directory thing
		static readonly string[] agents = {
			"bonzi", "clippy", "f1", "genius", "links", "merlin", "peedy", "rocky", "rover"
		};

		public string[] GetAgents ()
		{
			return agents;
		}

		public IAgentDelegate Delegate { get; private set; }

		public ISoundPlayer SoundPlayer { get; private set; }

		public void Initialize (IAgentDelegate agentDelegate, ISoundPlayer soundPlayer)
		{
			Delegate = agentDelegate;
			SoundPlayer = soundPlayer;
		}

		static AgentEnvirontment current;
		public static AgentEnvirontment Current {
			get {
				if (current == null) {
					current = new AgentEnvirontment ();
				}
				return current;
			}
		}
	}
}
