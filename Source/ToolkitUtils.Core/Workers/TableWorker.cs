﻿// ToolkitUtils.Core
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
using System.Collections.Generic;

namespace SirRandoo.ToolkitUtils.Workers
{
    public abstract class TableWorker<T> : TableWorkerBase
    {
        private protected List<T> InternalData;

        public IEnumerable<T> Data => InternalData;

        public abstract void EnsureExists(T data);
        public abstract void NotifyGlobalDataChanged();
        public abstract void NotifyCustomSearchRequested(Func<T, bool> worker);

        protected enum StateKey { Enable, Disable }

        protected enum SettingsKey { Expand, Collapse }
    }
}
