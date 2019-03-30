using System;
using AppKit;
using Foundation;

namespace ClippyTest
{
    public class AgentView : NSImageView
    {
        readonly Agent agent;
        public AgentView(Agent agent)
        {
            this.agent = agent;

            agent.NeedsRender += Agent_NeedsRender;

            agent.RefreshImage();
        }

        NSObject clickMonitor;

        bool ismoving;

        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);

            //if (ismoving)
            //{
            //    StartMouseMonitoring();
            //} else
            //{
            //    StopMouseMonitoring();
            //}
            //ismoving = !ismoving;
        }

        void StartMouseMonitoring ()
        {
            StopMouseMonitoring();

            clickMonitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.MouseMoved, (NSEvent theEvent) => {

                var currentX = theEvent.LocationInWindow.X - (Window.Frame.Width/2);
                var currentY = theEvent.LocationInWindow.Y - (Window.Frame.Height/2);
                Window.SetFrame(new CoreGraphics.CGRect(currentX, currentY, Window.Frame.Width, Window.Frame.Height), true);
                return theEvent;
            });
        }

        void StopMouseMonitoring ()
        {
            if (clickMonitor != null)
            {
                NSEvent.RemoveMonitor(clickMonitor);
                clickMonitor = null;
            }
        }

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
        }

        void Agent_NeedsRender(object sender, EventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                Image = agent.GetCurrentImage();
            });
        }

        protected override void Dispose(bool disposing)
        {
            agent.NeedsRender -= Agent_NeedsRender;
            base.Dispose(disposing);
        }
    }
}
//var json = AssemblyHelper.ReadResourceString ("clippy", "agent.json");
//var model = JsonConvert.DeserializeObject<AgentModel> (json);
//var frameSize = new CoreGraphics.CGSize (model.FrameSize[0], model.FrameSize[1]);


//var skView = new SKView {

//  ShowsFPS = true, ShowsNodeCount = true, IgnoresSiblingOrder = true,
//  Frame = View.Frame, 
//};

//View = skView;


//var scene = new Scene ();
//skView.PresentScene (scene);
//public class Scene : SKScene
//{
//  #region Constructors

//  public Scene (NSCoder coder) : base (coder)
//  {
//  }

//  public Scene () : base (new CoreGraphics.CGSize (500, 500))
//  {
//      Initialize ();
//  }

//  protected Scene (NSObjectFlag t) : base (t)
//  {
//  }

//  protected internal Scene (IntPtr handle) : base (handle)
//  {
//  }
//  #endregion

//  NSImage Resize (NSImage sourceImage, CGSize size)
//  {
//              var newImage = new NSImage(size);
//              newImage.LockFocus();
//              sourceImage.DrawInRect(
//                  new CGRect(CGPoint.Empty, size), new CGRect(CGPoint.Empty, size), NSCompositingOperation.Copy,
//                  1);
//              newImage.UnlockFocus();
//              newImage.Size = size;
//              return newImage;
//  //                NSApplication.CheckForIllegalCrossThreadCalls = false;


//              //            NSGraphicsContext.GlobalSaveGraphicsState ();


//              //            var colorSpace = CGColorSpace.CreateDeviceRGB();
//              //            CGBitmapContext c = new CGBitmapContext(IntPtr.Zero, 400, 400, 8, 400 * (colorSpace.Components + 1), colorSpace, CGImageAlphaInfo.PremultipliedLast);
//              //            NSGraphicsContext graphicsContext = NSGraphicsContext.FromBitmap(c);
//              //            NSGraphicsContext.CurrentContext = graphicsContext;


//              //            NSGraphicsContext.CurrentContext.ImageInterpolation = NSImageInterpolation.High;
//              //smallImage.LockFocus ();
//              //sourceImage.Size = size;
//              //sourceImage.DrawInRect (new CGRect (0, 0, size.Width, size.Height),
//              //    new CGRect (0, 0, size.Width, size.Height), NSCompositingOperation.Copy, 1);
//              //smallImage.UnlockFocus ();

//              //NSGraphicsContext.GlobalRestoreGraphicsState ();
//  }




//          void Initialize()
//  {
//      //var sourceImage = NSImage.ImageNamed ("0aea673c-8683-4d80-89f3-5d8e16d81007");
//                  //sourceImage.ResizingMode = NSImageResizingMode.Stretch;
//                  //sourceImage.Size = new CGSize(300, 300);

//              var sourceImage = AssemblyHelper.ReadResourceImage("clippy", "map.png");

//              //image = new NSImage(image.CGImage, new CoreGraphics.CGSize(500, 500));
//              //image.Size = new CoreGraphics.CGSize(500, 500);

//              //var image = Resize(sourceImage, new CGSize(100, 100));


//              var firstImage = sourceImage.CGImage.WithImageInRect(new CGRect(0,  sourceImage.CGImage.Height - 93, 124, 93));

//              BackgroundColor = NSColor.White;

//      var spriteSheet = SKTexture.FromImage (firstImage);

//              //spriteSheet.Size = new CGSize(300, 300);
//              //spriteSheet.Size = new CoreGraphics.CGSize (500, 500);

//              var rectangle = new CoreGraphics.CGRect (2, 2, 50, 50);
//      //var initFrame = SKTexture.FromRectangle (rectangle, spriteSheet);
//      //using (NSGraphicsContext gfxContext = NSGraphicsContext.FromBitmap (imageRep)) {

//      //  using (CGContext context = gfxContext.GraphicsPort) {


//      //      // context drawing calls
//      //      image.AddRepresentation (imageRep);

//      //  }

//      //}

//      var player = new SKSpriteNode (spriteSheet);
//      player.Color = NSColor.Red;


//      AddChild (player);

//      player.Position = new CoreGraphics.CGPoint (100, 100);
//      //player.Size = new CoreGraphics.CGSize (1000, 1000);
//      //var textures = new List<SKTexture> ();



//      //var initFrame = SKTexture.FromRectangle (new CoreGraphics.CGRect (0, 0, 124, 93), spriteSheet);

//      //var bear = new SKSpriteNode (texture: initFrame);
//      //bear.Position = new CoreGraphics.CGPoint (10, 10);
//      //scene.Add (bear);

//      //textures.Add (initFrame);
//      //textures.Add (SKTexture.FromRectangle (new CoreGraphics.CGRect (124, 0, 124, 93), spriteSheet));
//      //textures.Add (SKTexture.FromRectangle (new CoreGraphics.CGRect (248, 0, 124, 93), spriteSheet));
//      //textures.Add (SKTexture.FromRectangle (new CoreGraphics.CGRect (372, 0, 124, 93), spriteSheet));

//      //var action = SKAction.AnimateWithTextures (textures.ToArray (), 0.1f, false, true);

//      //bear.RunAction (SKAction.RepeatActionForever (action));
//  }
//}
