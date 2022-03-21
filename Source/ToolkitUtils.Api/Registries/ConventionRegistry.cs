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

namespace SirRandoo.ToolkitUtils.Registries
{
    /// <summary>
    ///     A "readonly" class for housing syntax conventions used throughout
    ///     the mod. Viewers are expected to use these conventions when they
    ///     issue commands through the mod.
    /// </summary>
    public static class ConventionRegistry
    {
        /// <summary>
        ///     The character a viewer must type in order to signal to the mod
        ///     that their command involved a product's "def name" instead of its
        ///     name.
        /// </summary>
        public const string DefNameIndicator = "#";

        /// <summary>
        ///     The character a viewer may type in order to signal to the mod
        ///     that their command involved a viewer's name.
        /// </summary>
        /// <remarks>
        ///     The associated code for this indicator makes various assumptions
        ///     on what a given viewer intended based on how the command's
        ///     signature is defined. Whether this indicator is provided or not,
        ///     a command with a proper signature will always receive the
        ///     associated viewer for the given command, whether it be through
        ///     <see cref="IContext"/>, or from a parameter in the method itself.
        /// </remarks>
        public const string UsernameIndicator = "@";
    }
}
