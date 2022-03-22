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

using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Entities
{
    /// <summary>
    ///     Represents a command within the mod's command framework.
    /// </summary>
    public class CommandEntity
    {
        /// <summary>
        ///     The command def associated with the command itself.
        /// </summary>
        public Command Def { get; set; }

        /// <summary>
        ///     The human readable name of the command.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The sub-commands of the given command.
        /// </summary>
        public CommandEntity[] SubCommands { get; set; }

        /// <summary>
        ///     The parameters of the given command.
        /// </summary>
        public ICommandParameter[] Parameters { get; set; }
    }
}
