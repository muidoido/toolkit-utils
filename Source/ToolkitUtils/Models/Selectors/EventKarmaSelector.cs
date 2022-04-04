﻿// MIT License
// 
// Copyright (c) 2021 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Helpers;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class EventKarmaSelector : ISelectorBase<EventItem>
    {
        private string _karmaLabel;
        private KarmaType _karmaType = KarmaType.Neutral;
        private List<FloatMenuOption> _karmaTypes;

        public ObservableProperty<bool> Dirty { get; set; }

        public void Prepare()
        {
            _karmaLabel = Label;
            _karmaTypes = Data.KarmaTypes.Select(i => new FloatMenuOption(i.ToString(), () => SetKarma(i))).ToList();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.Split(0.75f);
            UiHelper.Label(label, _karmaLabel);

            if (Widgets.ButtonText(field, _karmaType.ToString()))
            {
                Find.WindowStack.Add(new FloatMenu(_karmaTypes));
            }
        }

        public bool IsVisible([NotNull] TableSettingsItem<EventItem> item) => item.Data.KarmaType == _karmaType;

        public string Label => "TKUtils.Fields.KarmaType".TranslateSimple();

        private void SetKarma(KarmaType karma)
        {
            _karmaType = karma;
            Dirty.Set(true);
        }
    }
}
