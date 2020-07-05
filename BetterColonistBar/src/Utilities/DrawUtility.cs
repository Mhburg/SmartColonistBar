// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BetterColonistBar.UI
{
    public static class DrawUtility
    {
        public static Texture2D GetTexture(this MoodLevel moodLevel)
        {
            switch (moodLevel)
            {
                case MoodLevel.Satisfied:
                    return BCBTexture.Satisfied;

                case MoodLevel.Minor:
                    return BCBTexture.Minor;

                case MoodLevel.Major:
                    return BCBTexture.Major;

                case MoodLevel.Extreme:
                    return BCBTexture.Extreme;

                default:
                    return BCBTexture.Satisfied;
            }
        }

        public static float PaddingTiny = 2f;

        public static bool ButtonInvertImage(Rect rect, Texture2D image, Color baseColor)
        {
            Rect realRect = rect;
            if (rect.width < 0)
            {
                realRect.width = Math.Abs(rect.width);
                realRect.x -= realRect.width;
            }

            GUI.color = Mouse.IsOver(realRect) ? GenUI.MouseoverColor : baseColor;

            GUI.DrawTexture(rect, image);

            GUI.color = Color.white;

            return Widgets.ButtonInvisible(realRect);
        }
    }
}
