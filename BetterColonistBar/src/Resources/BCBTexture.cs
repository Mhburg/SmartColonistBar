// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;

namespace BetterColonistBar.UI
{
    [StaticConstructorOnStartup]
    public static class BCBTexture
    {
        public static Texture2D Expand = ContentFinder<Texture2D>.Get("Expand");
        public static Texture2D RightTriangle = ContentFinder<Texture2D>.Get("RightTriangle");

        public static Texture2D Medicine = (Texture2D)(ThingDefOf.MedicineIndustrial.graphic as Graphic_StackCount)
            .SubGraphicForStackCount(1, ThingDefOf.MedicineIndustrial).MatNorth.mainTexture;

        public static Texture2D Satisfied = SolidColorMaterials.NewSolidColorTexture(MoodColor.Satisfied);

        public static Texture2D Minor = SolidColorMaterials.NewSolidColorTexture(MoodColor.Minor);

        public static Texture2D Major = SolidColorMaterials.NewSolidColorTexture(MoodColor.Major);

        public static Texture2D Extreme = SolidColorMaterials.NewSolidColorTexture(MoodColor.Extreme);

        public static Texture2D _colorPallet = new Texture2D(255, 255);

        static BCBTexture()
        {
            _colorPallet.name = "ColorPicker";

            for (int y = 0; y < _colorPallet.height; y++)
            {
                for (int x = 0; x < _colorPallet.width; x++)
                {
                    _colorPallet.SetPixel(x, y, new Color(x / 255f, y / 255f, y / 255f, 1));
                }
            }
            _colorPallet.Apply();
        }
    }
}
