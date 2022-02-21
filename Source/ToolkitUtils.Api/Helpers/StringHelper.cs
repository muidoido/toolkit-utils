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
using System.Globalization;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class StringHelper
    {
        public static bool Contains([NotNull] this string source, [NotNull] string text, [NotNull] CultureInfo info) => info.CompareInfo.IndexOf(source, text) >= 0;

        [NotNull]
        public static string RemoveWhitespace([NotNull] this string source) => source.Trim().Replace(" ", "").Replace("\t", "").Replace("\n", "");

        [NotNull]
        public static Tuple<string, string> SplitBy([NotNull] this string source, params char[] by)
        {
            string[] s = source.Split(by, 1, StringSplitOptions.RemoveEmptyEntries);

            if (s.Length <= 0)
            {
                return Tuple.Create("", "");
            }

            var first = "";
            var last = "";

            for (var i = 0; i < s.Length; i++)
            {
                if (string.IsNullOrEmpty(first))
                {
                    first = s[i];

                    continue;
                }

                last = s[i];
            }

            return Tuple.Create(first, last);
        }
    }
}
