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
using System.Collections.Generic;
using System.Linq;
using CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum LeaveMethod { Thanos, MentalBreak }

    public enum DumpStyle { SingleFile, MultiFile }

    public enum UserCoinType { Broadcaster, Subscriber, Vip, Moderator, None }

    [StaticConstructorOnStartup]
    public class TkSettings : ModSettings
    {
        private static string _modVersion;
        public static bool TrueNeutral;
        public static bool ForceFullItem;
        public static bool Commands = true;
        public static string Prefix = "!";
        public static string BuyPrefix = "$";
        public static bool ToolkitStyleCommands = true;
        public static bool MinifyData;
        public static bool DecorateMods;
        public static bool Emojis = true;
        public static bool FilterWorkPriorities;
        public static bool ShowApparel;
        public static bool ShowArmor = true;
        public static bool ShowSurgeries = true;
        public static bool ShowWeapon = true;
        public static bool SortWorkPriorities;
        public static bool PurchasePawnKinds = true;
        public static bool TempInGear;
        public static bool DropInventory;
        public static string BroadcasterCoinType = nameof(UserCoinType.Broadcaster);
        public static string LeaveMethod = nameof(ToolkitUtils.LeaveMethod.MentalBreak);
        public static string DumpStyle = nameof(ToolkitUtils.DumpStyle.SingleFile);
        public static int LookupLimit = 10;
        public static bool VersionedModList;
        public static bool ShowCoinRate = true;
        public static bool HairColor = true;
        public static int OpinionMinimum;
        public static bool AsapPurchases;
        public static int StoreBuildRate = 60;
        public static bool StoreState = true; // Used by !togglestore to temporarily disable Twitch Toolkit's store.
        public static bool Offload;
        public static bool BuyItemBalance;
        public static bool ClassChanges;
        public static bool ResetClass;
        public static bool VisualExceptions;
        public static bool MinimalRelations = true;
        public static bool GatewayPuff = true;

        public static List<WorkSetting> WorkSettings = new List<WorkSetting>();
        private static List<FloatMenuOption> _dumpStyleOptions;
        private static List<FloatMenuOption> _coinUserTypeOptions;

        private static WorkTypeDef[] _workTypeDefs;

        private static Vector2 _workScrollPos = Vector2.zero;
        private static Vector2 _commandTweaksPos = Vector2.zero;
        private static Vector2 _dataScrollPos = Vector2.zero;
        public static bool EasterEggs = true;
        public static bool CommandRouter = true;
        public static bool TransparentColors;

        static TkSettings()
        {
            _workTypeDefs = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToArray();
        }

        public static List<FloatMenuOption> LeaveMenuOptions { get; set; }

        public static void DoWindowContents(Rect inRect)
        {
            // A fix for how some windows embed Utils' settings.
            inRect.height = inRect.height > 620f ? 620f : inRect.height;
            ValidateEnumOptions();

            Color cache = GUI.color;
            GUI.color = Color.grey;
            Widgets.DrawLightHighlight(inRect);
            GUI.color = cache;

            GUI.BeginGroup(inRect);
            var tabBarRect = new Rect(0f, 0f, inRect.width, Text.LineHeight * 2f);
            var tabPanelRect = new Rect(0f, tabBarRect.height, inRect.width, inRect.height - tabBarRect.height);
            Rect contentRect = tabPanelRect.ContractedBy(20f);
            var trueContentRect = new Rect(0f, 0f, contentRect.width, contentRect.height);


            GUI.BeginGroup(tabBarRect);
            GUI.EndGroup();

            Widgets.DrawLightHighlight(tabPanelRect);
            GUI.BeginGroup(contentRect);

            GUI.EndGroup();


            GUI.EndGroup();
        }

        private static void DrawModCompatTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.ModGroupHeader("Humanoid Alien Races", 839005762, false);
            listing.CheckboxLabeled("TKUtils.HAR.PawnKinds.Label".TranslateSimple(), ref PurchasePawnKinds);
            listing.DrawDescription("TKUtils.HAR.PawnKinds.Description".TranslateSimple());

            listing.ModGroupHeader("A RimWorld of Magic", 1201382956);
            listing.CheckboxLabeled("TKUtils.TMagic.Classes.Label".TranslateSimple(), ref ClassChanges);
            listing.DrawDescription("TKUtils.TMagic.Classes.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            if (ClassChanges)
            {
                listing.CheckboxLabeled("TKUtils.TMagic.ResetClass.Label".TranslateSimple(), ref ResetClass);
                listing.DrawDescription("TKUtils.TMagic.ResetClass.Description".TranslateSimple());
                listing.DrawExperimentalNotice();
            }

            listing.ModGroupHeader("Visual Exceptions", 2538411704);
            listing.CheckboxLabeled("TKUtils.VisualExceptions.SendErrors.Label".TranslateSimple(), ref VisualExceptions);
            listing.DrawDescription("TKUtils.VisualExceptions.SendErrors.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            listing.End();
        }

        private static void DrawDataTab(Rect canvas)
        {
            var view = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 32f);
            var listing = new Listing_Standard();
            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _dataScrollPos, view);
            listing.Begin(view);

            listing.GroupHeader("TKUtils.Data.Files".TranslateSimple(), false);

            (Rect dumpLabel, Rect dumpBtn) = listing.Split();
            UiHelper.Label(dumpLabel, "TKUtils.DumpStyle.Label".TranslateSimple());
            listing.DrawDescription("TKUtils.DumpStyle.Description".TranslateSimple());

            if (Widgets.ButtonText(dumpBtn, $"TKUtils.DumpStyle.{DumpStyle}".TranslateSimple()))
            {
                Find.WindowStack.Add(new FloatMenu(_dumpStyleOptions));
            }

            listing.CheckboxLabeled("TKUtils.MinifyData.Label".TranslateSimple(), ref MinifyData);
            listing.DrawDescription("TKUtils.MinifyData.Description".TranslateSimple());

            listing.CheckboxLabeled("TKUtils.OffloadShop.Label".TranslateSimple(), ref Offload);
            listing.DrawDescription("TKUtils.OffloadShop.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            listing.CheckboxLabeled("TKUtils.DoPurchasesAsap.Label".TranslateSimple(), ref AsapPurchases);
            listing.DrawDescription("TKUtils.DoPurchasesAsap.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            listing.CheckboxLabeled("TKUtils.TrueNeutral.Label".TranslateSimple(), ref TrueNeutral);
            listing.DrawDescription("TKUtils.TrueNeutral.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            (Rect coinTypeLabel, Rect coinTypeField) = listing.Split();
            UiHelper.Label(coinTypeLabel, "TKUtils.BroadcasterUserType.Label".TranslateSimple());
            listing.DrawDescription("TKUtils.BroadcasterUserType.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            if (Widgets.ButtonText(coinTypeField, $"TKUtils.BroadcasterUserType.{BroadcasterCoinType}".TranslateSimple()))
            {
                Find.WindowStack.Add(new FloatMenu(_coinUserTypeOptions));
            }


            listing.GroupHeader("TKUtils.Data.LazyProcess".TranslateSimple());

            (Rect storeLabel, Rect storeField) = listing.Split();
            UiHelper.Label(storeLabel, "TKUtils.StoreRate.Label".TranslateSimple());
            listing.DrawDescription("TKUtils.StoreRate.Description".TranslateSimple());

            var storeBuffer = StoreBuildRate.ToString();
            Widgets.TextFieldNumeric(storeField, ref StoreBuildRate, ref storeBuffer);

            listing.End();
            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        private static void ValidateEnumOptions()
        {
            _dumpStyleOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.DumpStyle.SingleFile".TranslateSimple(), () => DumpStyle = nameof(ToolkitUtils.DumpStyle.SingleFile)),
                new FloatMenuOption("TKUtils.DumpStyle.MultiFile".TranslateSimple(), () => DumpStyle = nameof(ToolkitUtils.DumpStyle.MultiFile))
            };

            LeaveMenuOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.Abandon.Method.Thanos".TranslateSimple(), () => LeaveMethod = nameof(ToolkitUtils.LeaveMethod.Thanos)),
                new FloatMenuOption("TKUtils.Abandon.Method.MentalBreak".TranslateSimple(), () => LeaveMethod = nameof(ToolkitUtils.LeaveMethod.MentalBreak))
            };

            _coinUserTypeOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.BroadcasterUserType.Broadcaster".TranslateSimple(), () => BroadcasterCoinType = nameof(UserCoinType.Broadcaster)),
                new FloatMenuOption("TKUtils.BroadcasterUserType.Subscriber".TranslateSimple(), () => BroadcasterCoinType = nameof(UserCoinType.Subscriber)),
                new FloatMenuOption("TKUtils.BroadcasterUserType.Vip".TranslateSimple(), () => BroadcasterCoinType = nameof(UserCoinType.Vip)),
                new FloatMenuOption("TKUtils.BroadcasterUserType.Moderator".TranslateSimple(), () => BroadcasterCoinType = nameof(UserCoinType.Moderator)),
                new FloatMenuOption("TKUtils.BroadcasterUserType.None".TranslateSimple(), () => BroadcasterCoinType = nameof(UserCoinType.None))
            };
        }

        private static void DrawGeneralTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.GroupHeader("TKUtils.General.Emojis".TranslateSimple(), false);
            listing.CheckboxLabeled("TKUtils.Emojis.Label".TranslateSimple(), ref Emojis);
            listing.DrawDescription("TKUtils.Emojis.Description".TranslateSimple());


            listing.GroupHeader("TKUtils.General.Viewer".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.HairColor.Label".TranslateSimple(), ref HairColor);
            listing.DrawDescription("TKUtils.HairColor.Description".TranslateSimple());

            listing.GroupHeader("TKUtils.General.Gateway".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.GatewayPuff.Label".TranslateSimple(), ref GatewayPuff);
            listing.DrawDescription("TKUtils.GatewayPuff.Description".TranslateSimple());

            listing.GroupHeader("TKUtils.General.Basket".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.EasterEggs.Label".TranslateSimple(), ref EasterEggs);
            listing.DrawDescription("TKUtils.EasterEggs.Description".TranslateSimple());

            listing.End();
        }

        private static void DrawCommandTweaksTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 48f);

            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _commandTweaksPos, viewPort);
            listing.Begin(viewPort);

            listing.GroupHeader("TKUtils.CommandTweaks.Balance".TranslateSimple(), false);
            listing.CheckboxLabeled("TKUtils.CoinRate.Label".TranslateSimple(), ref ShowCoinRate);
            listing.DrawDescription("TKUtils.CoinRate.Description".TranslateSimple());


            listing.GroupHeader("TKUtils.CommandTweaks.Handler".TranslateSimple());

            if (Commands)
            {
                (Rect prefixLabel, Rect prefixField) = listing.Split();
                UiHelper.Label(prefixLabel, "TKUtils.CommandPrefix.Label".TranslateSimple());
                listing.DrawDescription("TKUtils.CommandPrefix.Description".TranslateSimple());
                Prefix = CommandHelper.ValidatePrefix(Widgets.TextField(prefixField, Prefix));

                (Rect buyPrefixLabel, Rect buyPrefixField) = listing.Split();
                UiHelper.Label(buyPrefixLabel, "TKUtils.PurchasePrefix.Label".TranslateSimple());
                listing.DrawDescription("TKUtils.PurchasePrefix.Description".TranslateSimple());
                BuyPrefix = CommandHelper.ValidatePrefix(Widgets.TextField(buyPrefixField, BuyPrefix));
            }

            listing.CheckboxLabeled("TKUtils.CommandParser.Label".TranslateSimple(), ref Commands);
            listing.DrawDescription("TKUtils.CommandParser.Description".TranslateSimple());

            if (Commands)
            {
                listing.CheckboxLabeled("TKUtils.ToolkitStyleCommands.Label".TranslateSimple(), ref ToolkitStyleCommands);
                listing.DrawDescription("TKUtils.ToolkitStyleCommands.Description".TranslateSimple());
            }

            listing.CheckboxLabeled("TKUtils.CommandRouter.Label".TranslateSimple(), ref CommandRouter);
            listing.DrawDescription("TKUtils.CommandRouter.Description".TranslateSimple());
            listing.DrawExperimentalNotice();


            listing.GroupHeader("TKUtils.CommandTweaks.InstalledMods".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.DecorateUtils.Label".TranslateSimple(), ref DecorateMods);
            listing.DrawDescription("TKUtils.DecorateUtils.Description".TranslateSimple());

            listing.CheckboxLabeled("TKUtils.VersionedModList.Label".TranslateSimple(), ref VersionedModList);
            listing.DrawDescription("TKUtils.VersionedModList.Description".TranslateSimple());


            listing.GroupHeader("TKUtils.CommandTweaks.BuyItem".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.BuyItemBalance.Label".TranslateSimple(), ref BuyItemBalance);
            listing.DrawDescription("TKUtils.BuyItemBalance.Description".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.BuyItemFullSyntax.Label".TranslateSimple(), ref ForceFullItem);
            listing.DrawDescription("TKUtils.BuyItemFullSyntax.Description".TranslateSimple());


            listing.GroupHeader("TKUtils.CommandTweaks.Lookup".TranslateSimple());

            (Rect lookupLimitLabel, Rect lookupLimitField) = listing.Split();
            var buffer = LookupLimit.ToString();

            UiHelper.Label(lookupLimitLabel, "TKUtils.LookupLimit.Label".TranslateSimple());
            Widgets.TextFieldNumeric(lookupLimitField, ref LookupLimit, ref buffer);
            listing.DrawDescription("TKUtils.LookupLimit.Description".TranslateSimple());

            GUI.EndGroup();
            Widgets.EndScrollView();
            listing.End();
        }

        private static void DrawPawnCommandsTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 40f);

            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _commandTweaksPos, viewPort);
            listing.Begin(viewPort);

            listing.GroupHeader("TKUtils.PawnCommands.Abandon".Translate(), false);

            (Rect leaveLabelRect, Rect leaveRect) = listing.Split();
            UiHelper.Label(leaveLabelRect, "TKUtils.Abandon.Method.Label".TranslateSimple());
            listing.DrawDescription("TKUtils.Abandon.Method.Description".TranslateSimple());

            if (Widgets.ButtonText(leaveRect, $"TKUtils.Abandon.Method.{LeaveMethod}".TranslateSimple()))
            {
                Find.WindowStack.Add(new FloatMenu(LeaveMenuOptions));
            }

            if (!LeaveMethod.EqualsIgnoreCase(nameof(ToolkitUtils.LeaveMethod.Thanos)))
            {
                listing.CheckboxLabeled("TKUtils.Abandon.Gear.Label".TranslateSimple(), ref DropInventory);
                listing.DrawDescription("TKUtils.Abandon.Gear.Description".TranslateSimple());
            }

            listing.GroupHeader("TKUtils.PawnCommands.Gear".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.PawnGear.Temperature.Label".TranslateSimple(), ref TempInGear);
            listing.DrawDescription("TKUtils.PawnGear.Temperature.Description".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.PawnGear.Apparel.Label".TranslateSimple(), ref ShowApparel);
            listing.DrawDescription("TKUtils.PawnGear.Apparel.Description".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.PawnGear.Armor.Label".TranslateSimple(), ref ShowArmor);
            listing.DrawDescription("TKUtils.PawnGear.Armor.Description".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.PawnGear.Weapon.Label".TranslateSimple(), ref ShowWeapon);
            listing.DrawDescription("TKUtils.PawnGear.Weapon.Description".TranslateSimple());


            listing.GroupHeader("TKUtils.PawnCommands.Health".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.PawnHealth.Surgeries.Label".TranslateSimple(), ref ShowSurgeries);
            listing.DrawDescription("TKUtils.PawnHealth.Surgeries.Description".TranslateSimple());


            listing.GroupHeader("TKUtils.PawnCommands.Relations".TranslateSimple());
            (Rect opinionLabel, Rect opinionField) = listing.Split();
            var buffer = OpinionMinimum.ToString();

            if (!MinimalRelations)
            {
                UiHelper.Label(opinionLabel, "TKUtils.PawnRelations.OpinionThreshold.Label".TranslateSimple());
                Widgets.TextFieldNumeric(opinionField, ref OpinionMinimum, ref buffer);
                listing.DrawDescription("TKUtils.PawnRelations.OpinionThreshold.Description".TranslateSimple());
            }

            listing.CheckboxLabeled("TKUtils.PawnRelations.MinimalRelations.Label".TranslateSimple(), ref MinimalRelations);
            listing.DrawDescription("TKUtils.PawnRelations.MinimalRelations.Description".TranslateSimple());

            listing.GroupHeader("TKUtils.PawnCommands.Work".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.PawnWork.Sort.Label".TranslateSimple(), ref SortWorkPriorities);
            listing.DrawDescription("TKUtils.PawnWork.Sort.Description".TranslateSimple());
            listing.CheckboxLabeled("TKUtils.PawnWork.Filter.Label".TranslateSimple(), ref FilterWorkPriorities);
            listing.DrawDescription("TKUtils.PawnWork.Filter.Description".TranslateSimple());

            GUI.EndGroup();
            Widgets.EndScrollView();
            listing.End();
        }

        private static void DrawPawnWorkTab(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            var listing = new Listing_Standard();
            var content = new Rect(0f, 0f, canvas.width, canvas.height);
            var view = new Rect(0f, 0f, canvas.width - 16f, _workTypeDefs.Length * Text.LineHeight);

            Widgets.BeginScrollView(content, ref _workScrollPos, view);
            listing.Begin(view);

            for (var index = 0; index < _workTypeDefs.Length; index++)
            {
                WorkTypeDef workType = _workTypeDefs[index];
                WorkSetting workSetting = WorkSettings.FirstOrDefault(w => w.WorkTypeDef.EqualsIgnoreCase(workType.defName));

                if (workSetting == null)
                {
                    workSetting = new WorkSetting { Enabled = true, WorkTypeDef = workType.defName };

                    WorkSettings.Add(workSetting);
                }

                Rect line = listing.GetRect(Text.LineHeight);

                if (!line.IsVisible(content, _workScrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(line);
                }

                Widgets.CheckboxLabeled(line, workSetting.WorkTypeDef, ref workSetting.Enabled);

                Widgets.DrawHighlightIfMouseover(line);
            }

            GUI.EndGroup();
            Widgets.EndScrollView();
            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Commands, "commands", true);
            Scribe_Values.Look(ref Prefix, "prefix", "!");
            Scribe_Values.Look(ref BuyPrefix, "buyPrefix", "$");
            Scribe_Values.Look(ref ToolkitStyleCommands, "toolkitStyleCommands", true);
            Scribe_Values.Look(ref DecorateMods, "decorateUtils");
            Scribe_Values.Look(ref ForceFullItem, "forceFullItemSyntax");
            Scribe_Values.Look(ref Emojis, "emojis", true);
            Scribe_Values.Look(ref FilterWorkPriorities, "filterWork");
            Scribe_Values.Look(ref ShowApparel, "apparel");
            Scribe_Values.Look(ref ShowArmor, "armor", true);
            Scribe_Values.Look(ref ShowSurgeries, "surgeries", true);
            Scribe_Values.Look(ref ShowWeapon, "weapon", true);
            Scribe_Values.Look(ref SortWorkPriorities, "sortWork");
            Scribe_Values.Look(ref PurchasePawnKinds, "race", true);
            Scribe_Values.Look(ref TempInGear, "tempInGear");
            Scribe_Values.Look(ref DropInventory, "dropInventory");
            Scribe_Values.Look(ref LeaveMethod, "leaveMethod", nameof(ToolkitUtils.LeaveMethod.MentalBreak));
            Scribe_Values.Look(ref DumpStyle, "dumpStyle", nameof(ToolkitUtils.DumpStyle.SingleFile));
            Scribe_Values.Look(ref BroadcasterCoinType, "broadcasterCoinType", nameof(UserCoinType.Broadcaster));
            Scribe_Values.Look(ref LookupLimit, "lookupLimit", 10);
            Scribe_Values.Look(ref AsapPurchases, "asapPurchases");
            Scribe_Values.Look(ref VersionedModList, "versionedModList");
            Scribe_Values.Look(ref ShowCoinRate, "balanceCoinRate", true);
            Scribe_Values.Look(ref TrueNeutral, "trueNeutral");
            Scribe_Values.Look(ref HairColor, "hairColor", true);
            Scribe_Values.Look(ref OpinionMinimum, "minimumOpinion", 20);
            Scribe_Values.Look(ref StoreBuildRate, "storeBuildRate", 60);
            Scribe_Values.Look(ref Offload, "offload");
            Scribe_Values.Look(ref BuyItemBalance, "buyItemBalance");
            Scribe_Values.Look(ref ClassChanges, "classChanges");
            Scribe_Values.Look(ref ResetClass, "resetClass");
            Scribe_Values.Look(ref MinimalRelations, "minimalRelations", true);
            Scribe_Values.Look(ref GatewayPuff, "gatewayPuff", true);
            Scribe_Values.Look(ref EasterEggs, "easterEggs", true);
            Scribe_Values.Look(ref TransparentColors, "allowTransparentColors");

            Scribe_Collections.Look(ref WorkSettings, "workSettings", LookMode.Deep);
        }

        internal static void ValidateDynamicSettings()
        {
            if (_workTypeDefs.NullOrEmpty())
            {
                _workTypeDefs = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToArray();
            }


            WorkSettings ??= new List<WorkSetting>();


            foreach (WorkTypeDef workType in _workTypeDefs.Where(d => !WorkSettings.Any(s => s.WorkTypeDef.EqualsIgnoreCase(d.defName))))
            {
                WorkSettings.Add(new WorkSetting { Enabled = true, WorkTypeDef = workType.defName });
            }
        }

        public class WorkSetting : IExposable
        {
            [Description("Whether or not the work priority will be shown in !mypawnwork")]
            public bool Enabled;

            [Description("The def name of the work type instance.")]
            public string WorkTypeDef;

            public void ExposeData()
            {
                Scribe_Values.Look(ref WorkTypeDef, "defName");
                Scribe_Values.Look(ref Enabled, "enabled", true);
            }
        }
    }
}
