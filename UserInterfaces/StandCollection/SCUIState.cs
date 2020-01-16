using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.UserInterfaces.Elements.StandCollection;

namespace TerrarianBizzareAdventure.UserInterfaces.StandCollection
{
    public class SCUIState : TBAUIState
    {
        private const float
            PANEL_WIDTH = 630,
            PANEL_HEIGHT = 600;

        public override void OnInitialize()
        {
            MainPanel = new UIPanel();
            MainPanel.Width.Set(PANEL_WIDTH, 0);
            MainPanel.Height.Set(PANEL_HEIGHT, 0);
            MainPanel.SetPadding(0);
            MainPanel.VAlign = 0.5f;
            MainPanel.HAlign = 0.5f;

            SCGridScrollBar = new UIScrollbar();
            SCGridScrollBar.Width.Set(20, 0);
            SCGridScrollBar.Height.Set(350, 0);
            SCGridScrollBar.VAlign = 0.5f;
            SCGridScrollBar.Left.Set(5, 0);

            var bgPanel = new UIPanel();
            bgPanel.Width.Set(585, 0);
            bgPanel.Height.Set(500, 0);
            bgPanel.SetPadding(0);
            bgPanel.VAlign = 0.5f;
            bgPanel.Left.Set(30f, 0);

            StandCardGrid = new UIGrid();
            StandCardGrid.Width.Set(580, 0);
            StandCardGrid.Height.Set(450, 0);
            StandCardGrid.SetScrollbar(SCGridScrollBar);
            StandCardGrid.Left.Set(5, 0);
            StandCardGrid.Top.Set(5, 0);

            bgPanel.Append(StandCardGrid);

            MainPanel.Append(bgPanel);
            MainPanel.Append(SCGridScrollBar);

            base.Append(MainPanel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(!HasLoaded && StandLoader.Instance.Loaded)
            {
                HasLoaded = true;
                foreach (var stand in StandLoader.Instance.GenericEnumerable)
                {
                    int projToKill = Projectile.NewProjectile(Vector2.Zero, Vector2.Zero, TBAMod.Instance.ProjectileType(stand.GetType().Name), 0, 0, Main.myPlayer);

                    StandCard standCard = new StandCard(Main.projectile[projToKill].modProjectile as Stand);

                    standCard.Left.Set(5, 0);

                    StandCardGrid.Add(standCard);

                    Main.projectile[projToKill].Kill();
                }
            }

            Recalculate();
        }

        public bool HasLoaded { get; private set; }

        public UIGrid StandCardGrid { get; private set; }
        public UIScrollbar SCGridScrollBar {get; private set; }

        public UIPanel MainPanel { get; private set; }
    }
}
