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
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Interfaces
{
    public interface IWarehouse<TD, TM> where TM : IProductMetadata<TD>
    {
        IEnumerable<IStoreProduct<TD, TM>> AllProducts { get; }


        /// <summary>
        ///     Registers a product within the warehouse.
        /// </summary>
        /// <param name="product">The product to register to the warehouse</param>
        void Register([NotNull] IStoreProduct<TD, TM> product);

        /// <summary>
        ///     Unregisters a product from the warehouse.
        /// </summary>
        /// <param name="id">The id of the product</param>
        void Unregister([NotNull] string id);

        /// <summary>
        ///     Gets a product within the warehouse.
        /// </summary>
        /// <param name="id">The id of the product</param>
        /// <returns>
        ///     The product if it's registered to the warehouse, or
        ///     <c>null</c>
        /// </returns>
        IStoreProduct<TD, TM> Get([NotNull] string id);

        /// <summary>
        ///     Finds a product within the warehouse by the product code.
        /// </summary>
        /// <param name="query">The text to search for</param>
        /// <param name="comparison">
        ///     The comparison method to use when comparing
        ///     codes
        /// </param>
        /// <returns>
        ///     The product if it was found in the warehouse, or <c>null</c>
        /// </returns>
        IStoreProduct<TD, TM> FindByCode([NotNull] string query, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Finds a product within the warehouse by the product name.
        /// </summary>
        /// <param name="query">The text to search for</param>
        /// <param name="comparison">
        ///     The comparison method to use for comparing
        ///     names
        /// </param>
        /// <returns>
        ///     The product if it was found in the warehouse, or <c>null</c>
        /// </returns>
        IStoreProduct<TD, TM> FindByName([NotNull] string query, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Searches for a product that matches the given query according to
        ///     the product name.
        /// </summary>
        /// <param name="query">The text to search for</param>
        /// <returns>An enumerable of products that were found in the warehouse</returns>
        [NotNull]
        [ItemNotNull]
        IEnumerable<IStoreProduct<TD, TM>> SearchByName([NotNull] string query);

        /// <summary>
        ///     Searches for a product that matches a given query according to
        ///     the product code.
        /// </summary>
        /// <param name="query">The text to search for</param>
        /// <returns>An enumerable of products that were found in the warehouse</returns>
        [NotNull]
        [ItemNotNull]
        IEnumerable<IStoreProduct<TD, TM>> SearchByCode([NotNull] string query);
    }
}
