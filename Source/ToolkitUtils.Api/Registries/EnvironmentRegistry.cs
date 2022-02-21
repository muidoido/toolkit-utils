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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils.Registries
{
    [StaticConstructorOnStartup]
    public static class EnvironmentRegistry
    {
        static EnvironmentRegistry()
        {
            MainThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            MainThreadFactory = new TaskFactory(
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.ExecuteSynchronously,
                MainThreadScheduler
            );
        }

        public static TaskScheduler MainThreadScheduler { get; }
        public static TaskFactory MainThreadFactory { get; }

        public static async Task<T> OnMainThreadAsync<T>([NotNull] this Func<T> func) => await MainThreadFactory.StartNew(func);
    }
}
