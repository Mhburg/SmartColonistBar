// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorldUtility;
using Verse;

namespace BetterColonistBar
{
    public class BreakLevelCacheMaker : CacheMaker<BreakLevelCache, Pawn>
    {
        #region Overrides of CacheMaker<BreakLevelCache,Pawn>

        public override BreakLevelCache Make(Pawn key)
        {
            return new BreakLevelCache(key);
        }

        #endregion
    }
}
