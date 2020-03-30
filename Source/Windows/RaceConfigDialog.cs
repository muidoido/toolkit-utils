﻿using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Incidents;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class RaceConfigDialog : Window
    {
        private readonly List<ShopExpansion.Race> cache = TkUtils.ShopExpansion.races;
        private string currentQuery = "";
        private string lastQuery = "";
        private IReadOnlyCollection<ShopExpansion.Race> results;
        private Vector2 scrollPos = Vector2.zero;

        public RaceConfigDialog()
        {
            doCloseX = true;
            forcePause = true;

            optionalTitle = "TKUtils.Windows.Races.Title".Translate();
            cache.SortBy(t => t.name.NullOrEmpty() ? t.defName : t.name);
        }

        public override Vector2 InitialSize => new Vector2(640f, 740f);

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard {maxOneColumn = true};
            var searchRect = new Rect(inRect.x, inRect.y, inRect.width * 0.3f - 28f, Text.LineHeight);
            var clearRect = new Rect(searchRect.x + searchRect.width + 2f, searchRect.y, 28f, Text.LineHeight);

            currentQuery = Widgets.TextEntryLabeled(
                searchRect,
                "TKUtils.Windows.Config.Buttons.Search.Label".Translate(),
                currentQuery
            );

            var old = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;

            if (Widgets.ButtonText(clearRect, "X"))
            {
                currentQuery = "";
            }

            if (currentQuery == "")
            {
                results = null;
                lastQuery = "";
            }

            Text.Anchor = old;
            var enableText = "TKUtils.Windows.Config.Buttons.EnableAll.Label".Translate();
            var disableText = "TKUtils.Windows.Config.Buttons.DisableAll.Label".Translate();
            var enableSize = Text.CalcSize(enableText);
            var disableSize = Text.CalcSize(disableText);
            var disableRect = new Rect(
                inRect.width - disableSize.x * 1.5f,
                inRect.y,
                disableSize.x * 1.5f,
                Text.LineHeight
            );
            var enableRect = new Rect(
                inRect.width - disableSize.x * 1.5f - enableSize.x * 1.5f,
                inRect.y,
                enableSize.x * 1.5f,
                Text.LineHeight
            );

            if (Widgets.ButtonText(enableRect, enableText))
            {
                foreach (var race in TkUtils.ShopExpansion.races)
                {
                    race.enabled = true;
                }
            }

            if (Widgets.ButtonText(disableRect, disableText))
            {
                foreach (var race in TkUtils.ShopExpansion.races)
                {
                    race.enabled = true;
                }
            }

            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 2f, inRect.width);

            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 3f,
                inRect.width,
                inRect.height - Text.LineHeight * 3f
            );

            var total = results?.Count ?? cache.Count;
            var viewPort = new Rect(contentArea.x, 0f, contentArea.width * 0.9f, Text.LineHeight * total + 1);

            listing.BeginScrollView(contentArea, ref scrollPos, ref viewPort);
            foreach (var race in results ?? cache)
            {
                var lineRect = listing.GetRect(Text.LineHeight);
                var boxRect = new Rect(contentArea.x, lineRect.y, lineRect.width * 0.84f, lineRect.height);
                Widgets.CheckboxLabeled(race.enabled ? boxRect : lineRect, race.defName, ref race.enabled);

                var inputRect = new Rect(
                    lineRect.width - contentArea.width * 0.14f,
                    lineRect.y,
                    contentArea.width * 0.14f,
                    lineRect.height
                );

                var valueBuffer = race.price.ToString();
                Widgets.TextFieldNumeric(inputRect, ref race.price, ref valueBuffer);
            }

            listing.EndScrollView(ref viewPort);
        }

        private void Notify__SearchRequested()
        {
            lastQuery = currentQuery;

            results = GetSearchResults();
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();

            if (lastQuery.Equals(currentQuery))
            {
                return;
            }

            if (Time.time % 2 < 1)
            {
                Notify__SearchRequested();
            }
        }

        private IReadOnlyCollection<ShopExpansion.Race> GetSearchResults()
        {
            var serialized = currentQuery?.ToToolkit();

            if (serialized == null)
            {
                return null;
            }

            var searchResults = cache.Where(
                    t => t.defName.ToToolkit().EqualsIgnoreCase(currentQuery.ToToolkit())
                         || t.defName.ToToolkit().Contains(currentQuery.ToToolkit())
                )
                .ToList();

            return searchResults;
        }

        public override void PreClose()
        {
            ShopExpansionHelper.SaveShopExtension();
        }
    }
}
