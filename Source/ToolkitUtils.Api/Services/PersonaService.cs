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
using SirRandoo.ToolkitUtils.Defs;

namespace SirRandoo.ToolkitUtils.Services
{
    /// <summary>
    ///     A service for housing the <see cref="PersonaThingComp"/>s for all
    ///     viewers linked within the mod. Implementors looking to interact
    ///     with linked pawns should use this service, and its relevant
    ///     methods, to obtain the pawn a viewer is linked to.
    /// </summary>
    /// <remarks>
    ///     To maintain backwards compatibility, a method exists to return
    ///     personas by a user's username, as well as their display name.
    /// </remarks>
    public static class PersonaService
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
        private static readonly List<PersonaThingComp> Personas = new List<PersonaThingComp>();

        /// <summary>
        ///     Gets a persona for a given user.
        /// </summary>
        /// <param name="userId">The id of the user the persona is for.</param>
        /// <returns>
        ///     The persona of the user, or <c>null</c> if they don't have a
        ///     persona
        /// </returns>
        public static PersonaThingComp GetPersona(string userId)
        {
            if (!Lock.TryEnterReadLock(500))
            {
                return null;
            }

            PersonaThingComp comp = Personas.Find(p => p.UserId == userId);

            Lock.ExitReadLock();

            return comp;
        }

        [Obsolete("Name comparisons are deprecated, and implementors should use the viewer's id instead.")]
        public static PersonaThingComp GetPersonaByName(string username)
        {
            if (!Lock.TryEnterReadLock(500))
            {
                return null;
            }

            PersonaThingComp comp = Personas.Find(p => string.Equals(username, p.Viewer?.DisplayName) || string.Equals(username, p.Viewer?.Username));

            Lock.ExitReadLock();

            return comp;
        }

        internal static void RegisterPersona(PersonaThingComp comp)
        {
            if (!Lock.TryEnterWriteLock(500))
            {
                return;
            }

            Personas.Add(comp);
            Lock.ExitWriteLock();
        }

        internal static void UnregisterPersona(PersonaThingComp comp)
        {
            if (!Lock.TryEnterWriteLock(300))
            {
                return;
            }

            Personas.Remove(comp);
            Lock.ExitWriteLock();
        }

        internal static bool Contains(PersonaThingComp comp)
        {
            if (!Lock.TryEnterReadLock(500))
            {
                return false;
            }

            bool contains = Personas.Contains(comp);
            Lock.ExitReadLock();

            return contains;
        }

        internal static bool TryRegister(PersonaThingComp comp)
        {
            if (!Lock.TryEnterUpgradeableReadLock(500))
            {
                return false;
            }

            if (Personas.Contains(comp))
            {
                Lock.ExitUpgradeableReadLock();

                return false;
            }

            if (!Lock.TryEnterWriteLock(500))
            {
                Lock.ExitUpgradeableReadLock();

                return false;
            }

            Personas.Add(comp);
            Lock.ExitWriteLock();
            Lock.ExitUpgradeableReadLock();

            return true;
        }

        [ContractAnnotation("=> true,comp:notnull; => false,comp:null")]
        public static bool TryGetPersona(string userId, out PersonaThingComp comp)
        {
            comp = GetPersona(userId);

            return comp != null;
        }
    }
}
