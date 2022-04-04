﻿// ToolkitUtils
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

using CommonLib.Helpers;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class AnimalSelector : ISelectorBase<ThingItem>
    {
        private string _animalText;
        private bool _state = true;
        public ObservableProperty<bool> Dirty { get; set; }

        public void Prepare()
        {
            _animalText = "TKUtils.Fields.Animal".TranslateSimple();
        }

        public void Draw(Rect canvas)
        {
            if (UiHelper.LabeledPaintableCheckbox(canvas, _animalText, ref _state))
            {
                Dirty.Set(true);
            }
        }

        public bool IsVisible([NotNull] TableSettingsItem<ThingItem> item) => item.Data.Thing != null && item.Data.Thing.race.Animal;

        public string Label => "TKUtils.Fields.Animal".TranslateSimple();
    }
}
