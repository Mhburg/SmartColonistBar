// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorldUtility;
using Verse;

namespace BetterColonistBar
{
    public class PawnOrderBleedingCache : CacheableTick<bool>
    {
        private bool _cacheUsed;

        private List<ColonistBar.Entry> _entries;

        public PawnOrderBleedingCache()
            : base(
                true
                , () => Find.TickManager.TicksGame
                , BetterColonistBarMod.ModSettings.BleedingRateUpdateInterval
                , null
                , Find.TickManager.TicksGame)
        {
            this.UpdateInternal();
            this.Update = this.UpdateInternal;
        }

        public void Reorder()
        {
            if (_cacheUsed | !this || !_entries.Any())
                return;

            Find.ColonistBar.UpdateEntries(_entries);
            _cacheUsed = true;
        }

        private bool UpdateInternal()
        {
            var entries = Find.ColonistBar.GetEntries();
            _entries = entries
                .OrderBy(t => t.@group)
                .ThenBy(t => HealthUtility.TicksUntilDeathDueToBloodLoss(t.pawn))
                .ToList();

            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].pawn == entries[i].pawn)
                    continue;

                _cacheUsed = false;
                return true;
            }

            _cacheUsed = false;
            return false;
        }
    }
}
