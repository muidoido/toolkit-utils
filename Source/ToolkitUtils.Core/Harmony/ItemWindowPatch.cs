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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.Windows;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ItemWindowPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(StoreItemsWindow), "PostClose");
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void Prefix([NotNull] List<ThingDef> ___cachedTradeables, List<int> ___tradeablesPrices)
        {
            for (var i = 0; i < ___cachedTradeables.Count; i++)
            {
                ThingDef t = ___cachedTradeables[i];

                if (t.race?.Humanlike != true)
                {
                    continue;
                }

                int price = ___tradeablesPrices[i];

                if (price < 0)
                {
                    continue;
                }

                ___tradeablesPrices[i] = -10;
            }

            SaveKinds();
        }

        private static void SaveKinds()
        {
            if (TkSettings.Offload)
            {
                Task.Run(
                    async () =>
                    {
                        switch (TkSettings.DumpStyle)
                        {
                            case "MultiFile":
                                await Data.SavePawnKindsAsync(Paths.PawnKindFilePath);

                                return;
                            case "SingleFile":
                                await Data.SaveLegacyShopAsync(Paths.LegacyShopDumpFilePath);

                                return;
                        }
                    }
                );
            }
            else
            {
                switch (TkSettings.DumpStyle)
                {
                    case "MultiFile":
                        Data.SavePawnKinds(Paths.PawnKindFilePath);

                        return;
                    case "SingleFile":
                        Data.SaveLegacyShop(Paths.LegacyShopDumpFilePath);

                        return;
                }
            }
        }
    }
}
