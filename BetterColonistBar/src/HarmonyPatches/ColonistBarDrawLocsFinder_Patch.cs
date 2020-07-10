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
using UnityEngine;
using Verse;

namespace BetterColonistBar.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public class ColonistBarDrawLocsFinder_Patch
    {
        private static readonly MethodInfo _calculateDrawLocs =
            typeof(RimWorld.ColonistBarDrawLocsFinder).GetMethod(nameof(RimWorld.ColonistBarDrawLocsFinder.CalculateDrawLocs));

        private static readonly MethodInfo _calculateDrawLocsPrefix =
            typeof(ColonistBarDrawLocsFinder_Patch)
                .GetMethod(nameof(CalculateDrawLocsPrefix), BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo _calculateColonistsInGroup =
            typeof(ColonistBarDrawLocsFinder)
                .GetMethod("CalculateColonistsInGroup", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly MethodInfo _calculateColonistInGroupPrefix =
            typeof(ColonistBarDrawLocsFinder_Patch)
                .GetMethod(nameof(CalculateColonistsInGroupPrefix), BindingFlags.Public | BindingFlags.Static);

        private static readonly FieldInfo _cachedEntries =
            typeof(ColonistBar).GetField("cachedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        static ColonistBarDrawLocsFinder_Patch()
        {
            BCBManager.Harmony.Patch(_calculateDrawLocs, new HarmonyMethod(_calculateDrawLocsPrefix));
            BCBManager.Harmony.Patch(_calculateColonistsInGroup, new HarmonyMethod(_calculateColonistInGroupPrefix));
        }

        public static bool CalculateDrawLocsPrefix()
        {
            BCBManager.EntriesCache = ((List<ColonistBar.Entry>)_cachedEntries.GetValue(Find.ColonistBar)).ToList();
            return true;
        }

        public static bool CalculateColonistsInGroupPrefix()
        {
            BCBManager.ModColonistBarDirty = true;
            if (_settings.Expanded)
            {
                return true;
            }

            List <ColonistBar.Entry> entries = Find.ColonistBar.GetEntries();
            List<ColonistBar.Entry> copyEntries = new List<ColonistBar.Entry>(entries);

            foreach (ColonistBar.Entry entry in copyEntries)
            {
                if (entry.pawn.ShouldShowBar())
                    continue;

                int index = entries.FindIndex(e => e.pawn == entry.pawn);
                entries.RemoveAt(index);
            }

            if (entries.Count == 0)
                BCBManager.LastBarRect = Rect.zero;

            return true;
        }
    }
}
