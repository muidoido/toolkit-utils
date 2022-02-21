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
using SirRandoo.ToolkitUtils.Utils.ModComp;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnGear : CommandBase
    {
        public override async Task RunCommandAsync(IContext context)
        {
            if (!PurchaseHelper.TryGetPawn(context.User.Username, out Pawn pawn))
            {
                await context.Reply(await "TKUtils.NoPawn".TranslateSimpleAsync());

                return;
            }

            await context.Reply(string.Join(context.UseEmojis ? ResponseHelper.OuterGroupSeparator : ResponseHelper.OuterGroupSeparatorAlt, await GetPawnGearAsync(pawn)));
        }

        private static async Task<string[]> GetPawnGearAsync(Pawn pawn)
        {
            var container = new List<string>();

            Func<string[]> func;

            string[] result;

            if (TkSettings.TempInGear)
            {
                func = new Func<string[]>(() => GetTemperatureValues(pawn));
                result = await func.OnMainThreadAsync();
                container.Add(result.ToCommaList());
            }

            if (TkSettings.ShowArmor)
            {
                func = new Func<string[]>(() => GetArmorValues(pawn));
                result = await func.OnMainThreadAsync();
                container.Add(result.ToCommaList());
            }

            if (TkSettings.ShowWeapon)
            {
                func = new Func<string[]>(() => GetWeaponData(pawn));
                result = await func.OnMainThreadAsync();
                container.Add(result.ToCommaList());
            }

            if (!TkSettings.ShowApparel)
            {
                return container.ToArray();
            }

            func = () => GetWornApparel(pawn);
            result = await func.OnMainThreadAsync();
            container.Add(await "Apparel".TranslateSimpleAsync() + ": " + result.ToCommaList());

            return container.ToArray();
        }

        [NotNull]
        private static string[] GetWornApparel([NotNull] Pawn pawn)
        {
            Pawn_ApparelTracker tracker = pawn.apparel;

            if (tracker == null || tracker.WornApparelCount <= 0)
            {
                return new[] { "None".TranslateSimple() };
            }

            var list = new List<string>();

            foreach (Apparel a in tracker.WornApparel)
            {
                list.Add(Unrichify.StripTags(a.Label));
            }

            return list.ToArray();
        }

        private static float CalculateArmorRating([NotNull] Pawn pawn, StatDef stat)
        {
            var rating = 0f;
            float value = Mathf.Clamp01(pawn.GetStatValue(stat) / 2f);
            List<BodyPartRecord> parts = pawn.RaceProps.body.AllParts;
            List<Apparel> apparel = pawn.apparel?.WornApparel;
            bool hasApparel = !apparel.NullOrEmpty();

            foreach (BodyPartRecord record in pawn.RaceProps.body.AllParts)
            {
                float t = 1f - value;

                if (hasApparel)
                {
                    List<Apparel> relevant = apparel!.FindAll(a => a.def.apparel.CoversBodyPart(record));
                    t = relevant.Select(a => Mathf.Clamp01(a.GetStatValue(stat) / 2f)).Aggregate(t, (c, v) => c * (1f - v));
                }

                rating += record.coverageAbs * (1f - t);
            }

            return Mathf.Clamp(rating * 2f, 0f, 2f);
        }

        private static void GetWeaponData([NotNull] Pawn pawn, ICollection<string> parts)
        {
            List<Thing> sidearms = SimpleSidearms.GetSidearms(pawn)?.ToList();
            var weapons = new List<string>();
            List<ThingWithComps> equipment = pawn.equipment?.AllEquipmentListForReading ?? new List<ThingWithComps>();
            int equipmentCount = equipment.Count;
            List<Thing> inventory = pawn.inventory.innerContainer.InnerListForReading ?? new List<Thing>();
            var usedInventory = new List<Thing>();

            if (sidearms?.Count > 0)
            {
                GetSidearmData(sidearms, equipmentCount, equipment, weapons, inventory, usedInventory);
            }
            else
            {
                Pawn_EquipmentTracker e = pawn.equipment;

                if (e?.AllEquipmentListForReading?.Count > 0)
                {
                    IEnumerable<string> equip = e.AllEquipmentListForReading.Select(eq => Unrichify.StripTags(eq.LabelCap));

                    weapons.AddRange(equip);
                }
            }

            if (weapons.Count <= 0)
            {
                return;
            }

            string section = "Stat_Weapon_Name".TranslateSimple();

            parts.Add($"{(weapons.Count > 1 ? section.Pluralize() : section)}: {weapons.SectionJoin()}");
        }

        private static void GetTemperatureValues(Thing pawn, [NotNull] ICollection<string> parts)
        {
            string tempMin = pawn.GetStatValue(StatDefOf.ComfyTemperatureMin).ToStringTemperature();
            string tempMax = pawn.GetStatValue(StatDefOf.ComfyTemperatureMax).ToStringTemperature();

            parts.Add($"{ResponseHelper.TemperatureGlyph.AltText($"{"ComfyTemperatureRange".Localize()} ")}{tempMin}~{tempMax}");
        }

        private static void GetSidearmData(
            [NotNull] ICollection<Thing> sidearms,
            int equipmentCount,
            IReadOnlyCollection<ThingWithComps> equipment,
            IList<string> weapons,
            IReadOnlyCollection<Thing> inventory,
            ICollection<Thing> usedInventory
        )
        {
            var loops = 0;
            var equipmentUsed = false;

            while (sidearms.Count > 0 && loops <= 50)
            {
                Thing sidearm = sidearms.Take(1).FirstOrDefault();

                if (sidearm == null)
                {
                    continue;
                }

                if (equipmentCount > 0 && !equipmentUsed && GetEquipmentFromSidearmData(equipment, weapons, sidearm))
                {
                    sidearms.Remove(sidearm);
                    equipmentUsed = true;

                    continue;
                }

                GetSidearms(sidearms, weapons, inventory, usedInventory, sidearm);
                loops++;
            }
        }

        private static void GetSidearms(ICollection<Thing> sidearms, ICollection<string> weapons, [NotNull] IEnumerable<Thing> inventory, ICollection<Thing> usedInventory, Thing sidearm)
        {
            foreach (Thing thing in inventory.Where(thing => sidearm.def.defName.Equals(thing.def.defName)))
            {
                if (usedInventory.Contains(thing))
                {
                    continue;
                }

                weapons.Add(Unrichify.StripTags(thing.LabelCap));
                usedInventory.Add(thing);
                sidearms.Remove(sidearm);

                break;
            }
        }

        private static bool GetEquipmentFromSidearmData([NotNull] IEnumerable<ThingWithComps> equipment, IList<string> weapons, Thing sidearm)
        {
            foreach (ThingWithComps equip in equipment.Where(equip => sidearm.def.defName.Equals(equip.def.defName)))
            {
                weapons.Insert(0, Unrichify.StripTags(equip.LabelCap));

                return true;
            }

            return false;
        }

        private static void GetArmorValues([NotNull] Pawn pawn, ICollection<string> parts)
        {
            float sharp = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
            float blunt = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Blunt);
            float heat = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Heat);
            var stats = new List<string>();

            if (sharp > 0)
            {
                stats.Add($"{ResponseHelper.DaggerGlyph.AltText($"{"ArmorSharp".Localize()} ")}{sharp.ToStringPercent()}");
            }

            if (blunt > 0)
            {
                stats.Add($"{ResponseHelper.PanGlyph.AltText($"{"ArmorBlunt".Localize()} ")}{blunt.ToStringPercent()}");
            }

            if (heat > 0)
            {
                stats.Add($"{ResponseHelper.FireGlyph.AltText($"{"ArmorHeat".Localize()} ")}{heat.ToStringPercent()}");
            }

            if (stats.Any())
            {
                parts.Add($"{"OverallArmor".Localize()}: {stats.SectionJoin()}");
            }
        }
    }
}
