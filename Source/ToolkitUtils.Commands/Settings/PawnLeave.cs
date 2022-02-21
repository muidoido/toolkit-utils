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
using CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Settings
{
    public class PawnLeave : ICommandSettings
    {
        public void Draw(Rect region)
        {
            var listing = new Listing_Standard();

            listing.Begin(region);

            (Rect labelRect, Rect fieldRect) = listing.Split();
            UiHelper.Label(labelRect, "TKUtils.Abandon.Method.Label".TranslateSimple());
            listing.DrawDescription("TKUtils.Abandon.Method.Description".TranslateSimple());

            if (Widgets.ButtonText(fieldRect, $"TKUtils.Abandon.Method.{TkSettings.LeaveMethod}".TranslateSimple()))
            {
                Find.WindowStack.Add(new FloatMenu(TkSettings.LeaveMenuOptions));
            }

            if (!TkSettings.LeaveMethod.EqualsIgnoreCase(nameof(LeaveMethod.Thanos)))
            {
                listing.CheckboxLabeled("TKUtils.Abandon.Gear.Label".TranslateSimple(), ref TkSettings.DropInventory);
                listing.DrawDescription("TKUtils.Abandon.Gear.Description".TranslateSimple());
            }

            listing.End();
        }

        public void Save()
        {
            TkUtils.Instance.WriteSettings();
        }
    }
}
