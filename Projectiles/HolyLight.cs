using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Projectiles
{
	public class HolyLight : ModProjectile
	{
        // basic projectile
        // rained down from above screen to mouse
        // high damage to vampires
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
            projectile.magic = true;
            projectile.width = 16;
			projectile.height = 16;
			projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.timeLeft = 50;
            projectile.netImportant = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).necrotic = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).magic = true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.type == NPCID.Vampire || target.type == NPCID.VampireBat)
            {
                damage *= 200;
            }
        }

        public override void AI()
		{
	        Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 1;
        }

        public override void Kill(int timeLeft)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			}
		}
    }
}