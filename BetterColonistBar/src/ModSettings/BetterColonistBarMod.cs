// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterColonistBar.UI;
using RimWorld;
using RimWorldUtility;
using UnityEngine;
using Verse;

namespace BetterColonistBar
{
    /// <summary>
    /// Handles assets and settings for this mod.
    /// </summary>
    public class BetterColonistBarMod : Mod
    {
        private static readonly Dictionary<Color, Texture2D> _textureCache = new Dictionary<Color, Texture2D>();

        public const string Id = "NotooShabby.BetterColonistBar";

        public const string Name = "Smart Colonist Bar";

        /// <summary>
        /// Initialize an instance of <see cref="BetterColonistBarMod"/>.
        /// </summary>
        /// <param name="content"> Assets for this mod. </param>
        public BetterColonistBarMod(ModContentPack content)
            : base(content)
        {
            ModSettings = this.GetSettings<BetterColonistBarSettings>();
        }

        /// <summary>
        /// Gets the setting for this mod.
        /// </summary>
        public static BetterColonistBarSettings ModSettings { get; private set; }

        #region Overrides of Mod

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard { ColumnWidth = inRect.width / 3 };
            listing.Begin(inRect);
            listing.CheckboxLabeled(UIText.ShowInspiredPawn.TranslateSimple(), ref ModSettings.ShowInspiredPawn);
            listing.CheckboxLabeled(UIText.ShowGuestPawn.TranslateSimple(), ref ModSettings.ShowGuestPawn);
            listing.CheckboxLabeled(UIText.ShowSickPawn.TranslateSimple(), ref ModSettings.ShowSickPawn);
            listing.CheckboxLabeled(UIText.ShowDraftedPawn.TranslateSimple(), ref ModSettings.ShowDraftedPawn);
            listing.CheckboxLabeled(UIText.AutoHide.TranslateSimple(), ref ModSettings.AutoHide);

            listing.GapLine();

            listing.CheckboxLabeled(UIText.SortPawnByBleeding.TranslateSimple(), ref ModSettings.SortBleedingPawn);

            const float lineHeight = GenUI.ListSpacing * 2;

            listing.Gap();
            Draw.IntegerSetting($"{UIText.MoodUpdateInterval.TranslateSimple()}:", listing.GetRect(lineHeight), ref ModSettings.MoodUpdateInterval, tooltip: UIText.TickExplaination.TranslateSimple());

            listing.Gap();
            Draw.IntegerSetting($"{UIText.StatusUpdateInterval.TranslateSimple()}:", listing.GetRect(lineHeight), ref ModSettings.StatusUpdateInterval, tooltip: UIText.TickExplaination.TranslateSimple());

            listing.Gap();
            int seconds = ModSettings.AutoHideButtonTime.Seconds;
            Draw.IntegerSetting($"{UIText.TimeToAutoHide.TranslateSimple()}:", listing.GetRect(lineHeight), ref seconds);
            if (ModSettings.AutoHideButtonTime.Seconds != seconds)
                ModSettings.AutoHideButtonTime = new TimeSpan(0, 0, seconds);

            listing.NewColumn();

            DrawColorOption(listing, MoodLevel.Satisfied);
            DrawColorOption(listing, MoodLevel.Minor);
            DrawColorOption(listing, MoodLevel.Major);
            DrawColorOption(listing, MoodLevel.Extreme);
            if (listing.ButtonText(ModSettings.ShownMoodLevel.ToString()))
            {
                FloatMenuUtility.MakeMenu(
                    Enum.GetValues(typeof(MoodLevel)).Cast<MoodLevel>().Where(e => e != MoodLevel.Undefined)
                    , m => m.ToString()
                    , m => () => ModSettings.ShownMoodLevel = m);
            }

            listing.GapLine();

            DrawColorOption(listing, UIText.BackgroundColor.TranslateSimple(), ModSettings.BgColor, (color) => ModSettings.BgColor = color);
            DrawColorOption(listing, UIText.ThrehsoldMarker.TranslateSimple(), ModSettings.ThresholdMarker, (color) => ModSettings.ThresholdMarker = color);
            DrawColorOption(listing, UIText.CurrMoodLevel.TranslateSimple(), ModSettings.CurrMoodLevel, color => ModSettings.CurrMoodLevel = color);

            listing.GapLine();
            Draw.IntegerSetting(
                $"{UIText.ThrehsoldMarkerThickness.TranslateSimple()}:"
                , listing.GetRect(lineHeight)
                , ref ModSettings.ThresholdMarkerThickness
                , i => ModSettings.UISettingsChanged = true);

            listing.Gap();
            Draw.IntegerSetting(
                $"{UIText.CurrMoodLevelThickness.TranslateSimple()}:"
                , listing.GetRect(lineHeight)
                , ref ModSettings.CurrMoodLevelThickness
                , i => ModSettings.UISettingsChanged = true);

            int yOffset = Mathf.RoundToInt(ModSettings.YOffset);
            listing.Gap();
            Draw.IntegerSetting(
                $"{UIText.YOffset.TranslateSimple()}:"
                , listing.GetRect(lineHeight)
                , ref yOffset
                , i =>
                {
                    ModSettings.YOffset = yOffset;
                    Find.ColonistBar.MarkColonistsDirty();
                    BCBManager.ModColonistBarDirty = true;
                });

            listing.End();
        }

        public override string SettingsCategory()
        {
            return Name;
        }

        #endregion

        private static void DrawColorOption(Listing_Standard listing, MoodLevel moodLevel)
        {
            Rect rect = listing.GetRect(GenUI.ListSpacing);
            WidgetRow row = new WidgetRow(rect.x, rect.y, UIDirection.RightThenDown, rect.width);

            if (row.ButtonIcon(moodLevel.GetTexture()))
            {
                Find.WindowStack.Add(new Dialog_ColorPicker(
                    moodLevel.GetColor(),
                    (color) =>
                    {
                        switch (moodLevel)
                        {
                            case MoodLevel.Satisfied:
                                ModSettings.Satisfied = color;
                                BCBTexture.Satisfied = SolidColorMaterials.NewSolidColorTexture(color);
                                break;
                            case MoodLevel.Minor:
                                ModSettings.Minor = color;
                                BCBTexture.Minor = SolidColorMaterials.NewSolidColorTexture(color);
                                break;
                            case MoodLevel.Major:
                                ModSettings.Major = color;
                                BCBTexture.Major = SolidColorMaterials.NewSolidColorTexture(color);
                                break;
                            case MoodLevel.Extreme:
                                ModSettings.Extreme = color;
                                BCBTexture.Extreme = SolidColorMaterials.NewSolidColorTexture(color);
                                break;
                        }

                        ModSettings.UISettingsChanged = true;
                    }));
            }

            row.GapButtonIcon();
            row.Label(moodLevel.ToString());
        }

        private static void DrawColorOption(Listing_Standard listing, string label, Color color, Action<Color> action)
        {
            Rect rect = listing.GetRect(GenUI.ListSpacing);
            WidgetRow row = new WidgetRow(rect.x, rect.y, UIDirection.RightThenDown, rect.width);

            if (!_textureCache.TryGetValue(color, out Texture2D texture))
            {
                texture = SolidColorMaterials.NewSolidColorTexture(color);
                _textureCache[color] = texture;
            }

            if (row.ButtonIcon(texture))
            {
                Find.WindowStack.Add(
                    new Dialog_ColorPicker(
                        color
                        , (newColor) =>
                        {
                            action.Invoke(newColor);
                            ModSettings.UISettingsChanged = true;
                        }));
            }

            row.GapButtonIcon();
            row.Label(label);
        }
    }
}
