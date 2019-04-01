using System;

namespace ClippySharp
{
	public class SoundData
	{
		public string Id { get; }
        public SoundType SoundType { get; } = SoundType.NotDetected;
		public string Data { get; }
		public string Base { get; }

		public SoundData (string id, string data)
		{
            this.Id = id;
			var index = data.IndexOf (';');
			var type = data.Substring ("data:".Length, index - "data:".Length);
			if (type == "audio/mpeg") {
				SoundType = SoundType.Mpeg;
			}

			var separator = data.IndexOf (',');

			index++;
			Base = data.Substring (index, separator - index);
			Data = data.Substring (separator + 1);
		}

        public void Play()
        {
            SoundPlayer.Current.Play(this);
        }
    }
}
