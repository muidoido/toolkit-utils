// ToolkitUtils
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
using CommonLib.Helpers;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class PromptDialog : Window
    {
        private readonly Action _cancelAction;
        private readonly Action _closeAction;
        private readonly Action _confirmAction;
        private readonly string _message;
        private string _cancelText;
        private string _confirmText;


        public PromptDialog(string message, Action onConfirm, Action onCancel, [CanBeNull] Action onClose = null)
        {
            _message = message;

            _closeAction = onClose;
            _cancelAction = onCancel;
            _confirmAction = onConfirm;
        }

        public PromptDialog(string title, string message, Action onConfirm, Action onCancel, [CanBeNull] Action onClose = null) : this(message, onConfirm, onCancel, onClose)
        {
            optionalTitle = title;
        }

        public override void PostOpen()
        {
            _confirmText = "TKUtils.Buttons.Confirm".TranslateSimple();
            _cancelText = "TKUtils.Buttons.Cancel".TranslateSimple();
        }

        public override void DoWindowContents(Rect region)
        {
            float btnHeight = Mathf.CeilToInt(Text.SmallFontHeight * 1.25f);
            var messageRect = new Rect(0f, 0f, region.width, region.height - btnHeight);
            var buttonRect = new Rect(0f, region.height - btnHeight, region.width, btnHeight);

            GUI.BeginGroup(region);

            GUI.BeginGroup(messageRect);
            UiHelper.Label(messageRect, _message, TextAnchor.MiddleCenter);
            GUI.EndGroup();

            GUI.BeginGroup(buttonRect);
            DrawButtons(buttonRect.AtZero());
            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawButtons(Rect region)
        {
            var templateRect = new Rect(region.width - CloseButSize.x, 0f, CloseButSize.x, region.height);

            if (Widgets.ButtonText(templateRect, _cancelText))
            {
                _cancelAction();
                Close();
            }

            if (!Widgets.ButtonText(templateRect.Shift(Direction8Way.West), _confirmText))
            {
                return;
            }

            _confirmAction();
            Close();
        }

        public override void PostClose()
        {
            base.PostClose();
            _closeAction();
        }
    }
}
