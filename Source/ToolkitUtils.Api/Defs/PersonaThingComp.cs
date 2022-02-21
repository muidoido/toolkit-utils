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

using CommonLib.Entities;
using CommonLib.Interfaces;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Registries;
using SirRandoo.ToolkitUtils.Services;
using Verse;

namespace SirRandoo.ToolkitUtils.Defs
{
    [UsedImplicitly]
    public class PersonaThingComp : ThingComp
    {
        private static readonly IRimLogger Logger = new RimLogger("tku.persona");

        private int _attempts;
        private Pawn _pawn;
        public IUserData UserData;
        public string UserId;

        [CanBeNull]
        public Pawn Pawn => _pawn ??= parent as Pawn;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (_attempts >= 5 || string.IsNullOrEmpty(UserId) || UserData != null)
            {
                return;
            }

            if (!UserRegistry.TryGet(UserId, out UserData))
            {
                _attempts++;

                return;
            }

            if (Pawn == null)
            {
                Logger.Warn("Persona comp applied to something other than a Pawn.");

                return;
            }

            PersonaService.TryRegister(this);

            if (string.Equals(parent.LabelShort, UserData.DisplayName))
            {
                return;
            }

            switch (Pawn.Name)
            {
                case NameTriple triple:
                    Pawn.Name = new NameTriple(triple.First, UserData.DisplayName, triple.Last);

                    return;
                case NameSingle _:
                    Pawn.Name = new NameSingle(UserData.DisplayName);

                    return;
            }
        }

        public override void PostDeSpawn(Map map)
        {
            PersonaService.UnregisterPersona(this);
        }

        [NotNull]
        public override string GetDescriptionPart()
        {
            const string part = "Connected to";

            if (UserData == null)
            {
                return part + " None " + (_attempts >= 5 ? "(auto-link timed out)" : "(attempting auto-link...)");
            }

            return part + " " + UserData.DisplayName;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref UserId, "userId");
        }
    }
}
