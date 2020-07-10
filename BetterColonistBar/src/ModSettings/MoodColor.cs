// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using UnityEngine;
using Verse;

namespace BetterColonistBar
{
    /// <summary>
    /// Color for drawing mood at different levels.
    /// </summary>
    public static class MoodColor
    {
        public static Color Satisfied => BetterColonistBarMod.ModSettings.Satisfied;

        public static Color Minor => BetterColonistBarMod.ModSettings.Minor;

        public static Color Major => BetterColonistBarMod.ModSettings.Major;

        public static Color Extreme => BetterColonistBarMod.ModSettings.Extreme;
    }
}
