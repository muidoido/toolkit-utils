// ToolkitUtils.Core
// Copyright (C) 2021  SirRandoo
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
using SirRandoo.ToolkitUtils.Registries;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    [StaticConstructorOnStartup]
    public static class TranslationHelper
    {
        [ItemNotNull]
        public static async Task<string> TranslateSimpleAsync(this string source)
        {
            var func = new Func<string>(source.TranslateSimple);

            return await func.OnMainAsync();
        }

        [ItemNotNull]
        public static async Task<string> TranslateWithBackupAsync(this string source, string backup)
        {
            var func = new Func<string>(() => source.TranslateWithBackup(backup));

            return await func.OnMainAsync();
        }

        public static async Task<string> TranslateAsync(this string source, params NamedArgument[] arguments)
        {
            var func = new Func<string>(() => source.Translate(arguments));

            return await func.OnMainAsync();
        }

        public static async Task<bool> CanTranslateAsync(this string source)
        {
            var func = new Func<bool>(source.CanTranslate);

            return await func.OnMainAsync();
        }
    }
}
