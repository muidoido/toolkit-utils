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

using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class Balance : CommandBase
    {
        private readonly string _maxIntString = int.MaxValue.ToString("N0");

        public override async Task RunCommandAsync([NotNull] IContext context)
        {
            var builder = new StringBuilder();

            builder.Append(await CoinsText(context));
            builder.Append(": ");

            if (ToolkitSettings.UnlimitedCoins)
            {
                builder.Append(context.UseEmojis ? ResponseHelper.InfinityGlyph : _maxIntString);
            }
            else
            {
                builder.Append(context.User.Coins.ToString("N0"));
            }

            builder.Append(context.UseEmojis ? ResponseHelper.OuterGroupSeparator : ResponseHelper.OuterGroupSeparatorAlt);
            builder.Append(await KarmaText(context));
            builder.Append((context.User.Karma / 100f).ToString("P0"));

            if (ToolkitSettings.EarningCoins && TkSettings.ShowCoinRate)
            {
                builder.Append(context.UseEmojis ? ResponseHelper.OuterGroupSeparator : ResponseHelper.OuterGroupSeparatorAlt);
                builder.Append(await IncomeText(context));
            }

            await context.Reply(builder.ToString());
        }

        private static async Task<string> CoinsText([NotNull] IContext context) =>
            context.UseEmojis ? ResponseHelper.CoinGlyph : (await "TKUtils.Balance.Coins".TranslateSimpleAsync()).CapitalizeFirst();

        private static async Task<string> KarmaText([NotNull] IContext context) =>
            context.UseEmojis ? ResponseHelper.KarmaGlyph : (await "TKUtils.Balance.Karma".TranslateSimpleAsync()).CapitalizeFirst();

        private static async Task<string> IncomeText([NotNull] IContext context)
        {
            int income = context.User.Income;

            if (context.UseEmojis)
            {
                return income > 0 ? $"{ResponseHelper.IncomeGlyph} +{income:N0}" : $"{ResponseHelper.DebtGlyph} {income:N0}";
            }

            return await "TKUtils.Balance.Rate".TranslateAsync(income.ToString("N0"), ToolkitSettings.CoinInterval.ToString("N0"));
        }
    }
}
