// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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

        private static void DrawAddOn(Color color, Rect rect, Pawn pawn)
        {
            if (BCBManager.LastBarRectDirty)
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
            List<float> thresholds = new List<float>() { breakLevelModel.Minor, breakLevelModel.Major, breakLevelModel.Extreme };

            GUI.color = color;

            Rect moodBarRect = GetMoodBarRect(portraitRect, pawn);

            float moodHeight = moodBarRect.height * pawn.mindState.mentalBreaker.CurMood;
            moodHeight = moodHeight < 2f ? 2f : moodHeight;
            Rect coveredRect = new Rect(moodBarRect.x, moodBarRect.yMax, moodBarRect.width, -moodHeight);

            GUI.DrawTexture(moodBarRect, BaseContent.GreyTex);
            GUI.DrawTexture(coveredRect, breakLevelModel.MoodLevel.GetTexture());
            GUI.color = _thresholdColor;
            foreach (float t in thresholds)
            {
                DrawBarThreshold(moodBarRect, t);
            }
            GUI.color = Color.white;

            Rect markerRect = new Rect(moodBarRect.x, Mathf.Lerp(moodBarRect.yMax, moodBarRect.y, BCBManager.GetBreakLevelFor(pawn).CurInstanLevel), moodBarRect.width, 1f);

            GUI.DrawTexture(markerRect, BaseContent.WhiteTex);
        }

        private static Rect GetMoodBarRect(Rect portraitRect, Pawn pawn)
        {
            portraitRect = portraitRect.RightPart(_settings.MoodBarWidth);
            portraitRect.x += portraitRect.width;
            return portraitRect;
        }

        private static void DrawBarThreshold(Rect barRect, float threshPct)
        {
            float y = Mathf.Lerp(barRect.yMax, barRect.y, threshPct);
            Rect rect = new Rect(barRect.x, y, barRect.width / 2, GenUI.GapTiny / 2);

            Texture2D image = BaseContent.BlackTex;

            GUI.DrawTexture(rect, image);
        }
    }
}
