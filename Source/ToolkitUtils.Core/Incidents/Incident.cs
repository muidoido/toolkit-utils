﻿// ToolkitUtils.Core
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
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class Incident : IncidentHelper
    {
        private static readonly Dictionary<string, IIncidentData> Data;
        private IncidentParms _parms;
        private IncidentWorker _worker;

        static Incident()
        {
            Data = new Dictionary<string, IIncidentData>
            {
                { "TraderCaravanArrival", new TraderCaravanIncidentData() },
                { "OrbitalTraderArrival", new OrbitalTraderIncidentData() }
            };
        }

        public override bool IsPossible()
        {
            if (!Data.TryGetValue(storeIncident.defName, out IIncidentData data))
            {
                return false;
            }

            _worker = Activator.CreateInstance(data.WorkerClass) as IncidentWorker;
            Map map = Find.RandomPlayerHomeMap;

            if (map == null || _worker == null)
            {
                return false;
            }

            _parms = StorytellerUtility.DefaultParmsNow(data.ResolveCategory(_worker, storeIncident), map);
            _parms.forced = true;

            data.DoExtraSetup(_worker, _parms, storeIncident);

            return _worker.CanFireNow(_parms);
        }

        public override void TryExecute()
        {
            _worker.TryExecute(_parms);
        }
    }
}
