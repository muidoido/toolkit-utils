﻿using System.Collections.Generic;
using System.Linq;

using RimWorld;

using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnGearCommand : CommandBase
    {
        private static bool IsTemperatureCustom = false;
        private static bool TemperatureCheck = true;

        public override void RunCommand(ChatMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.Username);

            if(pawn == null)
            {
                SendCommandMessage(
                    "TKUtils.Responses.NoPawn".Translate(),
                    message
                );
                return;
            }

            SendCommandMessage(
                "TKUtils.Formats.PawnGear.Base".Translate(
                    GetPawnGear(pawn).Named("COMPOSITE")
                ),
                message
            );
        }

        private float CalculateArmorRating(Pawn pawn, StatDef stat)
        {
            var rating = 0f;
            var value = Mathf.Clamp01(StatExtension.GetStatValue(pawn, stat, applyPostProcess: true) / 2f);
            var parts = pawn.RaceProps.body.AllParts;
            var apparel = pawn.apparel?.WornApparel;

            foreach(var part in parts)
            {
                var cache = 1f - value;

                if(apparel != null && apparel.Any())
                {
                    foreach(var a in apparel)
                    {
                        if(a.def.apparel.CoversBodyPart(part))
                        {
                            float v = Mathf.Clamp01(StatExtension.GetStatValue(a, stat, applyPostProcess: true) / 2f);
                            cache *= 1f - v;
                        }
                    }
                }

                rating += part.coverageAbs * (1f - cache);
            }

            return Mathf.Clamp(rating * 2f, 0f, 2f);
        }

        private TaggedString GetPawnGear(Pawn pawn)
        {
            var parts = new List<string>();

            if(TKSettings.TempInGear)
            {
                var tempMin = GenText.ToStringTemperature(StatExtension.GetStatValue(pawn, StatDefOf.ComfyTemperatureMin, applyPostProcess: true));
                var tempMax = GenText.ToStringTemperature(StatExtension.GetStatValue(pawn, StatDefOf.ComfyTemperatureMax, applyPostProcess: true));
                TaggedString display;

                if(IsTemperatureCustom)
                {
                    display = TaggedString.Empty;
                }
                else
                {
                    display = GetTemperatureDisplay();

                    if(TemperatureCheck && display.RawText.Contains("["))
                    {
                        Logger.Info("Custom temperature display detected; omitting temperature scale from now on.");
                        IsTemperatureCustom = true;
                        TemperatureCheck = false;

                        display = TaggedString.Empty;
                    }
                }

                parts.Add(
                    "TKUtils.Formats.PawnGear.Temperature".Translate(
                        tempMin.Named("MINIMUM"),
                        tempMax.Named("MAXIMUM"),
                        display.Named("DISPLAY")
                    )
                );
            }

            if(TKSettings.ShowArmor)
            {
                var sharp = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                var blunt = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                var heat = CalculateArmorRating(pawn, StatDefOf.ArmorRating_Sharp);
                var stats = new List<string>();

                if(sharp > 0)
                {
                    stats.Add(
                        GetTranslatedEmoji("TKUtils.Formats.PawnGear.Armor.Sharp").Translate(
                            GenText.ToStringPercent(sharp).Named("SHARP")
                        )
                    );
                }

                if(blunt > 0)
                {
                    stats.Add(
                        GetTranslatedEmoji("TKUtils.Formats.PawnGear.Armor.Blunt").Translate(
                            GenText.ToStringPercent(blunt).Named("BLUNT")
                        )
                    );
                }

                if(heat > 0)
                {
                    stats.Add(
                        GetTranslatedEmoji("TKUtils.Formats.PawnGear.Armor.Heat").Translate(
                            GenText.ToStringPercent(heat).Named("HEAT")
                        )
                    );
                }

                parts.Add(
                    "TKUtils.Formats.PawnGear.Armor".Translate(
                        string.Join(
                            "TKUtils.Misc.Separators.Inner".Translate(),
                            stats.ToArray()
                        ).Named("STATS")
                    )
                );
            }

            if(TKSettings.ShowWeapon)
            {
                var e = pawn.equipment;

                if(e != null && e.AllEquipmentListForReading?.Count > 0)
                {
                    var equip = e.AllEquipmentListForReading;
                    var container = new List<string>();

                    foreach(var eq in equip)
                    {
                        container.Add(eq.LabelCap);
                    }

                    parts.Add(
                        "TKUtils.Formats.PawnGear.Equipment".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                container.ToArray()
                            ).Named("EQUIPMENT")
                        )
                    );
                }
            }

            if(TKSettings.ShowApparel)
            {
                var a = pawn.apparel;

                if(a != null && a.WornApparelCount > 0)
                {
                    var container = new List<string>();
                    var apparel = a.WornApparel;

                    foreach(var item in apparel)
                    {
                        container.Add(item.LabelCap);
                    }

                    parts.Add(
                        "TKUtils.Formats.PawnGear.Apparel".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                container.ToArray()
                            ).Named("APPAREL")
                        )
                    );
                }
            }

            return string.Join(
                "TKUtils.Misc.Separators.Upper".Translate(),
                parts.ToArray()
            );
        }

        private TaggedString GetTemperatureDisplay()
        {
            switch(Prefs.TemperatureMode)
            {
                case TemperatureDisplayMode.Fahrenheit:
                    return "TKUtils.Misc.Temperature.Fahrenheit".Translate();

                case TemperatureDisplayMode.Kelvin:
                    return "TKUtils.Misc.Temperature.Kelvin".Translate();

                case TemperatureDisplayMode.Celsius:
                    return "TKUtils.Misc.Temperature.Celsius".Translate();

                default:
                    return "?";
            }
        }
    }
}
