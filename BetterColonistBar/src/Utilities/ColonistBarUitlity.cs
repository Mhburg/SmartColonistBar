// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterColonistBar
{
    public static class ColonistBarUitlity
    {
        private static readonly FieldInfo _entriesField =
            typeof(ColonistBar).GetField("cachedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

        private static List<ColonistBar.Entry> _entries;

        public static void Init()
        {
            _entries = (List<ColonistBar.Entry>)_entriesField.GetValue(Find.ColonistBar);
        }

        public static List<ColonistBar.Entry> GetEntries(this ColonistBar bar)
        {
            return _entries;
        }

        public static void UpdateEntries(this ColonistBar bar, List<ColonistBar.Entry> newEntries)
        {
            _entries.Clear();
            _entries.AddRange(newEntries);
        }
    }
}
