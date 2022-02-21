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

using RimWorld;
using SirRandoo.ToolkitUtils.Entities;
using SirRandoo.ToolkitUtils.Enums;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Registries
{
    /// <summary>
    ///     A class for housing enums the mod uses for various functions
    ///     within itself.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class EnumRegistry
    {
        public static readonly EnumRegistrar<Gender> Genders = new EnumRegistrar<Gender>();
        public static readonly EnumRegistrar<KarmaType> KarmaTypes = new EnumRegistrar<KarmaType>();
        public static readonly EnumRegistrar<TechLevel> TechLevels = new EnumRegistrar<TechLevel>();
        public static readonly EnumRegistrar<ComparisonType> ComparisonTypes = new EnumRegistrar<ComparisonType>();
        public static readonly EnumRegistrar<QualityCategory> QualityCategories = new EnumRegistrar<QualityCategory>();
    }
}
