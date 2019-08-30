using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrarianBizzareAdventure.Extensions
{
    public static class EnumerableExtensions
    {
        public static string GenerateSlashedString(this List<float> values) => GenerateSlashedString(values.ToArray());

        public static string GenerateSlashedString(this float[] values)
        {
            StringBuilder sb = new StringBuilder();

            
            int matches = 0;

            matches = values.Count(x => x == values[0]);

            if (matches >= values.Length)
                return values[0].ToString();

            for (int i = 0; i < values.Length; i++)
            {
                sb.Append(values[i]);

                if (i + 1 < values.Length)
                    sb.Append('/');
            }

            return sb.ToString();
        }
    }
}