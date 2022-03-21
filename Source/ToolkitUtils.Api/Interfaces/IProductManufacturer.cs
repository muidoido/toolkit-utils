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

namespace SirRandoo.ToolkitUtils.Interfaces
{
    /// <summary>
    ///     A class that's responsible for creating the product a viewer is
    ///     purchasing in-game, as well as creating an abstract object for
    ///     drawing the product's settings when applicable.
    /// </summary>
    /// <typeparam name="TP">The in-game product the viewer is purchasing</typeparam>
    /// <typeparam name="TM">The associated metadata for the product</typeparam>
    public interface IProductManufacturer<TP, TM> where TM : IProductMetadata<TP>
    {
        /// <summary>
        ///     Creates the product in-game.
        /// </summary>
        /// <param name="context">
        ///     The context for the product's creation as given
        ///     by the framework, and the viewer
        /// </param>
        /// <param name="product">The product the viewer purchased</param>
        /// <returns></returns>
        Task<bool> ManufactureAsync(ICreationContext context, IStoreProduct<TP, TM> product);

        /// <summary>
        ///     Whether or not the product can be manufactured in-game.
        /// </summary>
        /// <param name="viewer">The viewer the product is for</param>
        /// <param name="product">The product being purchased by the viewer</param>
        /// <returns>Whether or not the product can be manufactured</returns>
        Task<bool> CanManufactureAsync(IViewer viewer, IStoreProduct<TP, TM> product);

        /// <summary>
        ///     Called when the product is first loaded to ensure
        /// </summary>
        /// <param name="product">The product to validate</param>
        /// <returns></returns>
        bool ValidateProduct(IStoreProduct<TP, TM> product);

        IProductInfo<TP, TM> GetProductInfo(IStoreProduct<TP, TM> product);
    }
}
