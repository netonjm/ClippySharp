using Foundation;
using System;
using UIKit;

namespace ClippySharp.Sample.iOS
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		class AnimationsModel : UIPickerViewModel
		{
			readonly string[] names;

			public AnimationsModel (string[] names)
			{
				this.names = names;
			}

			public override nint GetComponentCount (UIPickerView pickerView)
			{
				return 1;
			}

			public override nint GetRowsInComponent (UIPickerView pickerView, nint component)
			{
				return names.Length;
			}

			public override string GetTitle (UIPickerView pickerView, nint row, nint component)
			{
				return names[row];
			}

			public event EventHandler<int> CustomSelectionChanged;

			public override void Selected (UIPickerView pickerView, nint row, nint component)
			{
				CustomSelectionChanged?.Invoke (this, (int) row);
			}

			public override nfloat GetComponentWidth (UIPickerView picker, nint component)
			{
					return 240f;
			}

			public override nfloat GetRowHeight (UIPickerView picker, nint component)
			{
				return 40f;
			}
		}

		UIPickerView pickerView;
		string[] animations;
		AgentView agentView;
		UISegmentedControl segmentedControl;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AgentEnvirontment.Current.Initialize (new AgentDelegate (), new SoundPlayer ());

			// Perform any additional setup after loading the view, typically from a nib.

			var container = View;

			var screen = UIScreen.Screens[0].Bounds;

			segmentedControl = new UISegmentedControl (AgentEnvirontment.Current.GetAgents ());
			View.AddSubview (segmentedControl);
			segmentedControl.Frame = new CoreGraphics.CGRect (0, 0, screen.Width, 50);

			pickerView = new UIPickerView (new CoreGraphics.CGRect (0, 53, screen.Width, 200));
			View.AddSubview (pickerView);

			var size = 140;

			var positionX = (screen.Width / 2) - (size / 2);
			var positionY = 260;  //(screen.Height / 2) - (size / 2);

			agentView = new AgentView (new CoreGraphics.CGRect (positionX, positionY, size, size));
			View.AddSubview (agentView);

			agentView.TapPressed += (sender, e) => {
				((AgentView)sender).Agent.Animate ();
			};

			segmentedControl.ValueChanged += (sender, e) => {
				ItemChanged ();
			};

			segmentedControl.SelectedSegment = 0;
			ItemChanged ();
		}

		AnimationsModel model;

		Agent currentAgent;

		void ItemChanged ()
		{
			if (model != null)
				model.CustomSelectionChanged -= Model_SelectedChanged;

			var title = segmentedControl.TitleAt (segmentedControl.SelectedSegment);
			currentAgent = new Agent (title);
			agentView.ConfigureAgent (currentAgent);
			currentAgent.Animate ();

			animations = currentAgent.GetAnimations ();
			model = new AnimationsModel (animations);
			pickerView.Model = model;
			pickerView.ReloadAllComponents ();
			model.CustomSelectionChanged += Model_SelectedChanged;
		}

		void Model_SelectedChanged (object sender, int e)
		{
			currentAgent?.Play (animations[e]);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}