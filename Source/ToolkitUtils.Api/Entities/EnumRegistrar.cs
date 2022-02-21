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
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Entities
{
    /// <summary>
    ///     A class for indexing a given enum for use in enumeration and case
    ///     insensitive keyed access.
    /// </summary>
    /// <typeparam name="T">The enum to index</typeparam>
    public class EnumRegistrar<T> : IReadOnlyCollection<T>, IReadOnlyDictionary<string, T> where T : Enum
    {
        private readonly List<T> _entities = new List<T>();
        private readonly Dictionary<string, T> _entitiesKeyed = new Dictionary<string, T>();

        public EnumRegistrar()
        {
            foreach (string name in Enum.GetNames(typeof(T)))
            {
                var inst = (T)Enum.Parse(typeof(T), name);

                _entities.Add(inst);
                _entitiesKeyed.Add(name.ToLowerInvariant(), inst);
            }
        }

        /// <inheritdoc cref="IReadOnlyCollection{T}.GetEnumerator"/>
        public IEnumerator<T> GetEnumerator() => _entities.GetEnumerator();

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
        public int Count => _entities.Count;

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.GetEnumerator"/>
        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator() => _entitiesKeyed.GetEnumerator();

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.ContainsKey"/>
        public bool ContainsKey([NotNull] string key) => _entitiesKeyed.ContainsKey(key.ToLowerInvariant());

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.TryGetValue"/>
        public bool TryGetValue(string key, [CanBeNull] out T value) => _entitiesKeyed.TryGetValue(key.ToLowerInvariant(), out value);

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.this"/>
        public T this[[NotNull] string key] => _entitiesKeyed[key.ToLowerInvariant()];

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.Keys"/>
        public IEnumerable<string> Keys => _entitiesKeyed.Keys;

        /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.Values"/>
        public IEnumerable<T> Values => _entitiesKeyed.Values;
    }
}
