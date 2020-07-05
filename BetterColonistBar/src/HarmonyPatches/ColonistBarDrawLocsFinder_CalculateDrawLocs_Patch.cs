// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterColonistBar.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public class ColonistBarDrawLocsFinder_CalculateDrawLocs_Patch
    {
        private static readonly MethodInfo _original =
            typeof(ColonistBarDrawLocsFinder).GetMethod(nameof(ColonistBarDrawLocsFinder.CalculateDrawLocs));

        private static readonly MethodInfo _prefix =
            typeof(ColonistBarDrawLocsFinder_CalculateDrawLocs_Patch)
                .GetMethod(nameof(Prefix), BindingFlags.Public | BindingFlags.Static);

        private static readonly FieldInfo _cachedEntries =
            typeof(ColonistBar).GetField("cachedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

        static ColonistBarDrawLocsFinder_CalculateDrawLocs_Patch()
        {
            BCBManager.Harmony.Patch(_original, new HarmonyMethod(_prefix));
        }

        public static bool Prefix()
        {
            BCBManager.EntriesCache = ((List<ColonistBar.Entry>)_cachedEntries.GetValue(Find.ColonistBar)).ToList();
            return true;
        }
    }
}
