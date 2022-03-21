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

using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Interfaces
{
    public interface IProductMetadata<in TD>
    {
        /// <summary>
        ///     The mod the product originated from, if it could be determined.
        /// </summary>
        string Mod { get; set; }

        /// <summary>
        ///     The karma type for purchasing this product.
        /// </summary>
        KarmaType KarmaType { get; set; }

        /// <summary>
        ///     Loads the metadata about a given product.
        /// </summary>
        /// <param name="def">The def to load metadata from</param>
        /// <remarks>
        ///     This method is only called once when a new def is detected, and a
        ///     product line exists for a given def, and once more when the user
        ///     resets a product's metadata to its defaults.
        /// </remarks>
        void LoadFromDef(TD def);
    }
}
