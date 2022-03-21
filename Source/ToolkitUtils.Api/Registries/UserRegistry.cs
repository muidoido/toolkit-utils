// ToolkitUtils.Api
// Copyright (C) 2022  SirRandoo
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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Flags;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Registries
{
    public static class UserRegistry
    {
        private static readonly List<IUser> UserData = new List<IUser>();
        private static readonly Dictionary<string, IUser> UserDataKeyed = new Dictionary<string, IUser>();

        [CanBeNull]
        public static IUser Get([NotNull] string id)
        {
            lock (UserData)
            {
                return UserDataKeyed.TryGetValue(id, out IUser data) ? data : null;
            }
        }

        [CanBeNull]
        public static IUser Get([NotNull] ITwitchMessage message)
        {
            TwitchLibMessage mess = (TwitchLibMessage)message.WhisperMessage ?? message.ChatMessage;

            return Get(mess.UserId);
        }

        public static IUser GetByName([NotNull] string name)
        {
            lock (UserData)
            {
                return UserData.Find(d => d.DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        [ContractAnnotation("=> false,data:null; => true,data:notnull")]
        public static bool TryGet([NotNull] string id, out IUser data)
        {
            data = Get(id);

            return data != null;
        }

        [ContractAnnotation("=> false,data:null; => true,data:notnull")]
        public static bool TryGet([NotNull] ITwitchMessage message, out IUser data)
        {
            data = Get(message);

            return data != null;
        }

        [ContractAnnotation("=> false,data:null; => true,data:notnull")]
        public static bool TryGetByName([NotNull] string name, out IUser data)
        {
            data = Get(name);

            return data != null;
        }

        [NotNull]
        public static IUser Update([NotNull] ITwitchMessage message)
        {
            lock (UserData)
            {
                IUser data = CreateViewer(message);

                UserDataKeyed[data.Id] = data;
                UserData.Add(data);

                return data;
            }
        }

        [NotNull]
        private static IUser CreateViewer([NotNull] ITwitchMessage message)
        {
            TwitchLibMessage mess = (TwitchLibMessage)message.WhisperMessage ?? message.ChatMessage;

            var data = new Viewer
            {
                Id = mess.UserId,
                Username = mess.Username,
                DisplayName = string.IsNullOrEmpty(mess.DisplayName) ? mess.Username.CapitalizeFirst() : mess.DisplayName,
                UserType = UserTypes.None
            };

            foreach (KeyValuePair<string, string> pair in mess.Badges)
            {
                switch (pair.Key.ToLowerInvariant())
                {
                    case "broadcaster":
                        data.UserType |= UserTypes.Broadcaster;

                        break;
                    case "mod":
                        data.UserType |= UserTypes.Moderator;

                        break;
                    case "subscriber":
                        data.UserType |= UserTypes.Subscriber;

                        break;
                    case "founder":
                        data.UserType |= UserTypes.Founder;

                        break;
                    case "vip":
                        data.UserType |= UserTypes.Vip;

                        break;
                }
            }

            return data;
        }

        [ContractAnnotation("=> true,data:notnull; => false,data:null")]
        private static bool Remove([NotNull] string id, out IUser data)
        {
            lock (UserData)
            {
                if (!UserDataKeyed.TryGetValue(id, out data))
                {
                    return false;
                }

                UserDataKeyed.Remove(id);
                UserData.Remove(data);

                return true;
            }
        }
    }
}
