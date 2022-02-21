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
        private static readonly List<PersonaThingComp> Personas = new List<PersonaThingComp>();

        /// <summary>
        ///     Gets a persona for a given user.
        /// </summary>
        /// <param name="userId">The id of the user the persona is for.</param>
        /// <returns></returns>
        public static PersonaThingComp GetPersona(string userId)
        {
            return Personas.Find(p => p.UserId == userId);
        }

        [Obsolete]
        public static PersonaThingComp GetPersonaByName(string username)
        {
            return Personas.Find(p => string.Equals(username, p.UserData?.DisplayName) || string.Equals(username, p.UserData?.Username));
        }

        internal static void RegisterPersona(PersonaThingComp comp) => Personas.Add(comp);
        internal static void UnregisterPersona(PersonaThingComp comp) => Personas.Remove(comp);
        internal static bool Contains(PersonaThingComp comp) => Personas.Contains(comp);

        internal static bool TryRegister(PersonaThingComp comp)
        {
            if (Personas.Contains(comp))
            {
                return false;
            }

            Personas.Add(comp);

            return true;
        }
    }
}
