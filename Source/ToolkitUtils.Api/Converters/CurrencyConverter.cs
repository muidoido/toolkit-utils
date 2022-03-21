﻿// ToolkitUtils.Api
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

using System.Globalization;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LanguageExt;
using SirRandoo.ToolkitUtils.Interfaces;

namespace SirRandoo.ToolkitUtils.Converters
{
    public class CurrencyConverter : IConverter<int>
    {
        [NotNull]
        public Task<Option<int>> ConvertAsync(string source) =>
            !int.TryParse(source, NumberStyles.AllowExponent | NumberStyles.Currency | NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out int result)
                ? Task.FromResult(Option<int>.None)
                : Task.FromResult(Option<int>.Some(result));
    }
}
