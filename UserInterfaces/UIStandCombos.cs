using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using TerrarianBizzareAdventure.Enums;
using TerrarianBizzareAdventure.Players;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.UserInterfaces.Elements;
using TerrarianBizzareAdventure.UserInterfaces.Elements.StandCollection;

namespace TerrarianBizzareAdventure.UserInterfaces
{
    public class UIStandCombos : TBAUIState
    {
        private const float
            PANEL_WIDTH = 630,
            PANEL_HEIGHT = 600;

        public override void OnInitialize()
        {
            CurrentCombos = new Dictionary<string, StandCombo>();

            MainPanel = new UIPanel();
            MainPanel.Width.Set(PANEL_WIDTH, 0);
            MainPanel.Height.Set(PANEL_HEIGHT, 0);
            MainPanel.SetPadding(0);
            MainPanel.VAlign = 0.5f;
            MainPanel.HAlign = 0.5f;

            UIText bottomText = new UIText("Stand Moves", 1, true);
            bottomText.VAlign = 0.05f;
            bottomText.HAlign = 0.5f;
            MainPanel.Append(bottomText);

            SCGridScrollBar = new UIScrollbar();
            SCGridScrollBar.Width.Set(20, 0);
            SCGridScrollBar.Height.Set(350, 0);
            SCGridScrollBar.VAlign = 0.5f;
            SCGridScrollBar.Left.Set(5, 0);

            var bgPanel = new UIPanel();
            bgPanel.Width.Set(585, 0);
            bgPanel.Height.Set(466, 0);
            bgPanel.SetPadding(0);
            bgPanel.VAlign = 0.5f;
            bgPanel.Left.Set(30f, 0);

            StandComboGrid = new UIGrid();
            StandComboGrid.Width.Set(580, 0);
            StandComboGrid.Height.Set(450, 0);
            StandComboGrid.SetScrollbar(SCGridScrollBar);
            StandComboGrid.Left.Set(5, 0);
            StandComboGrid.Top.Set(5, 0);

            bgPanel.Append(StandComboGrid);

            MainPanel.Append(bgPanel);
            MainPanel.Append(SCGridScrollBar);

            base.Append(MainPanel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.gameMenu && (Main.LocalPlayer.controlInv || TBAInputs.OpenCollection.JustPressed))
                Visible = false;

            if(TBAInputs.StandPose.GetAssignedKeys().Count <= 0
                || TBAInputs.ContextAction.GetAssignedKeys().Count <= 0
                || TBAInputs.ExtraAction01.GetAssignedKeys().Count <= 0
                || TBAInputs.ExtraAction02.GetAssignedKeys().Count <= 0)
            {
                Main.NewText("Whoops! It looks like you forgot to setup your hotkeys! Go to Settings -> Controls and scroll down. Bind all hotkeys from this mod & try again");
                Visible = false;
                return;
            }

            if (NeedsToUpdateAutopsyReport && CurrentStand != null)
            {
				NeedsToUpdateAutopsyReport = false;
				
                StandComboGrid.Clear();

                CurrentCombos.Clear();
                foreach(var kvp in CurrentStand.Combos)
                {
                    CurrentCombos.Add(kvp.Key, kvp.Value);
                }

                foreach (var combo in CurrentCombos)
                {
                    if (combo.Value.Inputs.Count <= 0)
                        continue;

                    List<UIElement> inputElements = new List<UIElement>();

                    foreach(string s in combo.Value.Inputs)
                    {
                        if (s == MouseClick.LeftClick.ToString())
                            inputElements.Add(new UIMouseClick((int)MouseClick.LeftClick, 10));

                        if (s == MouseClick.RightClick.ToString())
                            inputElements.Add(new UIMouseClick((int)MouseClick.RightClick, 10));

                        if (s == MouseClick.MiddleClick.ToString())
                            inputElements.Add(new UIMouseClick((int)MouseClick.MiddleClick, 10));

                        if (s == MouseClick.LeftHold.ToString())
                            inputElements.Add(new UIMouseClick((int)MouseClick.LeftClick, 120));

                        if (s == MouseClick.RightHold.ToString())
                            inputElements.Add(new UIMouseClick((int)MouseClick.RightClick, 120));

                        if (s.Length == 1)
                            inputElements.Add(new UIButtonPress(s));
                    }

                    ComboPanel comboPanel = new ComboPanel(combo.Key, inputElements);

                    StandComboGrid.Add(comboPanel);


                }
            }

            RecalculateChildren();
            Recalculate();
        }

        public bool HasLoaded { get; private set; }

        private Stand _currentStand;
        public Stand CurrentStand
        {
            get => _currentStand;
            set
            {
                LastStand = _currentStand;

                _currentStand = value;
            }
        }

        public Stand LastStand { get; set; }

        public UIGrid StandComboGrid { get; private set; }
        public UIScrollbar SCGridScrollBar { get; private set; }

        public UIPanel MainPanel { get; private set; }
		
		public bool NeedsToUpdateAutopsyReport { get; set; }

        public Dictionary<string, StandCombo> CurrentCombos { get; private set; }
    }
}
