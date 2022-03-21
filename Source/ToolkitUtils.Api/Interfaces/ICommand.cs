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

namespace SirRandoo.ToolkitUtils.Interfaces
{
    /// <summary>
    ///     An interface for defining commands within the mod.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///     Called prior immediately before <see cref="ExecuteAsync"/>.
        /// </summary>
        /// <param name="context">The given context of the command's execution</param>
        /// <returns>Whether or not the command should be invoked</returns>
        Task<bool> BeforeExecuteAsync(IContext context);

        /// <summary>
        ///     Called immediately after <see cref="BeforeExecuteAsync"/>, if it
        ///     returns <c>true</c>.
        /// </summary>
        /// <param name="context">The given context of the command's execution</param>
        /// <returns>The associated response of the message.</returns>
        Task<IResponse> ExecuteAsync(IContext context);

        /// <summary>
        ///     Called immediately after <see cref="ExecuteAsync"/>.
        /// </summary>
        /// <param name="context">The given context of the command's execution</param>
        /// <param name="response">
        ///     The <see cref="IResponse"/> object returned
        ///     from <see cref="ExecuteAsync"/>
        /// </param>
        /// <param name="exception">
        ///     The exception that was thrown when the
        ///     command was executed
        /// </param>
        Task AfterExecuteAsync(IContext context, IResponse response, Exception exception = null);
    }
}
