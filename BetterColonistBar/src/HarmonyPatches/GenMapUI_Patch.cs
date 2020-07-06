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
    public static class GenMapUI_Patch
    {
        private static readonly MethodInfo _pawnLabelOriginal =
            typeof(GenMapUI).GetMethod("GetPawnLabel", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo _pawnLabelPrefix =
            typeof(GenMapUI_Patch).GetMethod(nameof(PawnLabelPrefix), BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo _pawnLabelPostfix =
            typeof(GenMapUI_Patch).GetMethod(nameof(PawnLabelPostfix), BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo _pawnNameWidthOriginal =
            typeof(GenMapUI).GetMethod("GetPawnLabelNameWidth", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo _pawnNameWidthPrefix =
            typeof(GenMapUI_Patch).GetMethod(nameof(GetPawnLabelNameWidthPrefix), BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo _pawnNameWidthPostfix =
            typeof(GenMapUI_Patch).GetMethod(nameof(GetPawnLabelNameWidthPostfix), BindingFlags.Public | BindingFlags.Static);

        private static Pawn _pawn;

        private static string _label;

        private static float _width;

        static GenMapUI_Patch()
        {
            BCBManager.Harmony.Patch(_pawnLabelOriginal, new HarmonyMethod(_pawnLabelPrefix), new HarmonyMethod(_pawnLabelPostfix));
            BCBManager.Harmony.Patch(_pawnNameWidthOriginal, new HarmonyMethod(_pawnNameWidthPrefix), new HarmonyMethod(_pawnNameWidthPostfix));
        }

        public static bool PawnLabelPrefix(Pawn pawn, ref string __result)
        {
            if (pawn != _pawn)
                return true;

            __result = _label;
            return false;
        }

        public static void PawnLabelPostfix(Pawn pawn, string __result)
        {
            _pawn = pawn;
            _label = __result;
        }

        public static bool GetPawnLabelNameWidthPrefix(Pawn pawn, ref float __result)
        {
            if (pawn != _pawn)
                return true;

            __result = _width;
            return false;
        }

        public static void GetPawnLabelNameWidthPostfix(Pawn pawn, float __result)
        {
            _pawn = pawn;
            _width = __result;
        }
    }
}
