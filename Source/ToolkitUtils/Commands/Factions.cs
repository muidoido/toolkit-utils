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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class Factions : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage msg)
        {
            List<string> factions = Current.Game.World.factionManager.AllFactionsVisibleInViewOrder
               .Where(f => !f.IsPlayer)
               .Select(f => ResponseHelper.JoinPair(f.GetCallLabel(), f.PlayerGoodwill.ToStringWithSign()))
               .ToList();

            msg.Reply(
                (factions.Count <= 0 ? "TKUtils.Factions.None".Localize() : factions.SectionJoin()).WithHeader(
                    "WorldFactionsTab".Localize()
                )
            );
        }
    }
}
