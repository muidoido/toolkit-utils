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
using System.Threading.Tasks;
using JetBrains.Annotations;
using LanguageExt;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Registries;
using Verse;

namespace SirRandoo.ToolkitUtils.Converters
{
    public class TraitDefConverter : IConverter<TraitDef>
    {
        /// <inheritdoc/>
        public async Task<Option<TraitDef>> ConvertAsync([NotNull] string source)
        {
            bool isDefName = source.StartsWith(ConventionRegistry.DefNameIndicator);

            var func = new Func<TraitDef>(() => FindTraitDef(source.TrimStart(ConventionRegistry.DefNameIndicator.ToCharArray()), isDefName));
            TraitDef result = await func.OnMainAsync();

            return result == null ? Option<TraitDef>.None : Option<TraitDef>.Some(result);
        }

        [CanBeNull]
        private static TraitDef FindTraitDef(string source, bool defName = true)
        {
            foreach (TraitDef traitDef in DefDatabase<TraitDef>.AllDefs)
            {
                switch (defName)
                {
                    case true when traitDef.defName.Equals(source, StringComparison.OrdinalIgnoreCase):
                        return traitDef;
                    case false when traitDef.label.ToToolkit().Equals(source, StringComparison.CurrentCultureIgnoreCase):
                        return traitDef;
                }
            }

            return null;
        }
    }
}
