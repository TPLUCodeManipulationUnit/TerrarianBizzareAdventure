using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio;
using TerrarianBizzareAdventure.TimeStop;
using System.Linq;
using TerrarianBizzareAdventure.Projectiles.Misc;
using TerrarianBizzareAdventure.TimeSkip;
using Terraria.Graphics.Shaders;
using System.Collections.Generic;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Items.Tools;
using TerrarianBizzareAdventure.Items.Weapons.Rewards;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public const int ACTIVE_STAND_PROJECTILE_INACTIVE_ID = -999;


        public static TBAPlayer Get() => Get(Main.LocalPlayer);
        public static TBAPlayer Get(Player player) => player.GetModPlayer<TBAPlayer>();


        public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
        {
            Item stiletto = new Item();
            stiletto.SetDefaults(ModContent.ItemType<RewardStiletto>());
            if (SteamHelper.TheSilverGhost == SteamHelper.SteamId64)
                items.Add(stiletto);
        }

        public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ModContent.ItemType<RewardStiletto>())
                return false;

            return base.CanSellItem(vendor, shopInventory, item);
        }

        public override void PreUpdate()
        {
            if (IsStopped())
            {
                if (!TimeStopManagement.playerStates.ContainsKey(player))
                    TimeStopManagement.RegisterStoppedPlayer(player);

                TimeStopManagement.playerStates[player].PreAI(player);
            }
        }

        public override void PostUpdate()
        {
            if (AttackDirectionResetTimer > 0)
                AttackDirectionResetTimer--;
            else
                AttackDirection = 0;

            if (AttackDirection != 0)
            {
                player.velocity.X *= 0.2f;
                player.velocity.Y *= 0.01f;
                player.direction = AttackDirection;
            }

            OnPostUpdate?.Invoke(this);
        }

        public override void ResetEffects()
        {
            ResetDrawingEffects();

            if (player.controlUseItem)
            {
                MouseOneTimeReset = 3;
                MouseOneTime++;
            }
            else
            {
                if (MouseOneTimeReset > 0)
                    MouseOneTimeReset--;
            }

            if (player.controlUseTile)
            {
                MouseTwoTimeReset = 3;
                MouseTwoTime++;
            }
            else
            {
                if (MouseTwoTimeReset > 0)
                    MouseTwoTimeReset--;
            }

            // I give 2 ticks to w.e. is using MouseTime to do its thing before it ultimately resets to 0
            if (MouseOneTimeReset <= 0)
                MouseOneTime = 0;


            if (MouseTwoTimeReset <= 0)
                MouseTwoTime = 0;

            CanTransform = false;
        }

        public override void UpdateBiomeVisuals()
        {
            bool shockWaveExist = Main.projectile.Count(x => x.active && x.modProjectile is TimeStopVFX) > 0;
            player.ManageSpecialBiomeVisuals("TBA:TimeStopInvert", shockWaveExist);

            GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(ItemID.TwilightDye), player);
        }

        public override void OnEnterWorld(Player player)
        {
            TimeStopManagement.OnPlayerEnterWorld(player);

            if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                new PlayerJoiningSynchronizationPacket().Send();
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!StandUser) return;

            if (TBAInputs.SummonStand.JustPressed)
            {
                if (ActiveStandProjectileId == -999) // Minimal value for a DAT in SHENZEN.IO :haha:
                {
                    ActiveStandProjectileId = Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType(Stand.GetType().Name), 0, 0, player.whoAmI);

                    if(Stand.CallSoundPath != "")
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, Stand.CallSoundPath));
                }
            }

            if (ActiveStandProjectileId == ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
                return;
        }

        public override void PlayerDisconnect(Player player)
        {
            if (player != Main.LocalPlayer)
                return;

            new CompileAssemblyPacket().playerInstantEnvironments.Remove(player.whoAmI);
        }

        public override void SetControls()
        {
            if (AttackDirection != 0)
            {
                player.controlLeft = AttackDirection != 1;
                player.controlRight = AttackDirection != -1;
            }
        }


        public bool IsStopped() => !IsImmuneToTimeStop() && TimeStopManagement.TimeStopped && TimeStopManagement.TimeStopper != this;
        public bool IsImmuneToTimeStop() => StandUser && Stand.IsNativelyImmuneToTimeStop(this);


        public Stand Stand { get; set; }
        public int ActiveStandProjectileId { get; set; }


        public bool StandUser => Stand != null;


        public int AttackDirection { get; set; }

        public int AttackDirectionResetTimer { get; set; }

        public Vector2 PreTimeStopPosition { get; private set; }

        // Used for stuff that happens when you hold LMB for some time.
        public int MouseOneTime { get; private set; }

        public int MouseOneTimeReset { get; private set; }

        public int MouseTwoTime { get; private set; }

        public int MouseTwoTimeReset { get; private set; }
    }
}
