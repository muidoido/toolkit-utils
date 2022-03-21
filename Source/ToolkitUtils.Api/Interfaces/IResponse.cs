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

using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Interfaces
{
    /// <summary>
    ///     Represents a response from a command.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        ///     The raw input that generated this response.
        /// </summary>
        string RawInput { get; set; }

        /// <summary>
        ///     Requests that the response be turned into a string.
        /// </summary>
        /// <returns>The response as a string</returns>
        /// <remarks>
        ///     This method is only called when sending a response object to
        ///     Twitch chat.
        /// </remarks>
        [NotNull]
        Task<string> AsTextAsync();
    }
}
