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
using SirRandoo.ToolkitUtils.Interfaces;

namespace SirRandoo.ToolkitUtils.Attributes
{
    /// <summary>
    ///     Decorates methods within an <see cref="ICommand"/> as a
    ///     sub-command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SubcommandAttribute : Attribute
    {
        /// <summary>
        ///     The text a viewer has to type after a command's text in order to
        ///     execute the attached sub-command method.
        /// </summary>
        public string Name;

        public SubcommandAttribute(string name)
        {
            Name = name;
        }
    }
}
