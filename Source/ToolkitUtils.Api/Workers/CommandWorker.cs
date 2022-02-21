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
using System.Threading.Tasks;
using CommonLib.Entities;
using CommonLib.Interfaces;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Flags;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Registries;
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
        /// <param name="userId">The id of the user executing the command</param>
        /// <returns>Whether or not the user can use this command</returns>
        [NotNull]
        public Task<bool> CanExecute([NotNull] string userId)
        {
            if (!UserRegistry.TryGet(userId, out IUserData data))
            {
                return Task.FromResult(false);
            }

            if (_command.requiresAdmin && (data.UserType & UserTypes.Broadcaster) == 0)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(!_command.requiresMod || ((data.UserType & UserTypes.Broadcaster) != 0 && (data.UserType & UserTypes.Moderator) != 0));
        }

        /// <summary>
        ///     Executes the command asynchronously if a valid implementation
        ///     exists,
        ///     or synchronously if no other implementation exists.
        /// </summary>
        /// <param name="message"></param>
        public async Task Execute(ITwitchMessage message)
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
                var ctx = new CommandContext { User = UserRegistry.Get(message) };

                await command.RunCommandAsync(ctx);
            }
            else
            {
                instance.RunCommand(message);
            }
        }
    }
}
