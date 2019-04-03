/* 
 * FigmaDisplayBinding.cs 
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using AppKit;
using ClippySharp;
using CoreGraphics;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.WelcomePage;
using System.Linq;
using MonoDevelop.Components.Mac;

namespace MonoDevelop.Assistant
{
	class AgentService : IDisposable
	{
		public Agent Agent { get; private set; }

		AgentView agentView;
		NSWindow window;

		public void SetAgent (string name, bool initialAnimation)
		{
			Agent = new Agent (name) {
				Sound = PropertySettings.AudioEnabled
			};
			agentView.ConfigureAgent (Agent);

			if (initialAnimation)
				Agent.Animate ();
		}

		AgentService ()
		{

			var xPos = NSScreen.MainScreen.Frame.Width / 2;
			var yPos = NSScreen.MainScreen.Frame.Height / 2;

			agentView = new AgentView ();

			SetAgent (PropertySettings.AgentSelected ?? "clippy", false);

			window = new NSWindow (new CGRect (xPos, yPos, 100, 100), NSWindowStyle.Borderless, NSBackingStore.Buffered, false) {
				Title = "ClippySharp",
				IsOpaque = false,
				BackgroundColor = NSColor.FromRgba (red: 1, green: 1, blue: 1f, alpha: 0)
			};
			window.Level = NSWindowLevel.ModalPanel;
			window.MovableByWindowBackground = true;
			window.ContentView = agentView;

			window.IsVisible = true;

			Ide.MessageService.PlaceDialog (window, Ide.MessageService.RootWindow);

			IdeApp.Workbench.RootWindow.Hidden += RootWindow_Hidden;

			IdeApp.Workbench.RootWindow.Shown += RootWindow_Shown;

			IdeApp.ProjectOperations.StartClean += ProjectOperations_StartClean;
			IdeApp.ProjectOperations.EndBuild += ProjectOperations_EndBuild;
			IdeApp.ProjectOperations.EndClean += ProjectOperations_Finished;
			IdeApp.ProjectOperations.StartBuild += ProjectOperations_StartBuild;
		}

		void RootWindow_Shown (object sender, EventArgs e)
		{
			var parent = GtkMacInterop.GetNSWindow (IdeApp.Workbench.RootWindow);
			parent.AddChildWindow (window, NSWindowOrderingMode.Above);

			OrderFront ();
			Agent.Play ("Greeting");
			Agent.Sound = PropertySettings.AudioEnabled;
		}

		void RootWindow_Hidden (object sender, EventArgs e)
		{
			var parent = GtkMacInterop.GetNSWindow (IdeApp.Workbench.RootWindow);
			parent.RemoveChildWindow (window);

			OrderFront ();
			Agent.Play ("GoodBye");
			Agent.Sound = false;
		}

		void ProjectOperations_StartClean (object sender, Projects.CleanEventArgs args)
		{
			OrderFront ();
			Agent.Play ("EmptyTrash");
		}

		void ProjectOperations_StartBuild (object sender, Projects.BuildEventArgs args)
		{
			OrderFront ();
			Agent.Play ("Processing");
		}

		void ProjectOperations_Finished (object sender, Projects.CleanEventArgs args)
		{
			FinishedSuccessful ();
		}

		void ProjectOperations_EndBuild (object sender, Projects.BuildEventArgs args) 
		{
			if (args.Success) {
				FinishedSuccessful ();
			} else {
				OrderFront ();
				Agent.Play ("Wave");
			}
		}

		void FinishedSuccessful ()
		{
			OrderFront ();
			Agent.Play ("Congratulate");
		}

		void OrderFront ()
		{

			window.OrderFront (null);
		}

		public void Initialize ()
		{

		}
		//EmptyTrash
		//Searching
		//Save
		public void Dispose ()
		{
			Agent.Play ("GoodBye");

			IdeApp.ProjectOperations.StartBuild += ProjectOperations_StartBuild;

			Agent.Dispose ();
			window = null;
			agentView = null;
		}

		static AgentService current;
		public static AgentService Current {
			get {
				if (current == null) {
					current = new AgentService ();
				}
				return current;
			}
		}
	}
}


