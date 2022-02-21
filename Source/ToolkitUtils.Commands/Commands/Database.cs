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
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Registries;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class Database : CommandBase
    {
        private static readonly Dictionary<string, Category> Index = new Dictionary<string, Category>
        {
            { "weapon", Category.Weapon },
            { "weapons", Category.Weapon },
            { "gun", Category.Weapon },
            { "sword", Category.Weapon },
            { "melee", Category.Weapon },
            { "ranged", Category.Weapon },
            { "club", Category.Weapon },
            { "clubs", Category.Weapon },
            { "knife", Category.Weapon },
            { "knives", Category.Weapon },
            { "spell", Category.TMagic },
            { "spells", Category.TMagic },
            { "ability", Category.TMagic },
            { "abilities", Category.TMagic }
        };

        private static readonly List<StatDef> WeaponStats = new List<StatDef>
        {
            StatDefOf.AccuracyLong,
            StatDefOf.AccuracyMedium,
            StatDefOf.AccuracyShort
        };

        private static readonly List<StatDef> RangedWeaponStats = new List<StatDef>
        {
            StatDefOf.RangedWeapon_Cooldown,
            StatDefOf.RangedWeapon_DamageMultiplier
        };

        private static readonly List<StatDef> MeleeWeaponStats = new List<StatDef>
        {
            StatDefOf.MeleeWeapon_AverageArmorPenetration,
            StatDefOf.MeleeWeapon_AverageDPS,
            StatDefOf.MeleeWeapon_CooldownMultiplier,
            StatDefOf.MeleeWeapon_DamageMultiplier
        };

        public override async Task RunCommandAsync([NotNull] IContext context)
        {
            string[] segments = CommandFilter.Parse(context.Message).Skip(1).ToArray();
            string category = segments.FirstOrFallback("");
            string query = segments.Skip(1).FirstOrFallback("");

            if (!Index.TryGetValue(category.ToLowerInvariant(), out Category result))
            {
                query = category;
            }

            await PerformLookup(context, result, query);
        }

        private static async Task PerformWeaponLookup([NotNull] IContext context, [NotNull] string query)
        {
            var worker = ArgWorker.CreateInstance(query);

            string response;

            if (!worker.TryGetNextAsItem(out ArgWorker.ItemProxy item) || !item.IsValid() || !item.Thing.Thing.IsWeapon)
            {
                response = await "TKUtils.InvalidItemQuery".TranslateAsync(worker.GetLast());
                await context.Reply(response);

                return;
            }

            if (item.TryGetError(out string error))
            {
                await context.Reply(error);

                return;
            }

            var func = new Func<string>(() => GetItemStats(item));
            response = await func.OnMainThreadAsync();

            await context.Reply(response);
        }

        private static async Task PerformTMagicLookup(IContext context, string query)
        {
            var func = new Func<string>(() => CompatRegistry.Magic?.GetSkillDescription(context.User.Username, query));
            string response = await func.OnMainThreadAsync();

            await context.Reply(response);
        }

        private static async Task PerformLookup(IContext context, Category category, string query)
        {
            switch (category)
            {
                case Category.Weapon:
                    await PerformWeaponLookup(context, query);

                    return;
                case Category.TMagic:
                    await PerformTMagicLookup(context, query);

                    return;
            }
        }

        [NotNull]
        private static string GetItemStats([NotNull] ArgWorker.ItemProxy item)
        {
            var container = new List<string>();

            Thing thing = PurchaseHelper.MakeThing(item.Thing.Thing, item.Stuff.Thing, item.Quality);

            container.AddRange(WeaponStats.Select(s => s.ValueToString(thing.GetStatValue(s))));

            if (item.Thing.Thing.IsMeleeWeapon)
            {
                container.AddRange(MeleeWeaponStats.Select(s => s.ValueToString(thing.GetStatValue(s))));
            }

            if (item.Thing.Thing.IsRangedWeapon)
            {
                container.AddRange(RangedWeaponStats.Select(s => s.ValueToString(thing.GetStatValue(s))));
            }

            return ResponseHelper.JoinPair(item.Thing.ToString(), container.ToCommaList(true));
        }

        // ReSharper disable once InconsistentNaming
        private enum Category { Weapon, TMagic }
    }
}
