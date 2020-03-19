﻿using System.Collections.Generic;

using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class InstalledModsDriver : CommandBase
    {
        public override void RunCommand(ChatMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            SendCommandMessage(
                "TKUtils.Formats.ModList.Base".Translate(
                    Toolkit.Mod.Version.Named("VERSION"),
                    (TKSettings.VersionedModList ? GetModListStringVersioned() : GetModListString()).Named("MODS")
                ),
                message
            );
        }

        private static string GetModListString()
        {
            var container = new List<string>();
            var unversioned = TKUtils.GetModListUnversioned();

            foreach(var mod in unversioned)
            {
                container.Add(TryFavoriteMod(mod));
            }

            return string.Join(
                "TKUtils.Misc.Separators.Inner".Translate(),
                container
            );
        }

        private static string GetModListStringVersioned()
        {
            var container = new List<string>();
            var versioned = TKUtils.GetModListVersioned();

            foreach(var mod in versioned)
            {
                container.Add(
                    "TKUtils.Formats.ModList.Mod".Translate(
                        TryFavoriteMod(mod.Item1).Named("NAME"),
                        mod.Item2.Named("VERSION")
                    )
                );
            }

            return string.Join(
                "TKUtils.Misc.Separators.Inner".Translate(),
                container
            );
        }

        private static string TryFavoriteMod(string mod)
        {
            if(mod.EqualsIgnoreCase(TKUtils.ID))
            {
                return GetTranslatedEmoji(
                    "TKUtils.Misc.Decorators.Favorite",
                    "TKUtils.Misc.Decorators.Favorite.Text"
                ).Translate(
                    mod.Named("DECORATING")
                );
            }
            else
            {
                return mod;
            }
        }
    }
}
