// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorldUtility;
using Verse;

namespace BetterColonistBar
{
    public class PawnStatusCache : CacheableTick<int>
    {
        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        private readonly Pawn _pawn;

        private bool _cacheUsed;

        private int _lastCache;

        private int _status;

        public PawnStatusCache(Pawn pawn)
            : base(0, () => Find.TickManager.TicksGame, _settings.StatusUpdateInterval, null, Find.TickManager.TicksGame)
        {
            _pawn = pawn;
            this.Update = CheckPawnStatus;
            _backingField = _lastCache = this.CheckPawnStatus();
        }

        public bool HasTendingHediff { get; private set; } = false;

        public bool HasInspiration { get; private set; } = false;

        public bool Dirty
        {
            get
            {
                int status = this;
                if (_cacheUsed)
                    return false;

                bool dirty = _lastCache != status;
                _cacheUsed = true;
                _lastCache = status;
                return dirty;
            }
        }

        private int CheckPawnStatus()
        {
            _cacheUsed = false;
            this.CheckInspiration();
            this.CheckHealth();
            this.CheckDraft();
            this.CheckIdle();
            this.CheckMentalState();
            this.CheckFleeing();
            this.CheckBurning();
            return _status;
        }

        private void CheckHealth()
        {
            this.HasTendingHediff = _pawn.health?.HasHediffsNeedingTendByPlayer() ?? false;
            _status = SetBit(_status, 0, this.HasTendingHediff);
        }

        private void CheckInspiration()
        {
            this.HasInspiration = _pawn.Inspired;
            _status = SetBit(_status, 1, this.HasInspiration);
        }

        private void CheckDraft()
        {
            _status = SetBit(_status, 2, _pawn.Drafted);
        }

        private void CheckIdle()
        {
            _status = SetBit(_status, 3, (_pawn.mindState?.IsIdle ?? false) && GenDate.DaysPassed > 1);
        }

        private void CheckMentalState()
        {
            _status = SetBit(_status, 4, _pawn.InMentalState);
        }

        private void CheckFleeing()
        {
            _status = SetBit(_status, 5, _pawn.CurJob?.def == JobDefOf.FleeAndCower);
        }

        private void CheckBurning()
        {
            _status = SetBit(_status, 6, _pawn.IsBurning());
        }

        private static int SetBit(int value, int position, bool bitValue)
        {
            int bitInt = 1 << position;
            if (bitValue)
                value |= bitInt;
            else
                value &= ~bitInt;

            return value;
        }
    }
}
