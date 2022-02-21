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
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Registries;
using SirRandoo.ToolkitUtils.Utils;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class Factions : CommandBase
    {
        public override async Task RunCommandAsync([NotNull] IContext context)
        {
            var func = new Func<string[]>(GetFactionNames);
            string[] factions = await func.OnMainThreadAsync();

            await context.Reply(factions.ToCommaList(true));
        }

        [NotNull]
        private static string[] GetFactionNames()
        {
            if (Find.FactionManager == null)
            {
                return Array.Empty<string>();
            }

            var container = new List<string>();

            foreach (Faction faction in Find.FactionManager.AllFactionsVisibleInViewOrder)
            {
                if (faction.IsPlayer)
                {
                    continue;
                }

                container.Add($"{faction.GetCallLabel()}: {faction.PlayerGoodwill.ToStringWithSign()}");
            }

            return container.ToArray();
        }
    }
}
