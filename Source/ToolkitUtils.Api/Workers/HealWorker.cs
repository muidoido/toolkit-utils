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

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Defs;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class HealWorker
    {
        public static float HandCoverageAbsWithChildren { get; } = ThingDefOf.Human.race.body.GetPartsWithDef(BodyPartDefOf.Hand).First().coverageAbsWithChildren;

        private static bool CanEverKill([NotNull] Hediff hediff)
        {
            if (!(hediff.def.stages?.Count > 0))
            {
                return hediff.def.lethalSeverity >= 0f;
            }

            foreach (HediffStage stage in hediff.def.stages)
            {
                if (stage.lifeThreatening)
                {
                    return true;
                }
            }

            return false;
        }

        [CanBeNull]
        public static Hediff_Addiction FindAddiction([NotNull] Pawn pawn)
        {
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff is Hediff_Addiction { Visible: true } addiction && addiction.def.everCurableByItem && hediff.def.GetModExtension<HealModExtension>()?.shouldHeal != false)
                {
                    return addiction;
                }
            }

            return null;
        }

        [CanBeNull]
        public static BodyPartRecord FindBiggestMissingBodyPart([NotNull] Pawn pawn, float minCoverage = 0f)
        {
            BodyPartRecord record = null;

            foreach (Hediff_MissingPart missing in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                if (missing.def.GetModExtension<HealModExtension>()?.shouldHeal == true)
                {
                    continue;
                }

                if (missing.Part.coverageAbsWithChildren < minCoverage || pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(missing.Part))
                {
                    continue;
                }

                if (missing.Part.coverageAbsWithChildren < record?.coverageAbsWithChildren)
                {
                    continue;
                }

                record = missing.Part;
            }

            return record;
        }
    }
}
