// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimWorldUtility;
using RimWorldUtility.Models;
using RimWorldUtility.UI;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Verse;

namespace BetterColonistBar
{
    public static class BCBManager
    {
        private static readonly ConcurrentCacheTable<Pawn, PawnStatusCache> _statusCaches =
            new ConcurrentCacheTable<Pawn, PawnStatusCache>(new PawnStatusCacheMaker());

        private static readonly ConcurrentCacheTable<Pawn, BreakLevelCache> _breakLevelCaches =
            new ConcurrentCacheTable<Pawn, BreakLevelCache>(new BreakLevelCacheMaker());

        private static PawnOrderBleedingCache _bleedingCache;

        public static Dictionary<Pawn, MoodBarLocation> BarLocations = new Dictionary<Pawn, MoodBarLocation>(PawnComparer.Instance);

        public static Harmony Harmony { get; } = new Harmony(BetterColonistBarMod.Id);

        public static List<MethodInfo> PatchedMethod { get; private set; } = new List<MethodInfo>();

        public static Rect LastBarRect { get; set; } = Rect.zero;

        public static bool ModColonistBarDirty { get; set; } = true;

        public static List<ColonistBar.Entry> EntriesCache { get; set; } = new List<ColonistBar.Entry>();

        public static BreakLevelModel GetBreakLevelFor(Pawn pawn) => _breakLevelCaches[pawn];

        public static PawnStatusCache GetStatusFor(Pawn pawn) => _statusCaches[pawn];

        public static void Reset()
        {
            _statusCaches.Clear();
            _breakLevelCaches.Clear();
            _bleedingCache = new PawnOrderBleedingCache();
        }

        public static void Reorder()
        {
            if (BetterColonistBarMod.ModSettings.SortBleedingPawn)
            {
                _bleedingCache.Reorder();
            }
        }

        public static bool UpdateColonistBar()
        {
            Reorder();

            int result = 0;

            if (!BreakLevelCache.HasUpdateException && !PawnStatusCache.HasUpdateException)
            {
                try
                {
                    EntriesCache.AsParallel().ForAll(
                        t =>
                        {
                            bool dirty = t.pawn != null && (_breakLevelCaches[t.pawn].Dirty | _statusCaches[t.pawn].Dirty);
                            if (dirty)
                                Interlocked.Exchange(ref result, 1);
                        });
                }
                catch (Exception e)
                {
                    BetterColonistBarMod.ExceptionReport = new ExceptionReport(e);
                }
            }
            else
            {
                try
                {
                    foreach (ColonistBar.Entry entry in EntriesCache)
                    {
                        if (entry.pawn == null)
                            continue;

                        _breakLevelCaches[entry.pawn].ForceUpdate();
                        _statusCaches[entry.pawn].ForceUpdate();
                    }

                    result = 1;
                    BreakLevelCache.HasUpdateException = PawnStatusCache.HasUpdateException = false;
                }
                catch (Exception e)
                {
                    BetterColonistBarMod.ExceptionReport.Exceptions.Add(e);
                    BetterColonistBarMod.ExceptionReport.ExtraString = BuildReportString();
                    BetterColonistBarMod.HasException = true;
                }
            }

            return result > 0;

            string BuildReportString()
            {
                const string report = "Encounters errors when sequentially updates {0} after a failed parallel one.";
                if (BreakLevelCache.HasUpdateException)
                    return string.Format(CultureInfo.InvariantCulture, report, nameof(BreakLevelCache));
                else if (PawnStatusCache.HasUpdateException)
                    return string.Format(CultureInfo.InvariantCulture, report, nameof(PawnStatusCache));

                return string.Empty;
            }
        }
     
        public class PawnComparer : IEqualityComparer<Pawn>
        {
            public static PawnComparer Instance { get; } = new PawnComparer();

            #region Implementation of IEqualityComparer<in Pawn>

            public bool Equals(Pawn x, Pawn y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;

                return x.thingIDNumber == y.thingIDNumber;
            }

            public int GetHashCode(Pawn obj)
            {
                return obj.thingIDNumber;
            }

            #endregion
        }
    }
}
