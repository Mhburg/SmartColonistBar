// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BetterColonistBar.UI;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterColonistBar.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class ColonistBarColonistDrawer_DrawColonist_Patch
    {
        private static readonly MethodInfo _original = typeof(ColonistBarColonistDrawer).GetMethod(nameof(ColonistBarColonistDrawer.DrawColonist));

        private static readonly MethodInfo _transpiler = typeof(ColonistBarColonistDrawer_DrawColonist_Patch).GetMethod(nameof(Transpiler), BindingFlags.Public | BindingFlags.Static);

        private static readonly FieldInfo _needs = typeof(Pawn).GetField(nameof(Pawn.needs));

        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        private static readonly Color _thresholdColor = new Color(1f, 1f, 1f, 0.9f);

        private static readonly int _iteration = 20_000;
        private static Stopwatch _stopwatch = new Stopwatch();
        private static int _counter = 0;
        private static double _time = 0;
        private static readonly Dictionary<Pawn, Texture2D> _texture2Ds = new Dictionary<Pawn, Texture2D>(BCBManager.PawnComparer.Instance);

        static ColonistBarColonistDrawer_DrawColonist_Patch()
        {
            BCBManager.Harmony.Patch(_original, transpiler: new HarmonyMethod(_transpiler));
            _stopwatch.Start();
            _stopwatch.Stop();
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo drawMood = typeof(ColonistBarColonistDrawer_DrawColonist_Patch)
                .GetMethod(nameof(DrawAddOn), BindingFlags.Static | BindingFlags.NonPublic);

            CodeInstruction instrc1 = new CodeInstruction(OpCodes.Ldloc_1);
            CodeInstruction instrc2 = new CodeInstruction(OpCodes.Ldarg_1);
            CodeInstruction instrc3 = new CodeInstruction(OpCodes.Ldarg_2);
            CodeInstruction instrc4 = new CodeInstruction(OpCodes.Call, drawMood);

            List<CodeInstruction> list = instructions.ToList();
            list.InsertRange(list.Count - 2, new List<CodeInstruction>() { instrc1, instrc2, instrc3, instrc4 });

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].OperandIs(_needs))
                {
                    list.RemoveRange(i - 1, 31);
                    break;
                }
            }

            return list;
        }

        private static void DrawAddOn(Color color, Rect rect, Pawn pawn)
        {
            if (BCBManager.ModColonistBarDirty)
            {
                BCBManager.LastBarRect = Rect.zero;
                if (rect.y > BCBManager.LastBarRect.y)
                    BCBManager.LastBarRect = rect;
                else if (Math.Round(rect.y) == Math.Round(BCBManager.LastBarRect.y) && rect.x > BCBManager.LastBarRect.x)
                    BCBManager.LastBarRect = rect;
            }

            if (pawn.mindState is null)
                return;

            BreakLevelModel breakLevelModel = BCBManager.GetBreakLevelFor(pawn);
            if (breakLevelModel.MoodLevel == MoodLevel.Undefined)
                return;

            DrawMoodBar(color, rect, pawn, breakLevelModel);
        }

        private static void DrawMoodBar(Color color, Rect portraitRect, Pawn pawn, BreakLevelModel breakLevelModel)
        {
            float[] thresholds = { breakLevelModel.Minor, breakLevelModel.Major, breakLevelModel.Extreme };

            GUI.color = color;

            Rect moodBarRect = GetMoodBarRect(portraitRect, pawn);

            float moodHeight = moodBarRect.height * pawn.mindState.mentalBreaker.CurMood;
            moodHeight = moodHeight < 2f ? 2f : moodHeight;
            Rect coveredRect = new Rect(moodBarRect.x, moodBarRect.yMax, moodBarRect.width, -moodHeight);

            GUI.DrawTexture(moodBarRect, BaseContent.GreyTex);
            GUI.DrawTexture(coveredRect, breakLevelModel.MoodLevel.GetTexture());
            GUI.color = _thresholdColor;

            //_stopwatch.Restart();

            //foreach (float t in thresholds)
            //{
            //    DrawBarThreshold(moodBarRect, t);
            //}

            DrawBarThresholdTest(moodBarRect, thresholds, pawn);
            //_stopwatch.Stop();
            //_time += _stopwatch.Elapsed.TotalMilliseconds;
            //_counter++;

            //if (_counter == _iteration)
            //{
            //    Log.Message($"Time: {_time / 1000 : 0000.0000}ms");
            //    _counter = 0;
            //    _time = 0;
            //}

            GUI.color = Color.white;

            Rect markerRect = new Rect(moodBarRect.x, Mathf.Lerp(moodBarRect.yMax, moodBarRect.y, BCBManager.GetBreakLevelFor(pawn).CurInstanLevel), moodBarRect.width, 1f);

            GUI.DrawTexture(markerRect, BaseContent.WhiteTex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Rect GetMoodBarRect(Rect portraitRect, Pawn pawn)
        {
            if (!BCBManager.ModColonistBarDirty && BCBManager.BarLocations.TryGetValue(pawn, out MoodBarLocation barRect))
                return barRect.BarRect;

            portraitRect = portraitRect.RightPart(_settings.MoodBarWidth);
            portraitRect.x += portraitRect.width;
            BCBManager.BarLocations[pawn] = new MoodBarLocation(portraitRect, pawn);
            return portraitRect;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawBarThreshold(Rect barRect, float threshPct)
        {
            float y = Mathf.Lerp(barRect.yMax, barRect.y, threshPct);
            Rect rect = new Rect(barRect.x, y, barRect.width / 2, GenUI.GapTiny / 2);

            GUI.DrawTexture(rect, BaseContent.BlackTex);
        }

        private static void DrawBarThresholdTest(Rect barRect, float[] pcts, Pawn pawn)
        {
            if (!_texture2Ds.TryGetValue(pawn, out Texture2D value))
            {
                Texture2D barTexture2D = new Texture2D(Mathf.RoundToInt(barRect.width), Mathf.RoundToInt(barRect.height));
                const float height = GenUI.GapTiny / 2;
                foreach (float pct in pcts)
                {
                    int start = Mathf.RoundToInt(barTexture2D.height * pct);
                    for (int x = 0; x < barTexture2D.width / 2; x++)
                    {
                        for (int y = start; y < height + start; y++)
                        {
                            barTexture2D.SetPixel(x, y, Color.black);
                        }
                    }
                }
                barTexture2D.Apply();
                _texture2Ds[pawn] = barTexture2D;
            }

            GUI.DrawTexture(barRect, value);
        }
    }
}
