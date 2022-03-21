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
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LanguageExt;
using SirRandoo.ToolkitUtils.Interfaces;

namespace SirRandoo.ToolkitUtils.Converters
{
    public class EnumConverter<T> : IConverter<T> where T : struct, Enum
    {
        [NotNull]
        public Task<Option<T>> ConvertAsync(string source)
        {
            Type t = typeof(T);
            TypeInfo ti = t.GetTypeInfo();

            if (!ti.IsEnum)
            {
                return Task.FromResult(Option<T>.None);
            }

            return Enum.TryParse(source, true, out T ev) ? Task.FromResult(Option<T>.Some(ev)) : Task.FromResult(Option<T>.None);
        }
    }
}
