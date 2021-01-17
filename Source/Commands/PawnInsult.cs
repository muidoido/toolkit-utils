﻿using System;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;
using Verse.AI;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnInsult : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Viewer data = Viewers.GetViewer(twitchMessage.Username);

            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            string query = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrFallback("");
            Pawn target = null;

            if (!query.NullOrEmpty())
            {
                if (query.StartsWith("@"))
                {
                    query = query.Substring(1);
                }

                Viewer viewer = Viewers.All.FirstOrDefault(v => v.username.EqualsIgnoreCase(query));

                if (viewer == null)
                {
                    return;
                }

                target = GetOrFindPawn(viewer.username);

                if (target == null)
                {
                    twitchMessage.Reply("TKUtils.PawnNotFound".Localize(query));
                    return;
                }
            }

            target ??= Find.ColonistBar.Entries.RandomElement().pawn;
            Job job = JobMaker.MakeJob(JobDefOf.Insult, target);

            if (job.CanBeginNow(pawn))
            {
                data.SetViewerKarma(Math.Max(data.karma - 15, ToolkitSettings.KarmaMinimum));

                pawn.jobs.StartJob(job, JobCondition.InterruptForced);
            }
        }
    }
}
