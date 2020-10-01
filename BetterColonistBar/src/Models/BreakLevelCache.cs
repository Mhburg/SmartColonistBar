// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RimWorld;
using RimWorldUtility;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BetterColonistBar
{
    public class BreakLevelCache : CacheableTick<BreakLevelModel>
    {
        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        private static readonly ReaderWriterLockSlim _curMoodLock = new ReaderWriterLockSlim();

        private static readonly ThreadLocal<List<Thought>> _moodOffsetThoughts = new ThreadLocal<List<Thought>>(() => new List<Thought>());

        private static readonly ThreadLocal<List<Thought>> _thoughts = new ThreadLocal<List<Thought>>(() => new List<Thought>());

        private readonly Pawn _pawn;

        private bool _cacheUsed;

        private bool _lastMood;

        public BreakLevelCache(Pawn pawn)
            : base(new BreakLevelModel(pawn), () => Find.TickManager.TicksGame, _settings.MoodUpdateInterval, null, 0)
        {
            ValidateArg.NotNull(pawn, nameof(pawn));

            _pawn = pawn;
            this.Update = this.UpdateCache;
            _backingField = this.UpdateCache(pawn);
        }

        #region Overrides of CacheableTick<BreakLevelModel>

        public static bool HasUpdateException { get; set; }

        public override bool Validate()
        {
            return !(_pawn is null || _pawn.Destroyed);
        }

        #endregion

        public bool Dirty
        {
            get
            {
                BreakLevelModel model = this;
                if (_cacheUsed)
                    return false;

                bool belowMood = model.MoodLevel <= _settings.ShownMoodLevel;
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

            try
            {
                if (breaker is null)
                {
                    _backingField.MoodLevel = MoodLevel.Undefined;
                    _backingField.CurInstanLevel =
                        _backingField.Minor = _backingField.Major = _backingField.Extreme = 0;
                    return _backingField;
                }
                else
                {
                    _backingField.Minor = breaker.BreakThresholdMinor;
                    _backingField.Major = breaker.BreakThresholdMajor;
                    _backingField.Extreme = breaker.BreakThresholdExtreme;
                    _backingField.CurInstanLevel = CurInstantLevelThreadSafe();
                    //_curMoodLock.EnterWriteLock();
                    //_backingField.CurInstanLevel = _backingField.Pawn.needs?.mood?.CurInstantLevel ?? 0;
                    //_curMoodLock.ExitWriteLock();
                    _backingField.MoodLevel = this.GetMoodLevel(pawn);
                    _backingField.UpdateBarTexture = true;
                    return _backingField;
                }
            }
            catch
            {
                HasUpdateException = true;
                if (_curMoodLock.IsWriteLockHeld)
                    _curMoodLock.ExitWriteLock();

                throw;
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

        /// <summary>
        /// A rip-off from vanilla.
        /// </summary>
        /// <returns></returns>
        private float CurInstantLevelThreadSafe()
        {
            float level = TotalMoodOffsetThreadSafe();
            if (_backingField.Pawn.IsColonist || _backingField.Pawn.IsPrisonerOfColony)
            {
                level += Find.Storyteller.difficultyValues.colonistMoodOffset;
            }

            return Mathf.Clamp01(_backingField.Pawn.needs.mood.def.baseLevel + level / 100f);
        }

        /// <summary>
        /// Thread-safe version for vanilla.
        /// </summary>
        /// <returns></returns>
        private float TotalMoodOffsetThreadSafe()
        {
            List<Thought> curThoughts = _moodOffsetThoughts.Value;
            ThoughtHandler handler = _backingField.Pawn.needs?.mood?.thoughts;
            if (handler is null)
                return 0f;

            handler.GetDistinctMoodThoughtGroups(curThoughts);
            return curThoughts.Sum(t => MoodOffsetOfGroup(t, handler));
        }

        /// <summary>
        /// Thread-safe version for vanilla.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        private float MoodOffsetOfGroup(Thought group, ThoughtHandler handler)
        {
            List<Thought> tmpThoughts = _thoughts.Value;
			handler.GetMoodThoughts(group, tmpThoughts);
			if (!tmpThoughts.Any())
				return 0f;

			float totalMoodOffset = 0f;
			float effectMultiplier = 1f;
			float totalEffectMultiplier = 0f;

			foreach (Thought thought in tmpThoughts)
            {
                totalMoodOffset += thought.MoodOffset();
                totalEffectMultiplier += effectMultiplier;
                effectMultiplier *= thought.def.stackedEffectMultiplier;
            }

			float num4 = totalMoodOffset / (float)tmpThoughts.Count;
			tmpThoughts.Clear();
			return num4 * totalEffectMultiplier;
		}
    }
}
