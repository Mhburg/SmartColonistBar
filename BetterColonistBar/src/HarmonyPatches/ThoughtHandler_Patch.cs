using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterColonistBar.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class ThoughtHandler_Patch
    {
        private static ThreadLocal<List<Thought>> _tmpThoughts = new ThreadLocal<List<Thought>>(() => new List<Thought>());

        private static ThreadLocal<List<Thought>> _tmpTotalMoodOffsetThoughts = new ThreadLocal<List<Thought>>(() => new List<Thought>());

        private static ThreadLocal<List<ISocialThought>> _tmpSocialThoughts = new ThreadLocal<List<ISocialThought>>(() => new List<ISocialThought>());

        private static ThreadLocal<List<ISocialThought>> _tmpTotalOpinionOffsetThoughts = new ThreadLocal<List<ISocialThought>>(() => new List<ISocialThought>());

        static ThoughtHandler_Patch()
        {
            BCBManager.Harmony.Patch(
                typeof(ThoughtHandler).GetMethod(nameof(ThoughtHandler.MoodOffsetOfGroup), AccessTools.all),
                transpiler: new HarmonyMethod(typeof(ThoughtHandler_Patch).GetMethod(nameof(MoodOffSetGroupTranspiler), AccessTools.all)));

            BCBManager.Harmony.Patch(
                typeof(ThoughtHandler).GetMethod(nameof(ThoughtHandler.TotalMoodOffset), AccessTools.all),
                transpiler: new HarmonyMethod(typeof(ThoughtHandler_Patch).GetMethod(nameof(TotalMoodOffsetTranspiler), AccessTools.all)));

            BCBManager.Harmony.Patch(
                typeof(ThoughtHandler).GetMethod(nameof(ThoughtHandler.OpinionOffsetOfGroup), AccessTools.all),
                transpiler: new HarmonyMethod(typeof(ThoughtHandler_Patch).GetMethod(nameof(OpinionOffsetOfGroupTranspiler), AccessTools.all)));

            BCBManager.Harmony.Patch(
                typeof(ThoughtHandler).GetMethod(nameof(ThoughtHandler.TotalOpinionOffset), AccessTools.all),
                transpiler: new HarmonyMethod(typeof(ThoughtHandler_Patch).GetMethod(nameof(TotalOpinionOffsetTranspiler), AccessTools.all)));
        }

        [HarmonyDebug]
        public static IEnumerable<CodeInstruction> MoodOffSetGroupTranspiler(IEnumerable<CodeInstruction> code, ILGenerator ilGenerator)
        {
            return ReflectionUtility.ReplaceStaticFieldWithThreadLocal<List<Thought>>(
                code
                , ilGenerator
                , typeof(ThoughtHandler).GetField("tmpThoughts", AccessTools.all)
                , typeof(ThoughtHandler_Patch).GetField(nameof(_tmpThoughts), AccessTools.all));
        }

        [HarmonyDebug]
        public static IEnumerable<CodeInstruction> TotalMoodOffsetTranspiler(IEnumerable<CodeInstruction> code, ILGenerator ilGenerator)
        {
            return ReflectionUtility.ReplaceStaticFieldWithThreadLocal<List<Thought>>(
                code
                , ilGenerator
                , typeof(ThoughtHandler).GetField("tmpTotalMoodOffsetThoughts", AccessTools.all)
                , typeof(ThoughtHandler_Patch).GetField(nameof(_tmpTotalMoodOffsetThoughts), AccessTools.all));
        }

        [HarmonyDebug]
        public static IEnumerable<CodeInstruction> OpinionOffsetOfGroupTranspiler(IEnumerable<CodeInstruction> code, ILGenerator ilGenerator)
        {
            return ReflectionUtility.ReplaceStaticFieldWithThreadLocal<List<ISocialThought>>(
                code
                , ilGenerator
                , typeof(ThoughtHandler).GetField("tmpSocialThoughts", AccessTools.all)
                , typeof(ThoughtHandler_Patch).GetField(nameof(_tmpSocialThoughts), AccessTools.all));
        }

        [HarmonyDebug]
        public static IEnumerable<CodeInstruction> TotalOpinionOffsetTranspiler(IEnumerable<CodeInstruction> code, ILGenerator ilGenerator)
        {
            return ReflectionUtility.ReplaceStaticFieldWithThreadLocal<List<ISocialThought>>(
                code
                , ilGenerator
                , typeof(ThoughtHandler).GetField("tmpTotalOpinionOffsetThoughts", AccessTools.all)
                , typeof(ThoughtHandler_Patch).GetField(nameof(_tmpTotalOpinionOffsetThoughts),AccessTools.all));
        }
    }
}
