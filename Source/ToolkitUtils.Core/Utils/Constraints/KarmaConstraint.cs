// ToolkitUtils.Core
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
using SirRandoo.ToolkitUtils.Enums;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class KarmaConstraint : ComparableConstraint
    {
        private readonly string _labelText;
        private string _buffer;
        private int _karma;
        private bool _valid;

        public KarmaConstraint()
        {
            _labelText = "TKUtils.PurgeMenu.Karma".TranslateSimple().CapitalizeFirst();
            _buffer = "0";
            _valid = true;
        }

        public override void Draw(Rect canvas)
        {
            (Rect labelRect, Rect fieldRect) = canvas.Split(0.7f);
            (Rect buttonRect, Rect inputRect) = fieldRect.Split(0.25f);

            UiHelper.Label(labelRect, _labelText);
            DrawButton(buttonRect);

            GUI.backgroundColor = _valid ? Color.white : Color.red;

            if (!UiHelper.TextField(inputRect, _buffer, out string result))
            {
                GUI.backgroundColor = Color.white;

                return;
            }

            GUI.backgroundColor = Color.white;
            _buffer = result;

            if (int.TryParse(result, out int parsed))
            {
                _karma = parsed;
                _valid = true;
            }
            else
            {
                _valid = false;
            }
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            switch (Comparison)
            {
                case ComparisonType.Equal:
                    return viewer.karma == _karma;
                case ComparisonType.Greater:
                    return viewer.karma > _karma;
                case ComparisonType.Less:
                    return viewer.karma < _karma;
                case ComparisonType.GreaterEqual:
                    return viewer.karma >= _karma;
                case ComparisonType.LessEqual:
                    return viewer.karma <= _karma;
                default:
                    return false;
            }
        }
    }
}
