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
using SirRandoo.ToolkitUtils.Registries;
using SirRandoo.ToolkitUtils.Utils;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnFix : CommandBase
    {
        public override async Task RunCommandAsync([NotNull] IContext context)
        {
            if (!PurchaseHelper.TryGetPawn(context.User.Username, out Pawn pawn))
            {
                await context.Reply(await "TKUtils.NoPawn".TranslateSimpleAsync());

                return;
            }

            var func = new Func<bool>(() => SetPawnName(pawn, context.User.Username));
            await func.OnMainThreadAsync();

            await context.Reply(await "TKUtils.PawnFix".TranslateSimpleAsync());
        }

        private static bool SetPawnName([NotNull] Pawn pawn, string name)
        {
            if (!(pawn.Name is NameTriple triple) || string.Equals(triple.Nick, name, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            pawn.Name = new NameTriple(triple.First ?? string.Empty, name, triple.Last ?? string.Empty);

            return true;
        }
    }
}
