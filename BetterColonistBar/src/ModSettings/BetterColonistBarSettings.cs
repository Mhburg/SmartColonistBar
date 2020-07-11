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

        public MoodLevel ShownMoodLevel = MoodLevel.Minor;

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

        public float YOffset = 21f;

        #endregion

        static BetterColonistBarSettings()
        {
            ParseHelper.Parsers<MoodLevel>.Register(str => (MoodLevel)Enum.Parse(typeof(MoodLevel), str));
            ParseHelper.Parsers<TimeSpan>.Register(str => TimeSpan.Parse(str));
        }

        #region Overrides of ModSettings

        public override void ExposeData()
        {
            // Save settings on features
            Scribe_Values.Look(ref BleedingRateUpdateInterval, nameof(BleedingRateUpdateInterval), _updateIntervalBase);
            Scribe_Values.Look(ref MoodUpdateInterval, nameof(MoodUpdateInterval), _updateIntervalBase);
            Scribe_Values.Look(ref StatusUpdateInterval, nameof(StatusUpdateInterval), _updateIntervalBase);
            Scribe_Values.Look(ref ShownMoodLevel, nameof(ShownMoodLevel), MoodLevel.Minor);
            Scribe_Values.Look(ref ShowInspiredPawn, nameof(ShowInspiredPawn), true);
            Scribe_Values.Look(ref ShowGuestPawn, nameof(ShowGuestPawn), true);
            Scribe_Values.Look(ref ShowSickPawn, nameof(ShowSickPawn), true);
            Scribe_Values.Look(ref ShowDraftedPawn, nameof(ShowDraftedPawn), true);
            Scribe_Values.Look(ref SortBleedingPawn, nameof(SortBleedingPawn), true);
            Scribe_Values.Look(ref AutoHideButtonTime, nameof(AutoHideButtonTime), new TimeSpan(0, 0, 3));

            // Save settings on UI
            Scribe_Values.Look(ref Satisfied, nameof(Satisfied), ColorLibrary.Cyan);
            Scribe_Values.Look(ref Minor, nameof(Minor), ColorLibrary.Gold);
            Scribe_Values.Look(ref Major, nameof(Major), ColorLibrary.DarkOrange);
            Scribe_Values.Look(ref Extreme, nameof(Extreme), ColorLibrary.Red);
            Scribe_Values.Look(ref BgColor, nameof(BgColor), Color.grey);
            Scribe_Values.Look(ref ThresholdMarker, nameof(ThresholdMarker), Color.black);
            Scribe_Values.Look(ref CurrMoodLevel, nameof(CurrMoodLevel), Color.white);
            Scribe_Values.Look(ref ThresholdMarkerThickness, nameof(ThresholdMarkerThickness), 2);
            Scribe_Values.Look(ref CurrMoodLevelThickness, nameof(CurrMoodLevelThickness), 2);
            Scribe_Values.Look(ref AutoHide, nameof(AutoHide));
            Scribe_Values.Look(ref YOffset, nameof(YOffset), 21);
        }

        #endregion
    }
}
