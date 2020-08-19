﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Players;

namespace TerrarianBizzareAdventure.Stands
{
    public class StandCombo
    {
        public StandCombo(params string[] inputs)
        {
            Inputs = new List<string>();

            if (inputs.Length <= 0 || inputs == null)
                return;

            for (int i = 0; i < inputs.Length; i++)
                Inputs.Add(inputs[i]);
        }

        public bool CheckCombo(TBAPlayer player)
        {
            int currentMatches = 0;

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

        public int RequiredMatches => Inputs.Count - 1;
    }
}
