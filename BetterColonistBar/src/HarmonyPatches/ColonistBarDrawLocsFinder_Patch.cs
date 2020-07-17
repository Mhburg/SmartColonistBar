// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        private static readonly MethodInfo _calculateDrawLocs =
            typeof(ColonistBarDrawLocsFinder).GetMethod(nameof(ColonistBarDrawLocsFinder.CalculateDrawLocs));

        private static readonly MethodInfo _calcualteDrawLocs2 =
            typeof(ColonistBarDrawLocsFinder)
                .GetMethod(nameof(ColonistBarDrawLocsFinder.CalculateDrawLocs), BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly MethodInfo _calculateDrawLocsPrefix =
            typeof(ColonistBarDrawLocsFinder_Patch)
                .GetMethod(nameof(CalculateDrawLocsPrefix), BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo _calculateDrawLocsTranspiler =
            typeof(ColonistBarDrawLocsFinder_Patch)
                .GetMethod(nameof(CalculateDrawLocsTranspiler), BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo _calculateColonistsInGroup =
            typeof(ColonistBarDrawLocsFinder)
                .GetMethod("CalculateColonistsInGroup", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly MethodInfo _calculateColonistInGroupPrefix =
            typeof(ColonistBarDrawLocsFinder_Patch)
                .GetMethod(nameof(CalculateColonistsInGroupPrefix), BindingFlags.Public | BindingFlags.Static);

        private static readonly FieldInfo _cachedEntries =
            typeof(ColonistBar).GetField("cachedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo _settingField = typeof(ColonistBarDrawLocsFinder_Patch)
            .GetField(nameof(_settings), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly FieldInfo _yOffset = typeof(BetterColonistBarSettings).GetField(nameof(BetterColonistBarSettings.YOffset));

        private static readonly MethodInfo _calculateGroupsCount = typeof(ColonistBarDrawLocsFinder)
            .GetMethod("CalculateGroupsCount", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly MethodInfo _calculateGroupsCountPostfix = typeof(ColonistBarDrawLocsFinder_Patch)
            .GetMethod(nameof(CalculateGroupsCountPostfix), BindingFlags.Public | BindingFlags.Static);

        static ColonistBarDrawLocsFinder_Patch()
        {
            BCBManager.Harmony.Patch(_calculateDrawLocs, new HarmonyMethod(_calculateDrawLocsPrefix));
            BCBManager.Harmony.Patch(_calcualteDrawLocs2, transpiler: new HarmonyMethod(_calculateDrawLocsTranspiler));
            BCBManager.Harmony.Patch(_calculateColonistsInGroup, new HarmonyMethod(_calculateColonistInGroupPrefix));
            BCBManager.Harmony.Patch(_calculateGroupsCount, postfix: new HarmonyMethod(_calculateGroupsCountPostfix));
        }

        public static bool CalculateDrawLocsPrefix()
        {
            BCBManager.EntriesCache = ((List<ColonistBar.Entry>)_cachedEntries.GetValue(Find.ColonistBar)).ToList();
            return true;
        }

        public static IEnumerable<CodeInstruction> CalculateDrawLocsTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionsList = instructions.ToList();
            foreach (CodeInstruction instruction in instructionsList)
            {
                if (!instruction.OperandIs(21f))
                {
                    yield return instruction;
                    continue;
                }

                yield return new CodeInstruction(OpCodes.Ldsfld, _settingField);
                yield return new CodeInstruction(OpCodes.Ldfld, _yOffset);
            }
        }

        public static bool CalculateColonistsInGroupPrefix()
        {
            try
            {
                BCBManager.ModColonistBarDirty = true;
                if (BCBComponent.Expanded)
                {
                    return true;
                }

                List<ColonistBar.Entry> entries = Find.ColonistBar.GetEntries();
                List<ColonistBar.Entry> copyEntries = new List<ColonistBar.Entry>(entries);

                foreach (ColonistBar.Entry entry in copyEntries)
                {
                    if (entry.pawn.ShouldShowBar())
                        continue;

                    int index = entries.FindIndex(e => e.pawn == entry.pawn);
                    if (index != -1)
                        entries.RemoveAt(index);
                }

                if (entries.Count == 0)
                    BCBManager.LastBarRect = Rect.zero;
            }
            catch (Exception e)
            {
                Log.Message(e.ToString());
            }

            return true;
        }

        public static void CalculateGroupsCountPostfix(ref int __result)
        {
            List<ColonistBar.Entry> entries = Find.ColonistBar.Entries;
            if (entries.TryMaxBy(e => e.@group, out ColonistBar.Entry value))
            {
                if (value.@group >= __result)
                    __result = value.@group + 1;
            }
        }
    }
}
