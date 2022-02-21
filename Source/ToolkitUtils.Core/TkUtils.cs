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
using CommonLib;
using CommonLib.Entities;
using CommonLib.Interfaces;
using CommonLib.Windows;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class TkUtils : ModPlus
    {
        public TkUtils(ModContentPack content) : base(content)
        {
            Instance = this;
            Settings = GetSettings<TkSettings>();
            Logger = new RimThreadedLogger(Content.Name);
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(TkUtilsWindow)));
        }

        public TkSettings Settings { get; }
        public static TkUtils Instance { get; private set; }
        public static IRimLogger Logger { get; private set; }

        [NotNull]
        protected override ProxySettingsWindow SettingsWindow => new ProxySettingsWindow(this);

        internal static void HandleException([NotNull] Exception exception, [CanBeNull] string reporter = null)
        {
            HandleException(exception.Message ?? "An unhandled exception occurred", exception, reporter);
        }

        internal static void HandleException(string message, Exception exception, [CanBeNull] string reporter = null)
        {
            if (TkSettings.VisualExceptions && VisualExceptions.Active)
            {
                VisualExceptions.HandleException(exception);

                return;
            }

            string exceptionMessage = message ?? exception.Message ?? "An unhandled exception occurred";
            Logger.Error(exceptionMessage, exception);

            Data.HealthReports.Add(
                new HealthReport
                {
                    Message = $"{exceptionMessage} :: Reason: {exception.GetType().Name}({exception.Message})", Stacktrace = StackTraceUtility.ExtractStringFromException(exception),
                    Type = HealthReport.ReportType.Error, OccurredAt = DateTime.Now,
                    Reporter = reporter ?? "Unknown"
                }
            );
        }
    }
}
