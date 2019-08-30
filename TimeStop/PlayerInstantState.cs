using Terraria;

namespace TerrarianBizzareAdventure.TimeStop
{
    public class PlayerInstantState : EntityState
    {
        public static PlayerInstantState FromPlayer(int playerId, Player player) =>
            new PlayerInstantState()
            {
                EntityId = playerId,
                Player = player,

                Position = player.position,
                Velocity = player.velocity,

                Damage = 0,

                Life = player.statLife,
                Mana = player.statMana,
            };


        public override void Restore()
        {
            Player.velocity = Velocity;

            Player.statMana = Mana;
        }


        public Player Player { get; set; }

        public int Life { get; set; }
        public int Mana { get; set; }
    }
}