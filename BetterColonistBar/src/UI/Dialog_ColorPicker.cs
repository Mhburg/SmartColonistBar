// Copyright (c) 2019 - 2020 Zizhen Li. All rights reserved.
// Licensed under the LGPL-3.0-only license. See LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorldUtility.UI;
using UnityEngine;
using Verse;

namespace BetterColonistBar.UI
{
    public class Dialog_ColorPicker : Window
    {
        private const int _defaultPalletHeight = DrawUtility.MaxRGBValue * 2;

        private static readonly BetterColonistBarSettings _settings = BetterColonistBarMod.ModSettings;

        private readonly ColorPicker _colorPicker;

        private readonly ColorPickerModel _model;

        public Dialog_ColorPicker(Color initColor, Action<Color> apply)
        {
            this.closeOnClickedOutside = true;
            this.closeOnCancel = true;
            this.doCloseX = true;
            this.draggable = true;
            this.absorbInputAroundWindow = true;

            _colorPicker = new ColorPicker(BCBTexture.PalletMarker, apply);
            _model = new ColorPickerModel(initColor);
        }

        #region Overrides of Window

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout)
                return;

            _colorPicker.Draw(_model, inRect.position);
        }

        public override void PreOpen()
        {
            base.PreOpen();
            windowRect = new Rect(windowRect.position, _settings.ColorPickerSize).ExpandedBy(this.Margin);
            _model.Size = _settings.ColorPickerSize;
            _model.PalletSize = new Vector2(DrawUtility.MaxRGBValue, _defaultPalletHeight);
            _model.SliderSize = new Vector2(GenUI.GapWide, _defaultPalletHeight / 3);
            _model.RgbTextFieldSize = new Vector2(400f, 100f);
            _colorPicker.Build(_model);
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();
            if (Event.current.type != EventType.Layout)
                _colorPicker.Update(_model);
        }

        #endregion
    }
}
