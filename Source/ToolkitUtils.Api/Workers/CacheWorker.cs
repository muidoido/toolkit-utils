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
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class CacheWorker : IDisposable
    {
        private static CacheWorker _instance;
        private readonly List<CacheIndex> _cache = new List<CacheIndex>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly Timer _timer;

        private CacheWorker()
        {
            _timer = new Timer(ValidateCache, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        [NotNull]
        public static CacheWorker Instance => _instance ??= new CacheWorker();

        public void Dispose()
        {
            _lock?.Dispose();
            _timer?.Dispose();
            _instance = null;
        }

        public string Get(string key)
        {
            if (!_lock.TryEnterReadLock(TimeSpan.FromMilliseconds(250)))
            {
                return null;
            }

            for (int i = _cache.Count - 1; i >= 0; i--)
            {
                CacheIndex index = _cache[i];

                if (!string.Equals(key, index.Key, StringComparison.Ordinal))
                {
                    continue;
                }

                _lock.ExitReadLock();

                return index.Value;
            }

            _lock.ExitReadLock();

            return null;
        }

        public void Store(string key, string value, TimeSpan expiresIn)
        {
            if (!_lock.TryEnterWriteLock(TimeSpan.FromMilliseconds(250)))
            {
                return;
            }

            for (int i = _cache.Count - 1; i >= 0; i--)
            {
                if (!string.Equals(key, _cache[i].Key, StringComparison.Ordinal))
                {
                    continue;
                }

                _cache[i] = new CacheIndex { Key = key, Value = value, ExpiresIn = expiresIn };
                _lock.ExitWriteLock();

                return;
            }

            _cache.Add(new CacheIndex { Key = key, Value = value, ExpiresIn = expiresIn });
            _lock.ExitWriteLock();
        }

        private void ValidateCache(object state)
        {
            if (!_lock.TryEnterWriteLock(TimeSpan.FromMilliseconds(500)))
            {
                return;
            }

            for (int i = _cache.Count - 1; i >= 0; i--)
            {
                CacheIndex index = _cache[i];

                double totalSeconds = index.ExpiresIn.TotalSeconds;

                if (totalSeconds <= 0)
                {
                    _cache.RemoveAt(i);

                    continue;
                }

                _cache[i] = new CacheIndex { Key = index.Key, Value = index.Value, ExpiresIn = TimeSpan.FromSeconds(totalSeconds) };
            }

            _lock.ExitWriteLock();
        }

        private struct CacheIndex
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public TimeSpan ExpiresIn { get; set; }
        }
    }
}
