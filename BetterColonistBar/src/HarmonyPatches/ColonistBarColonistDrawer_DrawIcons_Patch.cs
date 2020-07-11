// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BetterColonistBar.UI;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterColonistBar.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class ColonistBarColonistDrawer_DrawIcons_Patch
    {
        private const string _targetString = "ActivityIconAttacking";

        private static MethodInfo _original =
            typeof(ColonistBarColonistDrawer).GetMethod("DrawIcons", BindingFlags.NonPublic | BindingFlags.Instance);

        private static MethodInfo _transpiler =
            typeof(ColonistBarColonistDrawer_DrawIcons_Patch)
                .GetMethod(nameof(Transpiler), BindingFlags.Public | BindingFlags.Static);

        private static MethodInfo _drawIcon =
            typeof(ColonistBarColonistDrawer).GetMethod("DrawIcon", BindingFlags.NonPublic | BindingFlags.Instance);

        private static MethodInfo _getStatusFor =
            typeof(BCBManager).GetMethod(nameof(BCBManager.GetStatusFor), BindingFlags.Public | BindingFlags.Static);

        private static MethodInfo _hasHediff =
            typeof(PawnStatusCache).GetProperty(nameof(PawnStatusCache.HasTendingHediff)).GetMethod;
            

        static ColonistBarColonistDrawer_DrawIcons_Patch()
        {
            BCBManager.Harmony.Patch(_original, transpiler: new HarmonyMethod(_transpiler));
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label newLabel = generator.DefineLabel();
            CodeInstruction drawCodeInstruction = new CodeInstruction(OpCodes.Call, _drawIcon);

            List<CodeInstruction> instList = instructions.ToList();
            Label retLabel = default;

            bool foundTargetString = false;
            int counter = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.OperandIs(_targetString))
                    foundTargetString = true;

                if (foundTargetString)
                {
                    counter++;
                }

                switch (counter)
                {
                    case 9:
                        retLabel = (Label)instruction.operand;
                        instruction.operand = newLabel;
                        break;
                    case 12:
                        instruction.operand = newLabel;
                        break;
                    case 20:
                        var startInst = new CodeInstruction(OpCodes.Ldarg_2);
                        startInst.labels.Add(newLabel);

                        yield return startInst;
                        yield return new CodeInstruction(OpCodes.Call, _getStatusFor);
                        yield return new CodeInstruction(OpCodes.Call, _hasHediff);
                        yield return new CodeInstruction(OpCodes.Brfalse, retLabel);
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(
                            OpCodes.Ldsfld
                            , typeof(BCBTexture)
                                .GetField(nameof(BCBTexture.Medicine), BindingFlags.Public | BindingFlags.Static));

                        yield return new CodeInstruction(
                            OpCodes.Ldloca_S, 1);

                        yield return new CodeInstruction(OpCodes.Ldstr, string.Empty);
                        yield return drawCodeInstruction;
                        break;
                }

                yield return instruction;
            }
        }
    }
}
