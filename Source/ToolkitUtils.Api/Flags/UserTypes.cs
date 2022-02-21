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

namespace SirRandoo.ToolkitUtils.Flags
{
    /// <summary>
    ///     The various user types that a user may be at
    ///     any given moment.
    /// </summary>
    [Flags]
    public enum UserTypes : short
    {
        /// <summary>
        ///     The user has no significant user types.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The user is a subscriber in the given channel.
        /// </summary>
        Subscriber = 1,

        /// <summary>
        ///     The user is a founder in the given channel.
        /// </summary>
        /// <remarks>
        ///     This includes <see cref="Subscriber"/> implicitly.
        /// </remarks>
        Founder = 2 | Subscriber,

        /// <summary>
        ///     The user is a vip in the given channel.
        /// </summary>
        Vip = 4,

        /// <summary>
        ///     The user is a moderator in the given channel.
        /// </summary>
        Moderator = 8,

        /// <summary>
        ///     The user is the broadcaster in the given channel.
        /// </summary>
        Broadcaster = 16
    }
}
