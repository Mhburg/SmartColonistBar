﻿// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimWorldUtility;
using UnityEngine;
using Verse;

namespace BetterColonistBar
{
    public static class BCBManager
    {
        private static readonly CacheTable<Pawn, PawnStatusCache> _statusCaches =
            new CacheTable<Pawn, PawnStatusCache>(new PawnStatusCacheMaker());

        private static readonly CacheTable<Pawn, BreakLevelCache> _breakLevelCaches =
            new CacheTable<Pawn, BreakLevelCache>(new BreakLevelCacheMaker());

        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        public static Harmony Harmony { get; } = new Harmony(BetterColonistBarMod.Id);

        public static Rect LastBarRect { get; set; } = Rect.zero;

        public static bool LastBarRectDirty { get; set; } = true;

        public static List<ColonistBar.Entry> EntriesCache { get; set; } = new List<ColonistBar.Entry>();

        public static BreakLevelModel GetBreakLevelFor(Pawn pawn) => _breakLevelCaches[pawn];

        public static PawnStatusCache GetStatusFor(Pawn pawn) => _statusCaches[pawn];

        public static bool ModColonistBarDirty { get; set; }

        public static void Reset()
        {
            _statusCaches.Clear();
            _breakLevelCaches.Clear();
        }

        public static bool UpdateColonistBar()
        {
            bool dirty = EntriesCache.AsParallel().Aggregate(
                false
                , (current, entry) =>
                    current | (_breakLevelCaches[entry.pawn].Dirty | _statusCaches[entry.pawn].Dirty));

            return dirty;
        }
    }
}
