using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Projectiles
{
    public class Splitdagger : ModProjectile
    {
        // moves a set distance, then splits into 7 SplitdaggerBlade projectiles
        // if it hits an enemy it splits but shoots them away, doing less damage

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Splitdagger");
        }

        public override void SetDefaults()
        {
            projectile.magic = true;
            projectile.width = 26;
            projectile.height = 26;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 15;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.penetrate = 1;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).necrotic = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).throwing = true;
        }

        public override void AI()
        {
            Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 14, projectile.velocity.X, projectile.velocity.Y);
            d.scale = 0.8f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Vector2 bounceFrom = new Vector2(projectile.ai[0], projectile.ai[1]);
            if (bounceFrom != Vector2.Zero)
            {
                projectile.velocity = projectile.Center - bounceFrom;
                projectile.velocity.Normalize();
                projectile.velocity *= projectile.oldVelocity.Length();
            }
            float rotation = MathHelper.ToRadians(7);
            for (int i = -3; i < 4; i++)
            {
                Vector2 perturbedSpeed = projectile.velocity.RotatedBy(rotation * i);
                Projectile.NewProjectile(projectile.position + perturbedSpeed, perturbedSpeed, mod.ProjectileType<SplitdaggerBlade>(), projectile.damage, projectile.knockBack / 7f, projectile.owner);
            }
            Main.PlaySound(SoundID.Item40, projectile.position);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.velocity *= -1f;
        }
    }
}
