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

namespace SirRandoo.ToolkitUtils.Enums
{
    /// <summary>
    ///     The various results a call to
    ///     <see cref="IProductManufacturer{TP,TM}.ValidateProduct"/> can
    ///     return.
    /// </summary>
    public enum ValidationResult
    {
        /// <summary>
        ///     The product valid within the given environment.
        /// </summary>
        /// <remarks>
        ///     Indicates to the mod that the product can be spawned in-game, and
        ///     should be available for purchase by viewers.
        /// </remarks>
        Ok,

        /// <summary>
        ///     The product is invalid within the given environment, but should
        ///     be archived for future use.
        /// </summary>
        Archive,

        /// <summary>
        ///     The product is invalid within the given environment, and should
        ///     be deleted.
        /// </summary>
        Remove
    }
}
