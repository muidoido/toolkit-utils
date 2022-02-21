﻿// ToolkitUtils.Core
// Copyright (C) 2021  SirRandoo
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CommonLib.Helpers;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Windows
{
    [StaticConstructorOnStartup]
    public class CommandEditorDialog : Window_CommandEditor
    {
        private static readonly MethodInfo RemoveMethod;

        private readonly Command _command;
        private readonly ICommandSettings _settings;
        private string _adminText;
        private string _anyoneText;
        private string _commandLabel;
        private bool _confirmed;

        private bool _confirming;
        private string _confirmText;
        private float _confirmTextWidth;
        private string _deletedText;
        private float _deletedTextWidth;
        private string _deleteText;
        private float _deleteTextWidth;
        private string _disableText;
        private float _disableTextWidth;
        private TextEditor _editor;
        private Rect _editorPosition;
        private string _enableText;
        private float _enableTextWidth;
        private string _headerText;
        private bool _invalidId;
        private string _moderatorText;
        private Vector2 _scrollPos = Vector2.zero;
        private string _settingsText;
        private float _settingsTextWidth;
        private bool _showingSettings;
        private string _tagTooltip;
        private List<FloatMenuOption> _userLevelOptions;
        private string _userLevelText;

        static CommandEditorDialog()
        {
            RemoveMethod = AccessTools.Method(typeof(DefDatabase<Command>), "Remove");
        }

        public CommandEditorDialog([NotNull] Command command) : base(command)
        {
            _command = command;

            var ext = command.GetModExtension<CommandExtension>();

            if (ext?.SettingsHandler != null)
            {
                _settings = Activator.CreateInstance(ext.SettingsHandler) as ICommandSettings;
            }
        }

        public override void PostOpen()
        {
            base.PostOpen();

            GetTranslations();

            _userLevelOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption(_anyoneText, () => ChangeUserLevel(UserLevel.Anyone)),
                new FloatMenuOption(_moderatorText, () => ChangeUserLevel(UserLevel.Moderator)),
                new FloatMenuOption(_adminText, () => ChangeUserLevel(UserLevel.Admin))
            };

            _invalidId = _command.command.NullOrEmpty() || _command.command?.TrimStart(TkSettings.Prefix.ToCharArray()).NullOrEmpty() == true;
        }

        public override void DoWindowContents(Rect region)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GUI.BeginGroup(region);

            if (!_showingSettings)
            {
                var buttonBar = new Rect(0f, 0f, region.width, Text.SmallFontHeight);
                Rect content = new Rect(0f, Text.SmallFontHeight * 2, region.width, region.height - Text.SmallFontHeight).ContractedBy(20f);

                GUI.BeginGroup(buttonBar);
                DrawButtonBar(buttonBar.AtZero());
                GUI.EndGroup();

                GUI.BeginGroup(content);
                DrawContent(content.AtZero());
                GUI.EndGroup();
            }
            else
            {
                _settings?.Draw(region);
            }

            GUI.EndGroup();
        }

        private void DrawContent(Rect region)
        {
            var listing = new Listing_Standard();

            GUI.BeginGroup(region);
            listing.Begin(region);

            (Rect labelRect, Rect fieldRect) = listing.Split(0.6f);
            UiHelper.Label(labelRect, _commandLabel);

            GUI.color = _invalidId ? new Color(1f, 0.53f, 0.76f) : Color.white;

            if (UiHelper.TextField(fieldRect, $"{TkSettings.Prefix}{_command.command}", out string newContent))
            {
                if (newContent.ToToolkit().Length - TkSettings.Prefix.Length < 0)
                {
                    _invalidId = true;
                }
                else
                {
                    _command.command = newContent.Substring(TkSettings.Prefix.Length).ToToolkit();
                    _invalidId = false;
                }
            }

            GUI.color = Color.white;

            if (_command.isCustomMessage)
            {
                DrawCustomFields(listing);
            }

            listing.End();
            GUI.EndGroup();
        }

        private void DrawCustomFields([NotNull] Listing listing)
        {
            (Rect levelLabel, Rect levelField) = listing.Split(0.6f);
            UiHelper.Label(levelLabel, _userLevelText);

            if (Widgets.ButtonText(levelField, GetInferredUserLevelText()))
            {
                Find.WindowStack.Add(new FloatMenu(_userLevelOptions));
            }

            listing.Gap(24f);

            if (UiHelper.FieldButton(listing.GetRect(Text.SmallFontHeight), Textures.QuestionMark, _tagTooltip))
            {
                Application.OpenURL("https://storytoolkit.fandom.com/wiki/Commands#Tags");
            }

            _editorPosition = listing.GetRect(Text.SmallFontHeight * 11f);

            GUI.BeginGroup(_editorPosition);
            _command.outputMessage = Widgets.TextAreaScrollable(_editorPosition.AtZero(), _command.outputMessage, ref _scrollPos);
            _editor ??= GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.GetControlID(FocusType.Keyboard, _editorPosition)) as TextEditor;
            GUI.EndGroup();
        }

        public override void OnAcceptKeyPressed()
        {
            if (GUIUtility.keyboardControl <= 0 || _editor == null)
            {
                base.OnAcceptKeyPressed();

                return;
            }

            _command.outputMessage += "\n";
            _editor.MoveDown();
            Event.current.Use();
        }

        public override void OnCancelKeyPressed()
        {
            if (GUIUtility.keyboardControl <= 0 || _editor == null)
            {
                base.OnCancelKeyPressed();

                return;
            }

            GUIUtility.keyboardControl = 0;
            Event.current.Use();
        }

        public override void Close(bool doCloseSound = true)
        {
            if (_showingSettings)
            {
                _showingSettings = false;

                return;
            }

            base.Close(doCloseSound);
        }

        public override void PostClose()
        {
            base.PostClose();

            if (TkSettings.Offload)
            {
                Task.Run(async () => await Data.DumpCommandsAsync()).ConfigureAwait(false);
            }
            else
            {
                Data.DumpCommands();
            }
        }

        private void DrawButtonBar(Rect region)
        {
            float width = Mathf.Max(_deleteTextWidth, _confirmTextWidth, _enableTextWidth, _disableTextWidth, _deletedTextWidth, _settingsTextWidth);

            var buttonRect = new Rect(region.x + region.width - width, region.y, width, Text.SmallFontHeight);

            if (_command.isCustomMessage)
            {
                DrawCustomCommandButtons(buttonRect);
                buttonRect = buttonRect.Shift(Direction8Way.West, 0f);
            }

            if (Widgets.ButtonText(buttonRect, _command.enabled ? _disableText : _enableText))
            {
                _command.enabled = !_command.enabled;
            }

            if (_settings != null)
            {
                buttonRect = buttonRect.Shift(Direction8Way.West, 0f);

                if (Widgets.ButtonText(buttonRect, _settingsText))
                {
                    GUIUtility.keyboardControl = 0;
                    _showingSettings = true;
                }
            }

            var headerRect = new Rect(0f, 0f, region.width - buttonRect.width * 2 - 5f, Text.SmallFontHeight);
            UiHelper.Label(headerRect, _headerText);
        }

        private void DrawCustomCommandButtons(Rect buttonRect)
        {
            if (!_confirmed && Widgets.ButtonText(buttonRect, _confirming ? _confirmText : _deleteText))
            {
                _confirming = !_confirming;
                _confirmed = _confirming == false;

                if (_confirmed)
                {
                    ToolkitSettings.CustomCommandDefs.Remove(_command.defName);
                    RemoveMethod.Invoke(typeof(DefDatabase<Command>), new object[] { _command });
                }
            }

            if (_confirmed)
            {
                UiHelper.Label(buttonRect, _deletedText, new Color(1f, 0.53f, 0.76f));
            }
        }

        private void ChangeUserLevel(UserLevel level)
        {
            _command.requiresAdmin = level == UserLevel.Admin;
            _command.requiresMod = level == UserLevel.Moderator;
        }

        private string GetInferredUserLevelText()
        {
            if (_command.requiresAdmin)
            {
                return _adminText;
            }

            return _command.requiresMod ? _moderatorText : _anyoneText;
        }

        private void GetTranslations()
        {
            _headerText = "TKUtils.CommandEditor.Header".Translate((_command.label ?? _command.defName).CapitalizeFirst());
            _commandLabel = "TKUtils.Fields.Command".TranslateSimple();
            _deleteText = "TKUtils.Buttons.Delete".TranslateSimple();
            _enableText = "TKUtils.Buttons.Enable".TranslateSimple();
            _disableText = "TKUtils.Buttons.Disable".TranslateSimple();
            _confirmText = "TKUtils.Buttons.AreYouSure".TranslateSimple();
            _deletedText = "TKUtils.Headers.RestartRequired".TranslateSimple();
            _anyoneText = "TKUtils.CommandEditor.UserLevel.Anyone".TranslateSimple();
            _moderatorText = "TKUtils.CommandEditor.UserLevel.Moderator".TranslateSimple();
            _adminText = "TKUtils.CommandEditor.UserLevel.Admin".TranslateSimple();
            _userLevelText = "TKUtils.Fields.UserLevel".TranslateSimple();
            _tagTooltip = "TKUtils.CommandEditorTooltips.Tags".TranslateSimple();
            _settingsText = "TKUtils.Buttons.Settings".TranslateSimple();

            GameFont cache = Text.Font;
            Text.Font = GameFont.Small;
            _deleteTextWidth = Text.CalcSize(_deleteText).x;
            _confirmTextWidth = Text.CalcSize(_confirmText).x;
            _enableTextWidth = Text.CalcSize(_enableText).x;
            _disableTextWidth = Text.CalcSize(_disableText).x;
            _deletedTextWidth = Text.CalcSize(_deletedText).x;
            _settingsTextWidth = Text.CalcSize(_settingsText).x;
            Text.Font = cache;
        }

        private enum UserLevel { Anyone, Moderator, Admin }
    }
}
