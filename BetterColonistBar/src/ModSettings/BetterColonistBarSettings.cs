using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BetterColonistBar
{
    /// <summary>
    /// Settings for this mod.
    /// </summary>
    public class BetterColonistBarSettings : ModSettings
    {
        #region Features

        private static int _updateIntervalBase = 30;

        public int BleedingRateUpdateInterval = _updateIntervalBase;

        public int MoodUpdateInterval = _updateIntervalBase;

        public MoodLevel shownMoodLevel = MoodLevel.Minor;

        public bool ShowInspiredPawn = true;

        public bool ShowGuestPawn = true;

        public bool ShowSickPawn = true;

        public bool ShowDraftedPawn = true;

        public bool SortBleedingPawn = true;

        public float MoodBarWidth = 1 / 5f;

        public bool Expanded = false;

        public int StatusUpdateInterval = _updateIntervalBase;

        public TimeSpan AutoHideButtonTime = new TimeSpan(0, 0, 3);

        public Vector2 ColorPickerSize = new Vector2(600, 600);

        #endregion

        #region UI

        public Color Satisfied = ColorLibrary.Cyan;

        public Color Minor = ColorLibrary.Gold;

        public Color Major = ColorLibrary.DarkOrange;

        public Color Extreme = ColorLibrary.Red;

        public Color BgColor = Color.grey;

        public Color ThresholdMarker = Color.black;

        public Color CurrMoodLevel = Color.white;

        public bool UISettingsChanged = false;

        public int ThresholdMarkerThickness = Mathf.RoundToInt(GenUI.GapTiny / 2);

        public int CurrMoodLevelThickness = 2;

        public bool AutoHide = false;

        #endregion
    }
}
