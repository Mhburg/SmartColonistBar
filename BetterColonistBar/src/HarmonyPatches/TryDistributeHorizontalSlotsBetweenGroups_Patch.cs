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
    public class TryDistributeHorizontalSlotsBetweenGroups_Patch
    {
        private static readonly MethodInfo _original =
            typeof(RimWorld.ColonistBarDrawLocsFinder)
                .GetMethod("TryDistributeHorizontalSlotsBetweenGroups", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _prefix =
            typeof(TryDistributeHorizontalSlotsBetweenGroups_Patch)
                .GetMethod(nameof(Prefix), BindingFlags.Public | BindingFlags.Static);

        private static readonly FieldInfo _horGroup =
            typeof(RimWorld.ColonistBarDrawLocsFinder)
                .GetField("horizontalSlotsPerGroup", BindingFlags.NonPublic | BindingFlags.Instance);

        static TryDistributeHorizontalSlotsBetweenGroups_Patch()
        {
            BCBManager.Harmony.Patch(_original, new HarmonyMethod(_prefix));
        }

        public static bool Prefix(ref bool __result, RimWorld.ColonistBarDrawLocsFinder __instance)
        {
            __result = true;
            ((List<int>)_horGroup.GetValue(__instance)).Clear();

            return Find.ColonistBar.Entries.Any();
        }
    }
}
