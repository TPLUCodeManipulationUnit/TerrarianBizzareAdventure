using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands
{
    public class StandCombo
    {
        /// <summary>
        /// First index is used for Display Name
        /// </summary>
        /// <param name="inputs"></param>
        public StandCombo(params string[] inputs)
        {
            DisplayName = inputs[0];

            Inputs = new List<string>();

            if (inputs.Length <= 1 || inputs == null)
                return;

            for (int i = 1; i < inputs.Length; i++)
                Inputs.Add(inputs[i]);
        }

        public bool CheckCombo(TBAPlayer player, bool force = false)
        {
            if (force)
                return true;

            int currentMatches = 0;

            if (player.IsComboCheckDelayed)
                return false;

            if (player.Inputs.Count < Inputs.Count)
                return false;

            for (int i = player.Inputs.Count - RequiredMatches - 1; i < player.Inputs.Count; i++)
            {
                if (player.Inputs[i].Key.ToString() == Inputs[currentMatches])
                    currentMatches++;
                else
                    return false;
            }

            if (currentMatches >= RequiredMatches)
            {
                DrawHelpers.CircleDust(player.player.Center, Vector2.Zero, 6, 8, 8, 1.85f);
                player.Inputs.Clear();
            }

            return currentMatches >= RequiredMatches;
        }

        public List<string> Inputs { get; }

        public string DisplayName { get; set; }

        public int RequiredMatches => Inputs.Count - 1;
    }
}
