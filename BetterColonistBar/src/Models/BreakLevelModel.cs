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
    public class BreakLevelModel
    {
        private readonly int _updateTick;

        public BreakLevelModel(Pawn pawn)
        {
            ValidateArg.NotNull(pawn, nameof(pawn));

            this.Pawn = pawn;
            _updateTick = pawn.thingIDNumber % BetterColonistBarMod.ModSettings.MoodUpdateInterval;
        }

        public Pawn Pawn { get; set; }

        public float Minor { get; set; }

        public float Major { get; set; }

        public float Extreme { get; set; }

        public float CurInstanLevel { get; set; }

        public MoodLevel MoodLevel { get; set; }

        public bool UpdateBarTexture =>
            Find.TickManager.TicksGame % BetterColonistBarMod.ModSettings.MoodUpdateInterval == _updateTick;

        public bool BuildingTexture { get; set; }

    }
}
