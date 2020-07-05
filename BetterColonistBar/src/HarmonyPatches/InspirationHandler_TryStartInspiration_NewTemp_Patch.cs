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
using RimWorldUtility;
using Verse;

namespace BetterColonistBar.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public class InspirationHandler_TryStartInspiration_NewTemp_Patch
    {
        private static readonly MethodInfo _original =
            typeof(InspirationHandler).GetMethod(nameof(InspirationHandler.TryStartInspiration_NewTemp));

        private static readonly MethodInfo _postfix =
            typeof(InspirationHandler_TryStartInspiration_NewTemp_Patch)
                .GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.Static);

        static InspirationHandler_TryStartInspiration_NewTemp_Patch()
        {
            BCBManager.Harmony.Patch(_original, postfix: new HarmonyMethod(_postfix));
        }

        public static void Postfix(InspirationHandler __instance)
        {
            if (!__instance.pawn.IsFreeColonist)
                return;

            BCBManager.SetColonistBarDirty();
        }
    }
}
