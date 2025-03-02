using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Projectiles
{
	public class YellowBlade : ModProjectile
	{
        // spinny boomerang projectile

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yellow Blade");
        }

        public override void SetDefaults()
        {
            projectile.magic = true;
            projectile.width = 42;
			projectile.height = 58;
			projectile.friendly = true;
			projectile.penetrate = 30;
			projectile.timeLeft = 120;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).necrotic = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).magic = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).ichor = true;
        }

		public override void AI()
		{
            projectile.rotation += MathHelper.ToRadians(30);
            if (projectile.ai[0] == 1f)
            {
                Vector2 toPlayer = Main.player[projectile.owner].Center - projectile.Center;
                if (toPlayer.LengthSquared() < 32f * 32f)
                {
                    projectile.Kill();
                }
                toPlayer.Normalize();
                projectile.velocity = projectile.velocity * 0.95f + toPlayer;
            }
            else
            {
                if (projectile.timeLeft < 105)
                {
                    projectile.ai[0] = 1f;
                }
            }
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.ai[0] = 1f;
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            Main.PlaySound(SoundID.Item10, projectile.position);
            
            return false;
        }

        public override bool PreAI()
        {
            Lighting.AddLight(projectile.position, .6f, .6f, 0f);
            return true;
        }
    }
}