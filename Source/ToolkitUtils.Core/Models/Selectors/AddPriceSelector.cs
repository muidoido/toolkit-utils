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
using CommonLib.Helpers;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Enums;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class AddPriceSelector : ISelectorBase<TraitItem>
    {
        private int _addPrice;
        private string _addPriceBuffer = "0";
        private string _addPriceText;
        private ComparisonType _comparison = ComparisonType.Equal;
        private List<FloatMenuOption> _comparisonOptions;

        public void Prepare()
        {
            _addPriceText = "TKUtils.Fields.AddPrice".TranslateSimple();

            _comparisonOptions = Data.ComparisonTypes.Select(
                    i => new FloatMenuOption(
                        i.AsOperator(),
                        () =>
                        {
                            _comparison = i;
                            Dirty.Set(true);
                        }
                    )
                )
               .ToList();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.Split(0.75f);
            UiHelper.Label(label, _addPriceText);

            (Rect button, Rect input) = field.Split(0.3f);

            if (Widgets.ButtonText(button, _comparison.AsOperator()))
            {
                Find.WindowStack.Add(new FloatMenu(_comparisonOptions));
            }

            if (!UiHelper.NumberField(input, ref _addPrice, ref _addPriceBuffer, out int newCost))
            {
                return;
            }

            _addPrice = newCost;
            _addPriceBuffer = newCost.ToString();
            Dirty.Set(true);
        }

        public ObservableProperty<bool> Dirty { get; set; }

        public bool IsVisible([NotNull] TableSettingsItem<TraitItem> item)
        {
            if (!item.Data.Enabled)
            {
                return false;
            }

            switch (_comparison)
            {
                case ComparisonType.Greater:
                    return item.Data.CostToAdd > _addPrice;
                case ComparisonType.Equal:
                    return item.Data.CostToAdd == _addPrice;
                case ComparisonType.Less:
                    return item.Data.CostToAdd < _addPrice;
                case ComparisonType.GreaterEqual:
                    return item.Data.CostToAdd >= _addPrice;
                case ComparisonType.LessEqual:
                    return item.Data.CostToAdd <= _addPrice;
                default:
                    return false;
            }
        }

        public string Label => "TKUtils.Fields.AddPrice".TranslateSimple();
    }
}
