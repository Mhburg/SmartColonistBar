// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BetterColonistBar.UI;
using RimWorldUtility.UI;
using Verse;

namespace BetterColonistBar
{
    public class BCBComponent : GameComponent
    {
        public static bool Expanded = true;

        private bool _init;

        public BCBComponent(Game game)
        {
        }

        #region Overrides of GameComponent

        public override void FinalizeInit()
        {
            ColonistBarUitlity.Init();
            BCBManager.Reset();
            BetterColonistBarMod.Reset();

            if (_init)
                BCBManager.UpdateColonistBar();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Expanded, nameof(Expanded), true);
        }

        public override void GameComponentTick()
        {
            if (BetterColonistBarMod.HasException)
            { 
                BCBManager.Harmony.UnpatchAll(BetterColonistBarMod.Id);
                Find.WindowStack.Add(new Dialog_ErrorReporting(BuildExceptionString(), "Exception", BetterColonistBarMod.BugReportUrl));
                BetterColonistBarMod.HasException = false;
            }
        }

        private static string BuildExceptionString()
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(UIText.Version.TranslateSimple());
            stringBuilder.Append(" -- ");
            stringBuilder.AppendLine(UIText.Commit.TranslateSimple());
            stringBuilder.AppendLine(BetterColonistBarMod.AssemblyName?.FullName ?? "Assembly not found");
            stringBuilder.AppendLine(BetterColonistBarMod.ExceptionReport.ExtraString);

            foreach (Exception e in BetterColonistBarMod.ExceptionReport.Exceptions)
            {
                stringBuilder.AppendLine(e is AggregateException agg ? agg.Flatten().ToString() : e.ToString());
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}
