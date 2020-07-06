using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BetterColonistBar
{
    /// <summary>
    /// Settings for this mod.
    /// </summary>
    public class BetterColonistBarSettings : ModSettings
    {
        private static int _updateIntervalBase = 30;

        public int MoodUpdateInterval = _updateIntervalBase;

        public MoodLevel shownMoodLevel = MoodLevel.Minor;

        public bool ShowInspiredPawn = true;

        public bool ShowGuestPawn = true;

        public bool ShowIllPawn = true;

        public bool ShowDraftedPawn = true;

        public float MoodBarWidth = 1 / 5f;

        public bool Expanded = false;

        public int StatusUpdateInterval = _updateIntervalBase;

        public TimeSpan AutoHideButtonTime = new TimeSpan(0,0,3);
    }
}
