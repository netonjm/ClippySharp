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
using ClippySharp;
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

		const string AgentSelectedKey = "MonoDevelop.Assistant.AgentSelected";
		public static string AgentSelected {
			get => PropertyService.Get<string> (AgentSelectedKey);
			set {
				AgentService.Current.SetAgent (value, true);
				PropertyService.Set (AgentSelectedKey, value);
			}
		}
	}

	class OptionsWidget : VBox
	{
		CheckButton checkEnabledButton;
		OptionsPanel panel;
		ComboBox agentComboBox;

		string GetSelectedValue (ComboBox comboBox)
		{
			var row = new GLib.Value ();
			agentComboBox.GetActiveIter (out TreeIter iter);
			agentComboBox.Model.GetValue (iter, 0, ref row);
			return row.Val as string;
		}

		void SetSelectedValue (ComboBox comboBox, string value)
		{
			comboBox.Model.GetIterFirst (out TreeIter iter);
			do {
				GLib.Value thisRow = new GLib.Value ();
				comboBox.Model.GetValue (iter, 0, ref thisRow);
				if ((thisRow.Val as string).Equals (value)) {
					comboBox.SetActiveIter (iter);
					break;
				}
			} while (comboBox.Model.IterNext (ref iter));
		}

		public OptionsWidget (OptionsPanel figmaOptionsPanel)
		{
			panel = figmaOptionsPanel;

			var mainVBox = new HBox ();
			PackStart (mainVBox);

			var agentLabel = new Label ();
			agentLabel.Text = GettextCatalog.GetString ("Agent:");
			mainVBox.PackStart (agentLabel, false, false, 10);

			var agents = AgentContext.Current.GetAgents ();

			agentComboBox = new ComboBox (agents);
			mainVBox.PackStart (agentComboBox, true, true, 10);

			var soundLabel = new Label {
				Text = GettextCatalog.GetString ("Sound:")
			};
			mainVBox.PackStart (soundLabel, false, false, 10);
			checkEnabledButton = new CheckButton ();
			mainVBox.PackStart (checkEnabledButton, false, false, 10);
			checkEnabledButton.WidthRequest = 350;

			ShowAll ();


			SetSelectedValue (agentComboBox, PropertySettings.AgentSelected);

			checkEnabledButton.Active = AgentService.Current.Agent.Sound; ;

			agentComboBox.Changed += NeedsStoreValue;
			checkEnabledButton.Activated += NeedsStoreValue;
			checkEnabledButton.FocusOutEvent += NeedsStoreValue;
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
			PropertySettings.AudioEnabled = checkEnabledButton.Active;

			var selectedValue = GetSelectedValue (agentComboBox);
			if (!string.IsNullOrEmpty (selectedValue)) {
				PropertySettings.AgentSelected = selectedValue;
			}
		}

		public override void Dispose ()
		{
			agentComboBox.Changed -= NeedsStoreValue;
			checkEnabledButton.Activated -= NeedsStoreValue;
			checkEnabledButton.FocusOutEvent -= NeedsStoreValue;
			base.Dispose ();
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
