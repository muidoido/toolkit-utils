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
using System.Text;
using System.Threading.Tasks;
using CommonLib.Entities;
using CommonLib.Interfaces;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Flags;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Workers
{
    /// <summary>
    ///     A class for executing commands asynchronously, while still
    ///     retaining old synchronous behavior.
    /// </summary>
    public class CommandWorker
    {
        private static readonly IRimLogger Logger = new RimThreadedLogger("tku.commands");
        private readonly Command _command;

        public CommandWorker(Command command)
        {
            _command = command;
        }

        /// <summary>
        ///     Determines whether or not the given user can execute the given
        ///     command.
        /// </summary>
        /// <param name="context">The context of the command's execution</param>
        /// <returns>Whether or not the user can use this command</returns>
        [NotNull]
        public Task<bool> CanExecute([NotNull] IContext context)
        {
            if (!(context.User is IViewer viewer) || (_command.requiresAdmin && (viewer.UserType & UserTypes.Broadcaster) == 0))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(!_command.requiresMod || ((viewer.UserType & UserTypes.Broadcaster) != 0 && (viewer.UserType & UserTypes.Moderator) != 0));
        }

        [NotNull]
        public async Task Execute(IContext context)
        {
            if (ToolkitSettings.CustomCommandDefs.Contains(_command.defName))
            {
                Logger.Warn(
                    new StringBuilder().Append($@"The command ""{_command.command}"" cannot be executed from this context; ")
                       .Append("it uses Twitch Toolkit's custom command system, and as a result, requires a parameter unique to TwitchLib.")
                       .ToString()
                );

                return;
            }

            if (!(Activator.CreateInstance(_command.commandDriver) is ICommand instance))
            {
                Logger.Warn(
                    $@"The command ""{_command.command}"" cannot be instantiated; it inherits from {_command.commandDriver.BaseType.FullDescription()} instead of {typeof(CommandDriver).FullDescription()}"
                );

                return;
            }

            await instance.ExecuteAsync(context);
        }

        /// <summary>
        ///     Executes the command asynchronously if a valid implementation
        ///     exists, or synchronously if no other implementation exists.
        /// </summary>
        /// <param name="message">
        ///     The original Twitch message, as received from
        ///     TwitchLib
        /// </param>
        /// <param name="context">The context of the given command's execution</param>
        public async Task Execute(ITwitchMessage message, IContext context)
        {
            if (!(Activator.CreateInstance(_command.commandDriver) is CommandDriver instance))
            {
                Logger.Warn(
                    $@"The command ""{_command.command}"" cannot be instantiated; it inherits from {_command.commandDriver.BaseType.FullDescription()} instead of {typeof(CommandDriver).FullDescription()}"
                );

                return;
            }

            if (instance is ICommand command)
            {
                await command.ExecuteAsync(context);
            }
            else
            {
                instance.RunCommand(message);
            }
        }
    }
}
