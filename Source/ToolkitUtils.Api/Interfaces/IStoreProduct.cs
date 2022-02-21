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
    ///     An interface for outlining the data necessary
    ///     for purchasable goods within Toolkit's store.
    /// </summary>
    public interface IStoreProduct
    {
        /// <summary>
        ///     The cost of the product.
        /// </summary>
        int Cost { get; set; }

        /// <summary>
        ///     The human readable name of the product.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     The text a viewer must type in order to purchase this product.
        /// </summary>
        string PurchaseCode { get; set; }

        /// <summary>
        ///     The internal id of the product.
        /// </summary>
        string DefName { get; set; }

        /// <summary>
        ///     Whether or not the product is currently purchasable.
        /// </summary>
        bool Purchasable { get; set; }

        /// <summary>
        /// </summary>
        string Mod { get; set; }
    }
}
