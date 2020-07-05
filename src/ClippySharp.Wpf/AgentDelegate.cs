using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClippySharp
{
	public class AgentDelegate : IAgentDelegate
	{
		public static Bitmap ResizeImage(Image image, int x, int y, int width, int height)
		{
			RectangleF cropRect = new RectangleF(x, y, width, height);
			Bitmap target = new Bitmap(width, height);

			using (Graphics g = Graphics.FromImage(target))
			{
				g.DrawImage(image, new RectangleF(0, 0, target.Width, target.Height),
								 cropRect,
								 GraphicsUnit.Pixel);
			}
			target.MakeTransparent();
			return target;
		}

		private BitmapImage Convert(Bitmap bitmap)
		{
			using (MemoryStream memory = new MemoryStream())
			{
				bitmap.Save(memory, ImageFormat.Png);
				memory.Position = 0;
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				return bitmapImage;
			}
		}

		public IImageWrapper GetImage (IImageWrapper imageWrapper, int x, int y, IAgent agent)
		{
			ImageWrapper imageView = null;
			Application.Current.Dispatcher.Invoke(() => {
				var image = (BitmapImage)imageWrapper.NativeObject;
				var bitmapImage = BitmapImage2Bitmap(image);
				var resized = ResizeImage(bitmapImage, x, y, (int)agent.ImageSize.Width, (int)agent.ImageSize.Height);
				var imgSource = Convert(resized);
				imageView = new ImageWrapper (imgSource);// picture);
			});

			return imageView;
		}

		ImageSource GetFromUrl(string url)
		{
			try
			{
				var imageSource = new BitmapImage();
				imageSource.BeginInit();
				imageSource.UriSource = new Uri(url);
				imageSource.EndInit();
				return imageSource;
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex);
			}
			return null;
		}

		private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
		{
			// BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));
			using (MemoryStream outStream = new MemoryStream())
			{
				BitmapEncoder enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(bitmapImage));
				enc.Save(outStream);
				var bitmap = new Bitmap(new System.Drawing.Bitmap(outStream));
				bitmap.MakeTransparent();
				return bitmap;
			}
		}

		public IImageWrapper GetImage(string url)
		{
			ImageSource image = null;
			Application.Current.Dispatcher.Invoke(() => { image = GetFromUrl(url); });
			return new ImageWrapper (image);
		}

		public IImageWrapper GetImageFromFilePath(string filePath)
		{
			BitmapImage source = null;
			Application.Current.Dispatcher.Invoke(() => { source = new BitmapImage(new Uri(filePath)); });
			return new ImageWrapper(source);
		}

		public IImageWrapper GetImage (Stream stream)
		{
			ImageSource image = null;
			Application.Current.Dispatcher.Invoke(() => {
				var imageSource = new BitmapImage();
				imageSource.BeginInit();
				imageSource.StreamSource = stream;
				imageSource.EndInit();
				image = imageSource; 
			});
			return new ImageWrapper (image);
		}

		public IImageWrapper GetImageSheet (string agentName, string resourceName)
		{
			return AssemblyHelper.ReadResourceImage (agentName, resourceName);
		}
	}
}
