using AppKit;
using CoreGraphics;
using System.Linq;

namespace ClippyTest
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;

            var xPos = NSScreen.MainScreen.Frame.Width / 2;
            var yPos = NSScreen.MainScreen.Frame.Height / 2;

            var mainWindow = new NSWindow(new CGRect(xPos, yPos, 300, 368), NSWindowStyle.Borderless | NSWindowStyle.Closable, NSBackingStore.Buffered, false);
            mainWindow.Title = "ClippySharp";

            mainWindow.IsOpaque = false;
            mainWindow.BackgroundColor = NSColor.FromRgba(red: 1, green: 1, blue: 1f, alpha: 1);

            var stackView = new NSStackView()
            {
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Alignment = NSLayoutAttribute.Top,
                Distribution = NSStackViewDistribution.Fill
            };

            mainWindow.ContentView = stackView;

            var horizontalStackView = new NSStackView()
            {
                Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
                Alignment = NSLayoutAttribute.CenterY,
                Distribution = NSStackViewDistribution.Fill
            };
            stackView.AddArrangedSubview(horizontalStackView);

            var popUpButton = new NSPopUpButton();
            horizontalStackView.AddArrangedSubview(popUpButton);

            var agent = new Agent("clippy");
            var agentView = new AgentView(agent);
            stackView.AddArrangedSubview(agentView);

            agent.Animate();

            foreach (var animation in agent.Animator.Animations)
            {
                //fill with all animation
                popUpButton.AddItem(animation.Name);
            }

            popUpButton.Activated += (sender, e) =>
            {
                var animation = popUpButton.TitleOfSelectedItem;
                agent.Play(animation);
            };

            mainWindow.MakeKeyAndOrderFront(null);
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }
    }
}
