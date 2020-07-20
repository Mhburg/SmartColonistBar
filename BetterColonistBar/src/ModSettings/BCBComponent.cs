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

        public BCBComponent(Game game)
        {
        }

        #region Overrides of GameComponent

        public override void FinalizeInit()
        {
            ColonistBarUitlity.Init();
            BCBManager.Reset();
            BetterColonistBarMod.Reset();
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
                Find.WindowStack.Add(
                    new Dialog_ErrorReporting(
                        string.Concat(
                            UIText.Version.TranslateSimple()
                            , " -- "
                            , UIText.Commit.TranslateSimple()
                            , Environment.NewLine
                            , BetterColonistBarMod.AssemblyName?.FullName ?? "Assembly not found"
                            , Environment.NewLine
                            , BetterColonistBarMod.Exception.ToString())));
                BetterColonistBarMod.HasException = false;
            }

            BCBManager.UpdateCache();
        }

        #endregion
    }
}
