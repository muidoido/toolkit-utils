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
using SirRandoo.ToolkitUtils.Flags;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Utilities;

namespace SirRandoo.ToolkitUtils.Models
{
    public class UserData : IUserData, IEquatable<IUserData>
    {
        private Viewer _viewerData;

        public bool Equals(IUserData other) => other != null && Id == other.Id;

        /// <inheritdoc cref="IUserData.Id"/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public int Coins
        {
            get => ViewerData.GetViewerCoins();
            set => ViewerData.SetViewerCoins(value);
        }

        /// <inheritdoc/>
        public int Karma
        {
            get => ViewerData.GetViewerKarma();
            set => ViewerData.SetViewerKarma(value);
        }

        /// <inheritdoc/>
        public int Income
        {
            get
            {
                int baseCoins = ToolkitSettings.CoinAmount;
                float multiplier = Karma / 100f;

                if (IsSubscriber)
                {
                    baseCoins += ToolkitSettings.SubscriberExtraCoins;
                    multiplier *= ToolkitSettings.SubscriberCoinMultiplier;
                }
                else if (IsVip)
                {
                    baseCoins += ToolkitSettings.VIPExtraCoins;
                    multiplier *= ToolkitSettings.VIPCoinMultiplier;
                }
                else if (IsModerator)
                {
                    baseCoins += ToolkitSettings.ModExtraCoins;
                    multiplier *= ToolkitSettings.ModCoinMultiplier;
                }

                int minutesElapsed = TimeHelper.MinutesElapsed(ViewerData.last_seen);

                if (ToolkitSettings.ChatReqsForCoins)
                {
                    if (minutesElapsed > ToolkitSettings.TimeBeforeHalfCoins)
                    {
                        multiplier *= 0.5f;
                    }

                    if (minutesElapsed > ToolkitSettings.TimeBeforeNoCoins)
                    {
                        multiplier *= 0.0f;
                    }
                }

                return (int)Math.Ceiling((double)baseCoins * multiplier);
            }
        }

        /// <inheritdoc cref="IUserData.Username"/>
        public string Username { get; set; }

        /// <inheritdoc/>
        public DateTime LastSeen
        {
            get => ViewerData.last_seen;
            set => ViewerData.last_seen = value;
        }

        /// <inheritdoc cref="IUserData.DisplayName"/>
        public string DisplayName { get; set; }

        /// <inheritdoc cref="IUserData.UserType"/>
        public UserTypes UserType { get; set; }

        /// <inheritdoc/>
        public Viewer ViewerData
        {
            get => _viewerData ??= Viewers.GetViewer(Username);
            set => _viewerData = value;
        }

        public bool IsModerator => (UserType & UserTypes.Moderator) != 0 || (UserType & UserTypes.Broadcaster) != 0;
        public bool IsBroadcaster => (UserType & UserTypes.Broadcaster) != 0;
        public bool IsSubscriber => (UserType & UserTypes.Subscriber) != 0 || (UserType & UserTypes.Founder) != 0;
        public bool IsVip => (UserType & UserTypes.Vip) != 0;
    }
}
