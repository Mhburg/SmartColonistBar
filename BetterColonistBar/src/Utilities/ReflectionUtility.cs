using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;

namespace BetterColonistBar
{
    public static class ReflectionUtility
    {
        public static IEnumerable<CodeInstruction> ReplaceStaticFieldWithThreadLocal<T>(IEnumerable<CodeInstruction> code, ILGenerator ilGenerator,
            FieldInfo targetField, FieldInfo threadLocal)
        {
            LocalBuilder threadLocalBuilder = ilGenerator.DeclareLocal(typeof(T));
            List<CodeInstruction> instructions = code.ToList();

            instructions.Insert(0, new CodeInstruction(OpCodes.Ldsfld, threadLocal));
            instructions.Insert(1, new CodeInstruction(OpCodes.Call, typeof(ThreadLocal<T>).GetProperty(nameof(ThreadLocal<T>.Value), AccessTools.all).GetMethod));
            instructions.Insert(2, new CodeInstruction(OpCodes.Stloc, threadLocalBuilder));

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Is(OpCodes.Ldsfld, targetField))
                {
                    instruction.opcode = OpCodes.Ldloc;
                    instruction.operand = threadLocalBuilder;
                }
            }

            return instructions;
        }
    }
}
