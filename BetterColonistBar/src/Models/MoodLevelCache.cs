// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorldUtility;
using Verse;
using Verse.AI;

namespace BetterColonistBar
{
    public class MoodLevelCache : CacheableTick<MoodLevel>
    {
        private bool _lastCheck;

        public MoodLevelCache(int updateInterval, Pawn pawn)
            : base(GetMoodLevel(pawn), () => Find.TickManager.TicksGame, updateInterval, () => GetMoodLevel(pawn))
        {
        }

        private static MoodLevel GetMoodLevel(Pawn pawn)
        {
            ValidateArg.NotNull(pawn, nameof(pawn));

            BreakLevelModel breakLevel = BCBManager.GetBreakLevelFor(pawn);

            MentalBreaker breaker = pawn.mindState?.mentalBreaker;
            if (breaker is null)
                return MoodLevel.Undefined;

            float curMood = breaker.CurMood;

            if (curMood >= breaker.BreakThresholdMinor)
                return MoodLevel.Satisfied;
            else if (curMood >= breaker.BreakThresholdMajor)
                return MoodLevel.Minor;
            else if (curMood >= breaker.BreakThresholdExtreme)
                return MoodLevel.Major;
            else
                return MoodLevel.Extreme;
        }
    }
}
