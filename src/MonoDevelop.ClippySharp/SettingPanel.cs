/* 
 * FigmaRuntime.cs 
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
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;

namespace MonoDevelop.Assistant
{
	static class PropertySettings
	{
		const string AdioKey = "MonoDevelop.Assistant.Audio";

		public static bool AudioEnabled {
			get => PropertyService.Get<bool> (AdioKey);
			set {
				AgentService.Current.Agent.Sound = value;
				PropertyService.Set (AdioKey, value);
			}
		}
	}

	class OptionsWidget : VBox
	{
		CheckButton tokenValueEntry;
		OptionsPanel panel;
		public OptionsWidget (OptionsPanel figmaOptionsPanel)
		{
			panel = figmaOptionsPanel;

			var mainVBox = new HBox ();
			PackStart (mainVBox);

			var tokenLabel = new Label ();
			tokenLabel.Text = GettextCatalog.GetString ("Sound:");
			mainVBox.PackStart (tokenLabel, false, false, 10);
			tokenValueEntry = new CheckButton ();
			mainVBox.PackStart (tokenValueEntry, false, false, 10);

			tokenValueEntry.WidthRequest = 350;

			tokenValueEntry.Activated += NeedsStoreValue;
			tokenValueEntry.FocusOutEvent += NeedsStoreValue;

			ShowAll ();

			tokenValueEntry.Active = AgentService.Current.Agent.Sound; ;
			AgentService.Current.Agent.Sound = tokenValueEntry.State == StateType.Active;
		}

		void NeedsStoreValue (object sender, EventArgs e)
		{
			Store ();
		}

		internal void ApplyChanges ()
		{
			Store ();
		}

		void Store ()
		{
			AgentService.Current.Agent.Sound = tokenValueEntry.Active;
		}
	}

	class OptionsPanel : Ide.Gui.Dialogs.OptionsPanel
	{
		OptionsWidget widget;

		public override Control CreatePanelWidget ()
		{
			widget = new OptionsWidget (this);
			return widget;
		}


		public void SaveSdkLocationSetting (FilePath location)
		{

		}

		public override void ApplyChanges ()
		{
			widget.ApplyChanges ();
		}
	}
}
