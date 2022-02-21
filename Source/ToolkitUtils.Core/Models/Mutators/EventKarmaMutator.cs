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

using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class EventKarmaMutator : IMutatorBase<EventItem>
    {
        private KarmaType _karmaType = KarmaType.Neutral;
        private string _karmaTypeLabel;

        public int Priority => 1;

        public string Label => "TKUtils.Fields.KarmaType".TranslateSimple();

        public void Prepare()
        {
            _karmaTypeLabel = "TKUtils.Fields.KarmaType".TranslateSimple();
        }

        public void Draw(Rect canvas)
        {
            (Rect label, Rect field) = canvas.ToForm(0.75f);
            SettingsHelper.DrawLabel(label, _karmaTypeLabel);

            if (Widgets.ButtonText(field, _karmaType.ToString()))
            {
                Find.WindowStack.Add(new FloatMenu(Data.KarmaTypes.Select(i => new FloatMenuOption(i.ToString(), () => _karmaType = i)).ToList()));
            }
        }

        public void Mutate([NotNull] TableSettingsItem<EventItem> item)
        {
            item.Data.KarmaType = _karmaType;
        }
    }
}
