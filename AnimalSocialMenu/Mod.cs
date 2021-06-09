using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace AnimalSocialMenu
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        private static int myTabId = 0;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = this.Monitor;

            this.Helper.Events.Display.MenuChanged += this.onMenuChanged;
            myTabId = SpaceCore.Menus.ReserveGameMenuTab("animals");
        }

        private int myTabIndex = -1;
        private void onMenuChanged(object sender, MenuChangedEventArgs args)
        {
            if (args.NewMenu is GameMenu gm)
            {
                var pages = this.Helper.Reflection.GetField<List<IClickableMenu>>(gm, "pages").GetValue();
                var tabs = this.Helper.Reflection.GetField<List<ClickableComponent>>(gm, "tabs").GetValue();

                this.myTabIndex = tabs.Count;
                tabs.Add(new ClickableComponent(new Rectangle(gm.xPositionOnScreen + 192, gm.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 - 64, 64, 64), "animals", "Animals")
                {
                    myID = 912342,
                    downNeighborID = 12342,
                    rightNeighborID = 12343,
                    leftNeighborID = 12341,
                    tryDefaultIfNoDownNeighborExists = true,
                    fullyImmutable = true
                });
                tabs[1].upNeighborID = 912342;
                pages.Add((IClickableMenu)new AnimalSocialPage(gm.xPositionOnScreen, gm.yPositionOnScreen, gm.width, gm.height));

                this.Helper.Events.Display.RenderedActiveMenu += this.drawSocialIcon;
            }
            else if (args.OldMenu is GameMenu ogm)
            {
                this.Helper.Events.Display.RenderedActiveMenu -= this.drawSocialIcon;
            }
        }

        // The tab by default is rendered with the inventory icon due to how the tabs are hard-coded
        // This draws over it with the social icon instead of the inventory one
        private void drawSocialIcon(object sender, RenderedActiveMenuEventArgs e)
        {
            // For some reason this check is necessary despite removing it in the onMenuChanged event.
            if (!(Game1.activeClickableMenu is GameMenu menu))
            {
                this.Helper.Events.Display.RenderedActiveMenu -= this.drawSocialIcon;
                return;
            }
            if (menu.invisible || this.myTabIndex == -1)
                return;

            var tabs = this.Helper.Reflection.GetField<List<ClickableComponent>>(menu, "tabs").GetValue();
            if (tabs.Count <= this.myTabIndex)
            {
                return;
            }
            var tab = tabs[this.myTabIndex];
            e.SpriteBatch.Draw(Game1.mouseCursors, new Vector2((float)tab.bounds.X, (float)(tab.bounds.Y + (menu.currentTab == menu.getTabNumberFromName(tab.name) ? 8 : 0))), new Rectangle?(new Rectangle(2 * 16, 368, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);

            if (!Game1.options.hardwareCursor)
            {
                e.SpriteBatch.Draw(Game1.mouseCursors, new Vector2((float)Game1.getOldMouseX(), (float)Game1.getOldMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)(4.0 + (double)Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
            }
        }
    }
}
