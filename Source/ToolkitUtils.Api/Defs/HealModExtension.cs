﻿// ToolkitUtils.Api
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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils.Defs
{
    /// <summary>
    ///     A <see cref="DefModExtension"/> for flagging
    ///     <see cref="HediffDef"/>s
    ///     as unhealable by the mod's heal functions.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature | ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class HealModExtension : DefModExtension
    {
        /// <summary>
        ///     Whether or not the mod should heal the attached hediff.
        /// </summary>
        public bool shouldHeal = true;
    }
}
