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

using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, IUserData> UserData = new ConcurrentDictionary<string, IUserData>();

        [CanBeNull]
        public static IUserData Get([NotNull] string id) => UserData.TryGetValue(id, out IUserData data) ? data : null;

        [CanBeNull]
        public static IUserData Get([NotNull] ITwitchMessage message)
        {
            TwitchLibMessage mess = (TwitchLibMessage)message.WhisperMessage ?? message.ChatMessage;

            return Get(mess.UserId);
        }

        [ContractAnnotation("=> false,data:null; => true,data:notnull")]
        public static bool TryGet([NotNull] string id, out IUserData data)
        {
            data = Get(id);

            return data != null;
        }

        [ContractAnnotation("=> false,data:null; => true,data:notnull")]
        public static bool TryGet([NotNull] ITwitchMessage message, out IUserData data)
        {
            data = Get(message);

            return data != null;
        }

        public static IUserData Update([NotNull] ITwitchMessage message)
        {
            TwitchLibMessage mess = (TwitchLibMessage)message.WhisperMessage ?? message.ChatMessage;

            return UserData.AddOrUpdate(mess.UserId, key => CreateUserData(message), (key, oldValue) => CreateUserData(message));
        }

        [NotNull]
        private static IUserData CreateUserData([NotNull] ITwitchMessage message)
        {
            TwitchLibMessage mess = (TwitchLibMessage)message.WhisperMessage ?? message.ChatMessage;

            var data = new UserData
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
        private static bool Remove([NotNull] string id, out IUserData data) => UserData.TryRemove(id, out data);
    }
}
