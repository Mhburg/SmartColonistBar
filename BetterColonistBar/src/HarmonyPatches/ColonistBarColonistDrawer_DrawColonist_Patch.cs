// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BetterColonistBar.UI;
using HarmonyLib;
using RimWorld;
using RimWorldUtility;
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

        private static readonly List<Task<(Texture2D, Pawn)>> _tasks = new List<Task<(Texture2D, Pawn)>>();

        private static readonly ConcurrentDictionary<Pawn, Texture2D> _barTexture2Ds =
            new ConcurrentDictionary<Pawn, Texture2D>(BCBManager.PawnComparer.Instance);

        private static readonly Dictionary<Pawn, Texture2D> _texture2Ds = new Dictionary<Pawn, Texture2D>(BCBManager.PawnComparer.Instance);

        static ColonistBarColonistDrawer_DrawColonist_Patch()
        {
            BCBManager.Harmony.Patch(_original, transpiler: new HarmonyMethod(_transpiler));
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

        public static Texture2D BuildTexture(Texture2D newTexture, float currMoodHeight, Color moodColor, int moodLevel, float[] thresholds)
        {
            ValidateArg.NotNull(thresholds, nameof(thresholds));
            ValidateArg.NotNull(newTexture, nameof(newTexture));

            currMoodHeight = currMoodHeight < 2 ? 2 : currMoodHeight;
            int moodHeight = Mathf.RoundToInt(currMoodHeight);

            for (int x = 0; x < newTexture.width; x++)
            {
                for (int y = 0; y < moodHeight; y++)
                {
                    newTexture.SetPixel(x, y, moodColor);
                }

                for (int y = moodHeight; y < newTexture.height; y++)
                {
                    newTexture.SetPixel(x, y, _settings.BgColor);
                }
            }

            foreach (float pct in thresholds)
            {
                int start = Mathf.RoundToInt(newTexture.height * pct);
                for (int x = 0; x < newTexture.width / 2; x++)
                {
                    for (int y = start; y < _settings.ThresholdMarkerThickness + start; y++)
                    {
                        newTexture.SetPixel(x, y, _settings.ThresholdMarker);
                    }
                }
            }

            for (int x = 0; x < newTexture.width; x++)
            {
                for (int y = moodLevel; y < moodLevel + _settings.CurrMoodLevelThickness; y++)
                    newTexture.SetPixel(x, y, _settings.CurrMoodLevel);
            }

            return newTexture;
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

            GUI.color = color;
            DrawMoodBarFast(rect, pawn, breakLevelModel);
            GUI.color = Color.white;

            //DrawMoodBar(color, rect, pawn, breakLevelModel);
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

            DrawBarThresholdTest(moodBarRect, thresholds, pawn);

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


        private static void DrawMoodBarFast(Rect portraitRect, Pawn pawn, BreakLevelModel breakLevelModel)
        {
            float[] thresholds = { breakLevelModel.Minor, breakLevelModel.Major, breakLevelModel.Extreme };
            Rect moodBaRect = GetMoodBarRect(portraitRect, pawn);

            if (!_barTexture2Ds.TryGetValue(pawn, out Texture2D texture))
            {
                Texture2D newTexture = new Texture2D(Mathf.RoundToInt(moodBaRect.width), Mathf.RoundToInt(moodBaRect.height));
                texture = _barTexture2Ds[pawn] = BuildTexture(newTexture, moodBaRect, pawn, breakLevelModel, thresholds).texture;
                texture.Apply();
            }

            foreach (Task<(Texture2D texture, Pawn pawn)> t in _tasks.ToList())
            {
                if (t.IsCompleted)
                {
                    (Texture2D texture, Pawn pawn) result = t.Result;
                    Texture2D newTexture2D = result.texture;
                    newTexture2D.Apply();
                    _barTexture2Ds[result.pawn] = newTexture2D;
                    BCBManager.GetBreakLevelFor(result.pawn).BuildingTexture = false;

                    if (pawn == result.pawn)
                        texture = newTexture2D;

                    _tasks.Remove(t);
                }
            }

            if (_settings.UISettingsChanged || (breakLevelModel.UpdateBarTexture && !breakLevelModel.BuildingTexture))
            {
                breakLevelModel.BuildingTexture = true;
                Texture2D newTexture = new Texture2D(Mathf.RoundToInt(moodBaRect.width), Mathf.RoundToInt(moodBaRect.height));
                _tasks.Add(Task.Run(() => BuildTexture(newTexture, moodBaRect, pawn, breakLevelModel, thresholds)));
            }

            GUI.DrawTexture(moodBaRect, texture);
        }

        private static (Texture2D texture, Pawn pawn) BuildTexture(Texture2D newTexture, Rect moodBaRect, Pawn pawn, BreakLevelModel breakLevelModel, float[] thresholds)
        {
            int moodHeight = Mathf.RoundToInt(moodBaRect.height * pawn.mindState.mentalBreaker.CurMood);
            int curMoodY = Mathf.RoundToInt(BCBManager.GetBreakLevelFor(pawn).CurInstanLevel * newTexture.height);
            Color moodColor = breakLevelModel.MoodLevel.GetColor();

            BuildTexture(newTexture, moodHeight, moodColor, curMoodY, thresholds);

            return (newTexture, pawn);
        }

    }
}
