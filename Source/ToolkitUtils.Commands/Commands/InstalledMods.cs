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
using System.Threading.Tasks;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class InstalledMods : CommandBase
    {
        public override async Task RunCommandAsync([NotNull] IContext context)
        {
            int totalMods = Data.Mods.Length;
            var container = new string[totalMods];

            for (var i = 0; i < totalMods; i++)
            {
                container[i] = await FormatModAsync(context, Data.Mods[i]);
            }

            await context.Reply(container.ToCommaList(true).WithHeader($"Toolkit v{Toolkit.Mod.Version}"));
        }

        [NotNull]
        private static async Task<string> FormatModAsync(IContext context, [NotNull] ModItem mod)
        {
            if (!TkSettings.VersionedModList || string.IsNullOrEmpty(mod.Version))
            {
                return await DecorateMod(context, mod);
            }

            return $"{DecorateMod(context, mod)} (v{mod.Version})";
        }

        [NotNull]
        private static Task<string> DecorateMod(IContext context, [NotNull] ModItem mod)
        {
            if (!TkSettings.DecorateMods || !string.Equals(mod.Author, "sirrandoo", StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.FromResult(mod.Name);
            }

            return Task.FromResult((context.UseEmojis ? "\u2605" : "*") + mod.Name);
        }
    }
}
