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

using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class GiveCoins : CommandBase
    {
        public override async Task RunCommandAsync([NotNull] IContext context)
        {
            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(context.Message).Skip(1));

            if (!worker.TryGetNextAsViewer(out Viewer viewer) || !worker.TryGetNextAsInt(out int amount))
            {
                return;
            }

            viewer.GiveViewerCoins(amount);
            Store_Logger.LogGiveCoins(context.User.Username, viewer.username, amount);

            string response = await "TKUtils.GiveCoins.Done".TranslateAsync(amount.ToString("N0"), viewer.username, viewer.GetViewerCoins().ToString("N0"));

            await context.Reply(response);
        }
    }
}
