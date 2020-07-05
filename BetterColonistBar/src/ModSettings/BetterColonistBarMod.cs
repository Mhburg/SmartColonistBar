// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterColonistBar.UI;
using UnityEngine;
using Verse;

namespace BetterColonistBar
{
    /// <summary>
    /// Handles assets and settings for this mod.
    /// </summary>
    public class BetterColonistBarMod : Mod
    {
        public const string Id = "NotooShabby.BetterColonistBar";


        /// <summary>
        /// Initialize an instance of <see cref="BetterColonistBarMod"/>.
        /// </summary>
        /// <param name="content"> Assets for this mod. </param>
        public BetterColonistBarMod(ModContentPack content)
            : base(content)
        {
            ModSettings = this.GetSettings<BetterColonistBarSettings>();
            BCBManager.Reset();
        }

        /// <summary>
        /// Gets the setting for this mod.
        /// </summary>
        public static BetterColonistBarSettings ModSettings { get; private set; }

        #region Overrides of Mod

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect.x, inRect.y, 255, 255);
            GUI.DrawTexture(rect, BCBTexture._colorPallet);
        }

        public override string SettingsCategory()
        {
            return "Better Colonist Bar";
        }

        #endregion
    }
}
