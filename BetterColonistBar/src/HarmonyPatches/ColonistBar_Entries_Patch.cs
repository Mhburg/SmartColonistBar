// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public static class ColonistBar_Entries_Patch
    {
        private const string _interceptMethodName = "CalculateColonistsInGroup";

        private static readonly MethodInfo _original = typeof(ColonistBar).GetProperty(nameof(ColonistBar.Entries))?.GetMethod;

        private static readonly MethodInfo _postfix =
            typeof(ColonistBar_Entries_Patch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.Static);

        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        static ColonistBar_Entries_Patch()
        {
            BCBManager.Harmony.Patch(_original, postfix: new HarmonyMethod(_postfix));
        }

        public static void Postfix(ref List<ColonistBar.Entry> __result)
        {
            BCBManager.ModColonistBarDirty = true;
            if (_settings.Expanded)
            {
                return;
            }

            StackTrace stackTrace = new StackTrace(2);
            string methodName = stackTrace.GetFrame(0).GetMethod().Name;
            if (methodName != _interceptMethodName)
                return;

            List<ColonistBar.Entry> copyEntries = new List<ColonistBar.Entry>(__result);

            foreach (ColonistBar.Entry entry in copyEntries)
            {
                if (entry.pawn.ShouldShowBar())
                    continue;

                int index = __result.FindIndex(e => e.pawn == entry.pawn);
                __result.RemoveAt(index);
            }

            if (__result.Count == 0)
                BCBManager.LastBarRect = Rect.zero;
        }
    }
}
