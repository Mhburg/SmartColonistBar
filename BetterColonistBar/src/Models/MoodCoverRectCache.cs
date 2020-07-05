// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorldUtility;
using Verse;

namespace BetterColonistBar.src.Models
{
    public class MoodCoverRectCache : CacheableTick<MoodCoverRectModel>
    {
        public MoodCoverRectCache(MoodCoverRectModel t)
            : base(t, () => Find.TickManager.TicksGame, BetterColonistBarMod.ModSettings.MoodUpdateInterval, null, Find.TickManager.TicksGame)
        {
        }
    }
}
