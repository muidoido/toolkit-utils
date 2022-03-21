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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Registries;
using Verse;

namespace SirRandoo.ToolkitUtils.Converters
{
    public class ThingDefConverter : IConverter<ThingDef>
    {
        /// <inheritdoc/>
        public async Task<Option<ThingDef>> ConvertAsync([NotNull] string source)
        {
            bool isDefName = source.StartsWith(ConventionRegistry.DefNameIndicator);

            var func = new Func<ThingDef>(() => FindThingDef(source.TrimStart(ConventionRegistry.DefNameIndicator.ToCharArray()), isDefName));
            ThingDef result = await func.OnMainAsync();

            return result == null ? Option<ThingDef>.None : Option<ThingDef>.Some(result);
        }

        [CanBeNull]
        private static ThingDef FindThingDef(string source, bool defName = true)
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                switch (defName)
                {
                    case true when thingDef.defName.Equals(source, StringComparison.OrdinalIgnoreCase):
                        return thingDef;
                    case false when thingDef.label.ToToolkit().Equals(source, StringComparison.CurrentCultureIgnoreCase):
                        return thingDef;
                }
            }

            return null;
        }
    }
}
