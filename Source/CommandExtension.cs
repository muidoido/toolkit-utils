﻿using System.Collections.Generic;
using JetBrains.Annotations;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum UserLevels { Anyone, Vip, Subscriber, Moderator }

    [UsedImplicitly]
    public class Parameter
    {
        [Description("The name of this parameter.")]
        [UsedImplicitly]
        [DefaultValue(null)]
        public string Name;

        [Description("A list of names this parameter can be")]
        [UsedImplicitly]
        [DefaultValue(null)]
        public List<string> Names;

        [Description("Whether or not this parameter can be omitted.")]
        [DefaultValue(false)]
        [UsedImplicitly]
        public bool Optional;

        public override string ToString()
        {
            string name = Names != null ? string.Join("/", Names) : Name.ToStringSafe();
            return Optional ? $"[{name}]" : $"<{name}>";
        }
    }

    [UsedImplicitly]
    public class CommandExtension : DefModExtension
    {
        [Description("A brief overview of what the command does.")]
        [UsedImplicitly]
        public string Description;

        [Description("The arguments this command can handle.")]
        [UsedImplicitly]
        public List<Parameter> Parameters;

        [Description("The permission level a user must have before they can use the command.")]
        [DefaultValue(UserLevels.Anyone)]
        [UsedImplicitly]
        public UserLevels UserLevel;

        public bool HasPermission(Viewer viewer)
        {
            switch (UserLevel)
            {
                case UserLevels.Anyone:
                    return true;

                case UserLevels.Moderator:
                    return viewer.mod || viewer.username.EqualsIgnoreCase(ToolkitSettings.Channel);

                case UserLevels.Subscriber:
                    return viewer.subscriber;

                case UserLevels.Vip:
                    return viewer.vip;

                default:
                    return false;
            }
        }
    }
}
