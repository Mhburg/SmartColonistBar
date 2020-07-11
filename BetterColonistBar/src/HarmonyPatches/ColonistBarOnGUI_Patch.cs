// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BetterColonistBar.UI;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BetterColonistBar.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class ColonistBarOnGUI_Patch
    {
        private const float _startY = 21f;

        private static readonly MethodInfo _original = typeof(ColonistBar).GetMethod(nameof(ColonistBar.ColonistBarOnGUI));

        private static readonly MethodInfo _postfix =
            typeof(ColonistBarOnGUI_Patch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.Static);

        private static readonly Color _color = new Color(1, 1, 1, 0.8f);

        private static readonly Vector2 _minSize = new Vector2(20, 20);

        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        private static DateTime _lastTimeShow;

        private static bool _firstDraw = true;

        static ColonistBarOnGUI_Patch()
        {
            BCBManager.Harmony.Patch(_original, postfix: new HarmonyMethod(_postfix));
        }

        public static void Postfix()
        {
            if (Event.current.type == EventType.Layout)
                return;

            if (Event.current.type == EventType.Repaint)
            {
                _settings.UISettingsChanged = false;
                BCBManager.ModColonistBarDirty = false;
            }


            Rect buttonRect = GetRect();

            bool showButton = !_settings.AutoHide;
            if (!showButton)
            {
                bool timetoShowButton = false;

                if (_firstDraw)
                {
                    _firstDraw = false;
                    _lastTimeShow = DateTime.UtcNow;
                }
                else if (DateTime.UtcNow - _lastTimeShow < _settings.AutoHideButtonTime)
                {
                    timetoShowButton = true;
                }
                else if (MouseIsOver(buttonRect))
                {
                    timetoShowButton = true;
                    _lastTimeShow = DateTime.UtcNow;
                }

                showButton |= timetoShowButton;
            }

            if (showButton)
            {
                if (DrawUtility.ButtonInvertImage(buttonRect, BCBTexture.Expand, _color))
                {
                    if (Event.current.button == 0)
                    {
                        _settings.Expanded ^= true;
                        BCBManager.LastBarRect = Rect.zero;
                        Find.ColonistBar.MarkColonistsDirty();

                        return;
                    }
                    else if (Event.current.button == 1)
                    {
                    }
                }
            }

            if (Event.current.type != EventType.Repaint)
                return;

            if (BCBManager.UpdateColonistBar())
            {
                Find.ColonistBar.MarkColonistsDirty();
            }
        }

        private static Rect GetRect()
        {
            Vector2 size = Find.ColonistBar.Size * 0.8f;
            if (size.x < 20)
                size = _minSize;

            float gap = GenUI.Gap * Find.ColonistBar.Scale;
            if (!_settings.Expanded)
            {
                Rect rect;
                if (BCBManager.LastBarRect == Rect.zero)
                {
                    rect = new Rect(260f, _startY, size.x, size.y);
                    return new Rect(rect.xMax + gap, rect.y, size.x, size.x).CenteredOnYIn(new Rect(0, rect.y, 0, Find.ColonistBar.Size.x));
                }
                else
                {
                    rect = BCBManager.LastBarRect;
                    return new Rect(rect.xMax + gap, rect.y, size.x, size.x).CenteredOnYIn(BCBManager.LastBarRect);
                }
            }
            else
            {
                Rect rect = BCBManager.LastBarRect;
                return new Rect(rect.xMax + size.x + gap, rect.y, -size.x, size.y).CenteredOnYIn(BCBManager.LastBarRect);
            }
        }

        private static bool MouseIsOver(Rect rect)
        {
            if (rect.width < 0)
            {
                rect.width = Mathf.Abs(rect.width);
                rect.x -= rect.width;
            }

            return Mouse.IsOver(rect);
        }
    }
}
