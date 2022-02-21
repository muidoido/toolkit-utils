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
using System.Threading.Tasks;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    public class CommandContext : IContext
    {
        /// <inheritdoc/>
        public string Prefix { get; set; }

        /// <inheritdoc/>
        public bool UseEmojis { get; set; }

        /// <inheritdoc/>
        public IUserData User { get; set; }

        /// <inheritdoc/>
        public string Message { get; set; }

        /// <inheritdoc/>
        public Command Command { get; set; }

        /// <inheritdoc/>
        [NotNull]
        public Task Reply(string message)
        {
            string response = UseEmojis ? $"{User.DisplayName} {ResponseHelper.ArrowGlyph} {message}" : $"{User.DisplayName} -> {message}";

            CacheWorker.Instance.Store(Message.ToLowerInvariant(), response, TimeSpan.FromMinutes(1));
            TwitchWrapper.SendChatMessage(response);

            return Task.CompletedTask;
        }
    }
}
