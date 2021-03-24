using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TerrarianBizzareAdventure.Buffs;
using TerrarianBizzareAdventure.Helpers;
using TerrarianBizzareAdventure.Items.Weapons.Rewards;
using TerrarianBizzareAdventure.Networking;
using TerrarianBizzareAdventure.Projectiles.Misc;
using TerrarianBizzareAdventure.ScreenModifiers;
using TerrarianBizzareAdventure.Stands;
using TerrarianBizzareAdventure.Stands.Special.Developer.Webmilio;
using TerrarianBizzareAdventure.Stands.StardustCrusaders.TheWorld;
using TerrarianBizzareAdventure.TimeStop;
using TerrarianBizzareAdventure.UserInterfaces;
using WebmilioCommons.Extensions;

namespace TerrarianBizzareAdventure.Players
{
    public sealed partial class TBAPlayer : ModPlayer
    {
        public const int ACTIVE_STAND_PROJECTILE_INACTIVE_ID = -999;


        public static TBAPlayer Get() => Get(Main.LocalPlayer);
        public static TBAPlayer Get(Player player) => player.GetModPlayer<TBAPlayer>();


        private Stand _activeStandProjectile;


        public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
        {
            Item stiletto = new Item();

            stiletto.SetDefaults(ModContent.ItemType<RewardStiletto>());

            if (KnifeGangMember)
                items.Add(stiletto);
        }

        public override void Initialize()
        {
            _activeStandProjectile = default;

            //AuraAnimationKey = (int)AuraAnimationType.None;

            MaxStamina = 100;
            Stamina = MaxStamina;

            UnlockedStands = new List<string>();
        }


        public override void PreUpdate()
        {
            /*bool isStandReal = ActiveStandProjectileId != ACTIVE_STAND_PROJECTILE_INACTIVE_ID && Main.projectile[ActiveStandProjectileId].modProjectile is Stand;
            if (!isStandReal)
                ActiveStandProjectileId = ACTIVE_STAND_PROJECTILE_INACTIVE_ID;*/

            if(TimeStopManagement.TimeStopper != null && TimeStopManagement.TimeStopper.player.whoAmI == player.whoAmI && !TimeStopManagement.TimeStopped)
                player.AddBuff<TimeStopCDDebuff>(6 * Constants.TICKS_PER_SECOND);

            StaminaGainBonus = 0;

            if (player.wellFed)
                StaminaGainBonus += 1;

            if (player.statLife >= player.statLifeMax2)
                StaminaGainBonus += 1;
        }

        public override void PostUpdate()
        {
            OnPostUpdate?.Invoke(this);
        }

        public override void ResetEffects()
        {
            ResetInputs();
            ResetDrawingEffects();
            ResetBuffEffects();
            ResetStaminaEffects();
            ResetMouse();

            if (IsCombatLocked)
                CombatLockTimer--;

            if (IsCombatLocked)
            {
                player.position = player.oldPosition;
                player.velocity = player.oldVelocity;

                if (CombatLockTimer <= 1)
                {
                    player.velocity = StoredVelocity;
                    StoredVelocity = Vector2.Zero;
                }
            }

            if (AttackDirection != 0)
            {
                player.velocity.X *= 0.2f;
                player.velocity.Y *= 0.01f;
                player.direction = AttackDirection;
            }

            CanTransform = false;

            if (Stand is TheWorldStand)
                player.noFallDmg = true;

            if (AttackDirectionResetTimer > 0)
                AttackDirectionResetTimer--;
            else
                AttackDirection = 0;
        }

        public void ResetMouse()
        {
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
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            new PlayerJoiningSynchronizationPacket(this).Send(fromWho, toWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            TBAPlayer tPlayer = clientPlayer as TBAPlayer;

            if (tPlayer.Stand != Stand)
                new PlayerJoiningSynchronizationPacket(this).Send();

            if (tPlayer.ActiveStandProjectile != ActiveStandProjectile)
                new ActiveStandChangedPacket().Send();

            if (tPlayer.AuraAnimationKey != AuraAnimationKey)
                new AuraSyncPacket().Send();
        }

        public override void UpdateBiomeVisuals()
        {
            bool shockWaveExist = Main.projectile.Count(x => x.active && x.modProjectile is TimeStopVFX) > 0;
            player.ManageSpecialBiomeVisuals("TBA:FreezeSky", TimeStopManagement.TimeStoppedFor > 0 && !shockWaveExist);
            player.ManageSpecialBiomeVisuals("TBA:TimeStopInvert", shockWaveExist);

            GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(ItemID.TwilightDye), player);
        }

        public override void PlayerConnect(Player player)
        {
        }

        public override void OnEnterWorld(Player player)
        {
            TimeStopManagement.OnPlayerEnterWorld(player);

            //string text = VoiceRecognitionSystem.SuccesfulBoot ? "Successfully booted up Voice Recognition System;" : "Unsuccessful boot attempt at Voice Recognition System;";
            //Main.NewText(text + VoiceRecognitionSystem.FailReason, VoiceRecognitionSystem.SuccesfulBoot ? Color.Lime : Color.Red);

            ActiveStandProjectile = null;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TBAInputs.ContextAction.JustPressed)
            {
                Main.NewText("Screen Modifier Test");
                ShakeScreenModifier screenShake = new ShakeScreenModifier(Main.MouseWorld, new Vector2(30), 300, 4, 0.25f);
                ScreenModifiers.Add(new SmoothStepScreenModifier(player.Center, Main.MouseWorld, 0.15f));
                ScreenModifiers.Add(screenShake);
                ScreenModifiers.Add(new PlayerChaseScreenModifier(Main.MouseWorld, player.Center, 0.1f));
            }

            if (!StandUser) return;

            ProcessComboTriggers();

            //ProcessVoiceControls();

            if (TBAInputs.SummonStand.JustPressed && !Exhausted)
            {
                GetEligibleKeys();
                if (ActiveStandProjectile == null) // Minimal value for a DAT in SHENZEN.IO :haha:
                {
                    ActiveStandProjectile = Main.projectile[Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType(Stand.GetType().Name), 0, 0, player.whoAmI)].modProjectile as Stand;

                    if (Stand.CallSoundPath != "")
                       TBAMod.PlayVoiceLine(Stand.CallSoundPath);

                    Stand.Combos.Clear();
                    Stand.AddCombos();
                }
            }

            if (TBAInputs.OpenCollection.JustPressed)
                UIManager.SCLayer.State.Visible = !UIManager.SCLayer.State.Visible;
        }

        public override void PlayerDisconnect(Player player)
        {
            if (player != Main.LocalPlayer)
                return;

            new CompileAssemblyPacket().playerInstantEnvironments.Remove(player.whoAmI);
        }

        public override void SetControls()
        {
            if (ActiveStandProjectile is PunchBarragingStand stando && stando.RushTimer > 0)
            {
                BlockInputs();
            }

            if (AttackDirection != 0)
            {
                player.controlLeft = AttackDirection != 1;
                player.controlRight = AttackDirection != -1;
            }

            SetAerosmithControls();
        }

        private void BlockInputs(bool blockDirections = true, bool blockJumps = true, bool blockItemUse = true, bool blockOther = true)
        {
            if (blockDirections)
            {
                player.controlDown = false;
                player.controlUp = false;
                player.controlLeft = false;
                player.controlRight = false;
            }

            if (blockJumps)
                player.controlJump = false;

            if (blockItemUse)
            {
                player.controlUseItem = false;
                player.controlUseTile = false;
            }

            if (blockOther)
            {
                player.controlThrow = false;
                player.controlMount = false;
                player.controlHook = false;
            }
        }

        public void KillStand() => ActiveStandProjectile = null;


        public Stand Stand { get; set; }

        public Stand ActiveStandProjectile
        {
            get => _activeStandProjectile;
            set
            {
                if (_activeStandProjectile != default && value != default && _activeStandProjectile.projectile.whoAmI == value.projectile.whoAmI)
                    return;

                _activeStandProjectile = value;

                this.SendIfLocal<ActiveStandChangedPacket>();
            }
        }


        public bool StandUser => Stand != null;

        public bool StandActive => ActiveStandProjectile != null;

        private int _attackDirection;
        public int AttackDirection
        {
            get => _attackDirection;
            set
            {
                if (_attackDirection == value)
                    return;

                _attackDirection = value;

                this.SendIfLocal<AttackDirectionPacket>();
            }
        }

        private int _attackResetTimer;
        public int AttackDirectionResetTimer
        {
            get => _attackResetTimer;
            set
            {
                _attackResetTimer = value;
                if (_attackResetTimer >= 180)
                    this.SendIfLocal<AttackTimerPacket>();
            }
        }


        // Used for stuff that happens when you hold LMB for some time.
        public int MouseOneTime { get; private set; }

        public int MouseOneTimeReset { get; private set; }

        public int MouseTwoTime { get; private set; }

        public int MouseTwoTimeReset { get; private set; }

        public bool IsCombatLocked => CombatLockTimer > 0;
        public int CombatLockTimer { get; set; }
        public Vector2 StoredVelocity { get; set; }

        public bool KnifeGangMember => SteamHelper.KnifeGangMembers.Count(x => x.SteamId64 == SteamHelper.SteamId64) > 0;

        public List<string> UnlockedStands { get; private set; }
    }
}
