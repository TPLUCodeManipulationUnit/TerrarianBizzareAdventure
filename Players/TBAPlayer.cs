﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Network;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.TimeStop;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public const int ACTIVE_STAND_PROJECTILE_INACTIVE_ID = -999;


        public static TBAPlayer Get(Player player) => player.GetModPlayer<TBAPlayer>();


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

            OnPostUpdate?.Invoke(this);
        }

        public override void ResetEffects()
        {
            if (AttackDirection != 0)
            {
                player.velocity.X *= 0.2f;
                player.velocity.Y *= 0.01f;
                player.direction = AttackDirection;
            }
        }

        public override void OnEnterWorld(Player player)
        {
            TimeStopManagement.OnPlayerEnterWorld(player);

            if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                NetworkPacketManager.Instance.PlayerJoiningSynchronization.SendPacketToAllClients(player.whoAmI, player.whoAmI, Stand == null ? "" : Stand.UnlocalizedName, false);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!StandUser) return;

            if (TBAInputs.SummonStand.JustPressed)
            {
                if (ActiveStandProjectileId == -999) // Minimal value for a DAT in SHENZEN.IO :haha:
                    ActiveStandProjectileId = Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType(Stand.GetType().Name), 0, 0, player.whoAmI);
            }

            if (ActiveStandProjectileId == ACTIVE_STAND_PROJECTILE_INACTIVE_ID)
                return;
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
        public bool IsImmuneToTimeStop() => StandUser && Stand.IsImmuneToTimeStop(this);


        public Stand Stand { get; set; }
        public int ActiveStandProjectileId { get; set; }


        public bool StandUser => Stand != null;


        public int AttackDirection { get; set; }

        public int AttackDirectionResetTimer { get; set; }

        public Vector2 PreTimeStopPosition { get; private set; }
    }
}
