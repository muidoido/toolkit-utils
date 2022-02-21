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

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class TechnologySelector : ISelectorBase<ThingItem>
    {
        private ComparisonTypes _comparison = ComparisonTypes.LessEqual;
        private List<FloatMenuOption> _comparisonOptions;
        private TechLevel _techLevel = TechLevel.Industrial;
        private List<FloatMenuOption> _techLevelOptions;
        private string _techLevelText;

        public ObservableProperty<bool> Dirty { get; set; }

        public void Prepare()
        {
            _techLevelText = "TKUtils.Fields.Technology".TranslateSimple();

            _techLevelOptions = Data.TechLevels.Where(i => i != TechLevel.Undefined)
               .Select(
                    i => new FloatMenuOption(
                        $"TechLevel_{i}".TranslateSimple(),
                        () =>
                        {
                            _techLevel = i;
                            Dirty.Set(true);
                        }
                    )
                )
               .ToList();

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
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, _techLevelText);

            (Rect comp, Rect tech) = field.ToForm(0.3f);

            if (Widgets.ButtonText(comp, _comparison.AsOperator()))
            {
                Find.WindowStack.Add(new FloatMenu(_comparisonOptions));
            }

            if (Widgets.ButtonText(tech, $"TechLevel_{_techLevel}".Localize().CapitalizeFirst()))
            {
                Find.WindowStack.Add(new FloatMenu(_techLevelOptions));
            }
        }

        public bool IsVisible([NotNull] TableSettingsItem<ThingItem> item)
        {
            if (item.Data.Thing?.techLevel == null)
            {
                return false;
            }

            switch (_comparison)
            {
                case ComparisonTypes.Greater:
                    return (int)item.Data.Thing.techLevel > (int)_techLevel;
                case ComparisonTypes.Equal:
                    return (int)item.Data.Thing.techLevel == (int)_techLevel;
                case ComparisonTypes.Less:
                    return (int)item.Data.Thing.techLevel < (int)_techLevel;
                case ComparisonTypes.GreaterEqual:
                    return (int)item.Data.Thing.techLevel >= (int)_techLevel;
                case ComparisonTypes.LessEqual:
                    return (int)item.Data.Thing.techLevel <= (int)_techLevel;
                default:
                    return false;
            }
        }

        public string Label => "TKUtils.Fields.Technology".TranslateSimple();
    }
}
