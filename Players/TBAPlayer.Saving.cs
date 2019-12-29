﻿using Terraria.ModLoader.IO;
using TerrarianBizzareAdventure.Stands;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer
    {
        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();

            if (StandUser)
                tag.Add(nameof(Stand), Stand.UnlocalizedName);

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            Stand = null;
            ActiveStandProjectileId = -999;

            if (tag.ContainsKey(nameof(Stand)))
                Stand = StandManager.Instance[tag.GetString(nameof(Stand))];
        }
    }
}