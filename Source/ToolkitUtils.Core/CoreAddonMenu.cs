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
using CommonLib.Helpers;
using JetBrains.Annotations;
using ToolkitCore;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class CoreAddonMenu : IAddonMenu
    {
        private static readonly List<FloatMenuOption> DisconnectedOptions;
        private static readonly List<FloatMenuOption> ConnectedOptions;

        static CoreAddonMenu()
        {
            var baseOptions = new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.AddonMenu.Settings".TranslateSimple(), () => LoadedModManager.GetMod<ToolkitCore.ToolkitCore>().OpenSettings()),
                new FloatMenuOption("Message Log".TranslateSimple(), () => Find.WindowStack.Add(new Window_MessageLog())),
                new FloatMenuOption("Help", () => Application.OpenURL("https://github.com/hodldeeznuts/ToolkitCore/wiki"))
            };

            var reconnectOption = new FloatMenuOption(
                "TKUtils.AddonMenu.Reconnect".TranslateSimple(),
                () => Task.Run(
                    () =>
                    {
                        try
                        {
                            TwitchWrapper.Client.Disconnect();
                        }
                        catch (Exception e)
                        {
                            TkUtils.Logger.Error("Encountered an error while disconnected from Twitch -- You can probably ignore this.", e);
                        }

                        TwitchWrapper.StartAsync();
                    }
                )
            );

            var disconnectOption = new FloatMenuOption("TKUtils.AddonMenu.Disconnect".TranslateSimple(), () => Task.Run(() => TwitchWrapper.Client.Disconnect()));
            var connectOption = new FloatMenuOption("TKUtils.AddonMenu.Connect".TranslateSimple(), () => Task.Run(TwitchWrapper.StartAsync));

            ConnectedOptions = new List<FloatMenuOption>(baseOptions) { reconnectOption, disconnectOption };
            DisconnectedOptions = new List<FloatMenuOption>(baseOptions) { connectOption };
        }

        public List<FloatMenuOption> MenuOptions() => TwitchWrapper.Client?.IsConnected == true ? ConnectedOptions : DisconnectedOptions;
    }
}
