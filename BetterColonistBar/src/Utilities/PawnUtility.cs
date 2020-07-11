// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorldUtility;
using Verse;
using Verse.AI;

namespace BetterColonistBar
{
    public static class PawnUtility
    {
        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        public static bool ShouldShowBar(this Pawn pawn)
        {
            ValidateArg.NotNull(pawn, nameof(pawn));

            if (_settings.ShowInspiredPawn && pawn.Inspired)
                return true;

            if (_settings.ShowSickPawn && BCBManager.GetStatusFor(pawn).HasTendingHediff)
                return true;

            if (_settings.ShowGuestPawn && (pawn.IsQuestLodger() || pawn.IsQuestHelper()))
                return true;

            if (_settings.ShowDraftedPawn && pawn.Drafted)
                return true;

            if ((pawn.mindState?.IsIdle ?? false) && GenDate.DaysPassed > 1)
                return true;

            if (pawn.InMentalState)
                return true;

            if (pawn.IsBurning())
                return true;

            if (pawn.CurJob?.def == JobDefOf.FleeAndCower)
                return true;

            MoodLevel shownMoodLevel = BetterColonistBarMod.ModSettings.ShownMoodLevel;

            return BCBManager.GetBreakLevelFor(pawn).MoodLevel <= shownMoodLevel;
        }
    }
}
