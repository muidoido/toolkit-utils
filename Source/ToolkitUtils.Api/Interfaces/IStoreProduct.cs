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

namespace SirRandoo.ToolkitUtils.Interfaces
{
    /// <summary>
    ///     An interface for outlining the data necessary for purchasable
    ///     goods within Toolkit's store.
    /// </summary>
    public interface IStoreProduct<TP, TD> where TD : IProductMetadata<TP>
    {
        /// <summary>
        ///     The product the viewer is purchasing in-game.
        /// </summary>
        TP Product { get; set; }

        /// <summary>
        ///     The product type's associated metadata.
        /// </summary>
        TD Metadata { get; set; }

        /// <summary>
        ///     The cost of the product.
        /// </summary>
        int Cost { get; set; }

        /// <summary>
        ///     The human readable name of the product.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The text a viewer must type in order to purchase this product.
        /// </summary>
        /// <remarks>
        ///     This should typically be <see cref="Name"/> without spaces. The
        ///     mod naturally compares purchase codes case insensitively.
        /// </remarks>
        string Code { get; set; }

        /// <summary>
        ///     The internal id of the product.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Whether or not the product is currently purchasable.
        /// </summary>
        bool Purchasable { get; set; }

        /// <summary>
        ///     Loads the data from a given def.
        /// </summary>
        /// <param name="def">The def to load metadata from</param>
        /// <remarks>
        ///     This method is only called once when a new def is detected, and a
        ///     product line exists for a given def, and once more when the user
        ///     resets a product's data to its defaults.
        /// </remarks>
        void LoadFromDef(TP def);
    }
}
