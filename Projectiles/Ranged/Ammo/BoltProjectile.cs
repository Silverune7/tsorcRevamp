using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Projectiles.Ranged.Ammo
{
    public class BoltProjectile : ModProjectile
    {
        public override string Texture => "tsorcRevamp/Items/Ammo/Bolt";
        public override void SetDefaults()
        {
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.height = 10;
            Projectile.penetrate = 2;
            Projectile.tileCollide = true;
            Projectile.width = 5;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.aiStyle = 1;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        }
    }

}
