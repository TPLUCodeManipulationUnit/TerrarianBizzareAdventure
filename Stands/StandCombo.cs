using System.Collections.Generic;

namespace TerrarianBizzareAdventure.Stands
{
    public class StandCombo
    {
        public StandCombo(string name, params string[] inputs)
        {
            ComboName = name;

            Inputs = new List<string>();

            if (inputs.Length <= 0 || inputs == null)
                return;

            for (int i = 0; i < inputs.Length; i++)
                Inputs.Add(inputs[i]);
        }

        public string ComboName { get; }

        public List<string> Inputs { get; }
    }
}
