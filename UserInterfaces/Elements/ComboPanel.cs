using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TerrarianBizzareAdventure.UserInterfaces.Elements
{
    /// <summary>
    /// Used to display how to use certain abilities with stands
    /// </summary>
    public class ComboPanel : UIPanel
    {
        public ComboPanel(string comboName, List<UIElement> inputs)
        {
            Width.Set(574, 0);
            Height.Set(128, 0);

            UIPanel tAPTFTT = new UIPanel(); // short for tinyAssPanelToFitTheText
            tAPTFTT.Width.Set(574, 0);
            tAPTFTT.Height.Set(32, 0);

            UIText bottomText = new UIText(comboName);
            bottomText.VAlign = 0.5f;
            bottomText.HAlign = 0.5f;
            tAPTFTT.Append(bottomText);

            if(inputs.Count > 0)
                foreach(UIElement e in inputs)
                {
                    e.Left.Set(2 + 54 * inputs.FindIndex(x => x == e), 0);
                    if(e is UIButtonPress)
                    e.Top.Set(46, 0);
                    else
                        e.Top.Set(34, 0);
                    this.Append(e);
                }

            this.Append(tAPTFTT);
        }
    }
}
