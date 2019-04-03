using AppKit;
using ClippySharp;
using CoreGraphics;
using System.Linq;

namespace ClippyTest
{
    static class MainClass
    {
		static AgentView agentView;
		static NSPopUpButton agentPopupButton;
		static NSPopUpButton animationPopupButton;

		static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;

            var xPos = NSScreen.MainScreen.Frame.Width / 2;
            var yPos = NSScreen.MainScreen.Frame.Height / 2;

            var mainWindow = new NSWindow(new CGRect(xPos, yPos, 200, 150), NSWindowStyle.Borderless, NSBackingStore.Buffered, false)
            {
                Title = "ClippySharp",

                IsOpaque = false,
                BackgroundColor = NSColor.FromRgba(red: 1, green: 1, blue: 1f, alpha: 0.5f)
            };

            mainWindow.MovableByWindowBackground = true;

            var stackView = new NSStackView()
            {
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Distribution = NSStackViewDistribution.Fill
            };

            mainWindow.ContentView = stackView;

            agentPopupButton = new NSPopUpButton();
            stackView.AddArrangedSubview(agentPopupButton);

			foreach (var item in AgentContext.Current.GetAgents ()) {
				agentPopupButton.AddItem (item);
			}

			animationPopupButton = new NSPopUpButton ();
			stackView.AddArrangedSubview (animationPopupButton);

            agentView = new AgentView ();
            stackView.AddArrangedSubview(agentView);

			agentPopupButton.Activated += AgentPopupButton_Activated;

			animationPopupButton.Activated += AnimationPopupButton_Activated;

			AgentPopupButton_Activated (null, null);

			mainWindow.MakeKeyAndOrderFront(null);
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
		}

		static void AnimationPopupButton_Activated (object sender, System.EventArgs e)
		{
			var animation = animationPopupButton.TitleOfSelectedItem;
			agentView.Agent.Play (animation);
		}

		static void AgentPopupButton_Activated (object sender, System.EventArgs e)
		{
			var agentName = agentPopupButton.TitleOfSelectedItem;
			var agent = new Agent (agentName);
			agentView.ConfigureAgent (agent);

			agent.Animate ();

			animationPopupButton.RemoveAllItems ();
			foreach (var animation in agent.GetAnimations ()) {
				//fill with all animation
				animationPopupButton.AddItem (animation);
			}
		}
	}
}
