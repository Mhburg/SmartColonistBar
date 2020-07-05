// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BetterColonistBar
{
    public struct MoodBarLocation : IEquatable<MoodBarLocation>
    {
        public MoodBarLocation(Rect barRect, Pawn pawn)
        {
            BarRect = barRect;
            Pawn = pawn;
        }

        public Rect BarRect { get; set; }

        public Pawn Pawn { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                return (BarRect.GetHashCode() * 397) ^ (Pawn != null ? Pawn.GetHashCode() : 0);
            }
        }

        public static bool operator ==(MoodBarLocation left, MoodBarLocation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MoodBarLocation left, MoodBarLocation right)
        {
            return !(left == right);
        }

        #region Equality members

        public bool Equals(MoodBarLocation other)
        {
            return BarRect.Equals(other.BarRect) && Equals(Pawn, other.Pawn);
        }

        public override bool Equals(object obj)
        {
            return obj is MoodBarLocation other && Equals(other);
        }

        #endregion
    }
}
