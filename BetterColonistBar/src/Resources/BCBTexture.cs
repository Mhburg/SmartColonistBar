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

        public static Texture2D PalletMarker = ContentFinder<Texture2D>.Get("PalletMarker");

        public static Texture2D Medicine = (Texture2D)(ThingDefOf.MedicineIndustrial.graphic as Graphic_StackCount)
            .SubGraphicForStackCount(1, ThingDefOf.MedicineIndustrial).MatNorth.mainTexture;

        public static Texture2D Satisfied = SolidColorMaterials.NewSolidColorTexture(MoodColor.Satisfied);

        public static Texture2D Minor = SolidColorMaterials.NewSolidColorTexture(MoodColor.Minor);

        public static Texture2D Major = SolidColorMaterials.NewSolidColorTexture(MoodColor.Major);

        public static Texture2D Extreme = SolidColorMaterials.NewSolidColorTexture(MoodColor.Extreme);
    }
}
