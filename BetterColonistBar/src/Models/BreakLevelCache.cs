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
    public class BreakLevelCache : CacheableTick<BreakLevelModel>
    {
        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        private bool _cacheUsed;

        private bool _lastMood;

        public BreakLevelCache(Pawn pawn)
            : base(new BreakLevelModel(pawn), () => Find.TickManager.TicksGame, _settings.MoodUpdateInterval, null, Find.TickManager.TicksGame)
        {
            ValidateArg.NotNull(pawn, nameof(pawn));

            this.Update = this.UpdateCache;
            _backingField = this.UpdateCache(pawn);
        }

        public bool Dirty
        {
            get
            {
                BreakLevelModel model = this;
                if (_cacheUsed)
                    return false;

                bool belowMood = model.MoodLevel <= _settings.shownMoodLevel;
                bool dirty = belowMood ^ _lastMood;
                _lastMood = belowMood;
                _cacheUsed = true;

                return dirty;
            }
        }

        private BreakLevelModel UpdateCache()
        {
            return this.UpdateCache(_backingField.Pawn);
        }

        private BreakLevelModel UpdateCache(Pawn pawn)
        {
            MentalBreaker breaker = pawn.mindState?.mentalBreaker;
            _cacheUsed = false;

            if (breaker is null)
            {
                _backingField.MoodLevel = MoodLevel.Undefined;
                _backingField.CurInstanLevel = _backingField.Minor = _backingField.Major = _backingField.Extreme = 0;
                return _backingField;
            }
            else
            {
                _backingField.Minor = breaker.BreakThresholdMinor;
                _backingField.Major = breaker.BreakThresholdMajor;
                _backingField.Extreme = breaker.BreakThresholdExtreme;
                _backingField.CurInstanLevel = _backingField.Pawn.needs?.mood?.CurInstantLevel ?? 0;
                _backingField.MoodLevel = this.GetMoodLevel(pawn);
                return _backingField;
            }
        }

        private MoodLevel GetMoodLevel(Pawn pawn)
        {
            float curMood = pawn.mindState.mentalBreaker.CurMood;

            if (curMood >= _backingField.Minor)
                return MoodLevel.Satisfied;
            else if (curMood >= _backingField.Major)
                return MoodLevel.Minor;
            else if (curMood >= _backingField.Extreme)
                return MoodLevel.Major;
            else
                return MoodLevel.Extreme;
        }
    }
}
