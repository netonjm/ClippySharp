using System;
using System.Threading;
using AVFoundation;
using Foundation;

namespace ClippySharp
{
	public class SoundPlayer : ISoundPlayer
	{
        public SoundPlayer ()
        {

        }

        //static SoundPlayer current;
        //public static SoundPlayer Current
        //{
        //    get
        //    {
        //        if (current == null)
        //        {
        //            current = new SoundPlayer();
        //        }
        //        return current;
        //    }
        //}

        public ISoundPlayer Play (SoundData data, int delay = 500)
		{
			NSError error = null;
			try {
				var audioPlayerData = new NSData (data.Data, NSDataBase64DecodingOptions.None);
				var player = new AVAudioPlayer (audioPlayerData, "AVFileTypeMPEGLayer3", out error);
				player.PrepareToPlay ();
				player.Play ();
                Sleep(delay);
            } catch (Exception ex) {
				Console.WriteLine (ex.Message + " " + error?.Description);
			}
            return this;
		}

        public ISoundPlayer Sleep (int miliseconds)
        {
            Thread.Sleep(miliseconds);
            return this;
        }
	}
}
