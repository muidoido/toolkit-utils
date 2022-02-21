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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class Lookup : CommandBase
    {
        public enum Category
        {
            Item,
            Event,
            Kind,
            Disease,
            Animal,
            Skill,
            Trait,
            Mod
        }

        internal static readonly Dictionary<string, Category> Index = new Dictionary<string, Category>
        {
            { "item", Category.Item },
            { "items", Category.Item },
            { "incident", Category.Event },
            { "incidents", Category.Event },
            { "event", Category.Event },
            { "events", Category.Event },
            { "pawn", Category.Kind },
            { "pawns", Category.Kind },
            { "race", Category.Kind },
            { "races", Category.Kind },
            { "kinds", Category.Kind },
            { "kind", Category.Kind },

            // ReSharper disable once StringLiteralTypo
            { "pawnkinds", Category.Kind },
            { "disease", Category.Disease },
            { "diseases", Category.Disease },
            { "animal", Category.Animal },
            { "animals", Category.Animal },
            { "skill", Category.Skill },
            { "skills", Category.Skill },
            { "trait", Category.Trait },
            { "traits", Category.Trait },
            { "mods", Category.Mod },
            { "mod", Category.Mod }
        };

        public override async Task RunCommandAsync([NotNull] IContext context)
        {
            string[] segments = CommandFilter.Parse(context.Message).Skip(1).ToArray();
            string rawCategory = segments.FirstOrFallback("");
            string query = segments.Skip(1).FirstOrFallback("");

            if (!Index.TryGetValue(rawCategory.ToLowerInvariant(), out Category category))
            {
                query = rawCategory;
                category = Category.Item;
            }

            await PerformLookupAsync(context, category, query);
        }

        private static async Task PerformAnimalLookupAsync([NotNull] IContext context, string query)
        {
            var results = new string[TkSettings.LookupLimit];

            using (IEnumerator<ThingItem> enumerator = Data.Items.AsEnumerable()!.GetEnumerator())
            {
                while (results.Length < TkSettings.LookupLimit)
                {
                    ThingItem product = enumerator.Current;

                    if (product == null || !product.Thing.race.Animal || !product.Name.Contains(query, CultureInfo.InvariantCulture))
                    {
                        break;
                    }

                    results[results.Length] = product.Name.CapitalizeFirst();
                }
            }

            await context.Reply(results.ToCommaList(true));
        }

        private static async Task PerformDiseaseLookupAsync([NotNull] IContext context, string query)
        {
            var results = new string[TkSettings.LookupLimit];

            using (IEnumerator<IncidentDef> enumerator = DefDatabase<IncidentDef>.AllDefs.GetEnumerator())
            {
                while (results.Length < TkSettings.LookupLimit)
                {
                    IncidentDef incident = enumerator.Current;

                    if (incident == null || incident.category != IncidentCategoryDefOf.DiseaseHuman || !incident.label.Contains(query, CultureInfo.InvariantCulture))
                    {
                        break;
                    }

                    results[results.Length] = incident.label.CapitalizeFirst();
                }
            }

            await context.Reply(results.ToCommaList(true));
        }

        private static async Task PerformEventLookupAsync([NotNull] IContext context, string query)
        {
            var results = new string[TkSettings.LookupLimit];

            using (IEnumerator<StoreIncident> enumerator = DefDatabase<StoreIncident>.AllDefs.GetEnumerator())
            {
                while (results.Length < TkSettings.LookupLimit)
                {
                    StoreIncident @event = enumerator.Current;

                    if (@event == null || !@event.abbreviation.Contains(query, CultureInfo.InvariantCulture))
                    {
                        break;
                    }

                    results[results.Length] = @event.abbreviation.CapitalizeFirst();
                }
            }

            await context.Reply(results.ToCommaList(true));
        }

        private static async Task PerformItemLookupAsync([NotNull] IContext context, string query)
        {
            var results = new string[TkSettings.LookupLimit];

            using (IEnumerator<ThingItem> enumerator = Data.Items.AsEnumerable()!.GetEnumerator())
            {
                while (results.Length < TkSettings.LookupLimit)
                {
                    ThingItem item = enumerator.Current;

                    if (item == null || !item.Name.Contains(query, CultureInfo.InvariantCulture))
                    {
                        break;
                    }

                    results[results.Length] = item.Name.CapitalizeFirst();
                }
            }

            await context.Reply(results.ToCommaList(true));
        }

        private async Task PerformLookupAsync(IContext context, Category category, string query)
        {
            switch (category)
            {
                case Category.Disease:
                    await PerformDiseaseLookupAsync(context, query);

                    return;
                case Category.Skill:
                    await PerformSkillLookupAsync(context, query);

                    return;
                case Category.Event:
                    await PerformEventLookupAsync(context, query);

                    return;
                case Category.Item:
                    await PerformItemLookupAsync(context, query);

                    return;
                case Category.Animal:
                    await PerformAnimalLookupAsync(context, query);

                    return;
                case Category.Trait:
                    await PerformTraitLookupAsync(context, query);

                    return;
                case Category.Kind:
                    await PerformKindLookupAsync(context, query);

                    return;
                case Category.Mod:
                    await PerformModLookupAsync(context, query);

                    return;
            }
        }

        private static async Task PerformModLookupAsync([NotNull] IContext context, string query)
        {
            var results = new string[TkSettings.LookupLimit];

            using (IEnumerator<ModItem> enumerator = Data.Mods.AsEnumerable()!.GetEnumerator())
            {
                while (results.Length < TkSettings.LookupLimit)
                {
                    ModItem mod = enumerator.Current;

                    if (mod == null || !mod.Name.Contains(query, CultureInfo.InvariantCulture))
                    {
                        break;
                    }

                    results[results.Length] = mod.Name.CapitalizeFirst();
                }
            }

            await context.Reply(results.ToCommaList(true));
        }

        private static async Task PerformKindLookupAsync([NotNull] IContext context, string query)
        {
            var results = new string[TkSettings.LookupLimit];

            using (IEnumerator<PawnKindItem> enumerator = Data.PawnKinds.AsEnumerable()!.GetEnumerator())
            {
                while (results.Length < TkSettings.LookupLimit)
                {
                    PawnKindItem kind = enumerator.Current;

                    if (kind == null || !kind.Name.Contains(query, CultureInfo.InvariantCulture))
                    {
                        break;
                    }

                    results[results.Length] = kind.Name.CapitalizeFirst();
                }
            }

            await context.Reply(results.ToCommaList(true));
        }

        private static async Task PerformSkillLookupAsync([NotNull] IContext context, string query)
        {
            var results = new string[TkSettings.LookupLimit];

            using (IEnumerator<SkillDef> enumerator = DefDatabase<SkillDef>.AllDefs.GetEnumerator())
            {
                while (results.Length < TkSettings.LookupLimit)
                {
                    SkillDef skill = enumerator.Current;

                    if (skill == null)
                    {
                        break;
                    }

                    string label = skill.label.RemoveWhitespace();

                    if (!label.Contains(query, CultureInfo.InvariantCulture))
                    {
                        break;
                    }

                    results[results.Length] = label.CapitalizeFirst();
                }
            }

            await context.Reply(results.ToCommaList(true));
        }

        private static async Task PerformTraitLookupAsync([NotNull] IContext context, string query)
        {
            var results = new string[TkSettings.LookupLimit];

            using (IEnumerator<TraitItem> enumerator = Data.Traits.AsEnumerable()!.GetEnumerator())
            {
                while (results.Length < TkSettings.LookupLimit)
                {
                    TraitItem trait = enumerator.Current;

                    if (trait == null || !trait.Name.Contains(query, CultureInfo.InvariantCulture))
                    {
                        break;
                    }

                    results[results.Length] = trait.Name.CapitalizeFirst();
                }
            }

            await context.Reply(results.ToCommaList(true));
        }
    }
}
