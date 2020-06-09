using System;
using System.Linq;
using Terraria.ModLoader.IO;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer
    {
        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();


            if (StandUser)
            {
                tag.Add(nameof(Stand), Stand.UnlocalizedName);
                tag.Add("UnlockedStands", UnlockedStands);
            }


            return tag;
        }

        public override void Load(TagCompound tag)
        {
            Stand = null;
            KillStand();

            if (tag.ContainsKey(nameof(Stand)))
                Stand = StandLoader.Instance.GetGeneric(tag.GetString(nameof(Stand)));


            UnlockedStands = tag.GetList<string>("UnlockedStands").ToList();
        }
    }
}
