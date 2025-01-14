using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Necromancy.NPCs;

namespace Necromancy.Projectiles
{
	public class GreedRing : ModProjectile
	{
        // creates a slowly shrinking ring around the player
        // moves very fast in a shrinking circle
        // is supposed to knock enemies that are inside the circle towards the player
        // and knock enemies that are outside away
        // but it doesn't work too well
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greed Ring");
        }

        public override void SetDefaults()
        {
            projectile.magic = true;
            projectile.width = 16;
			projectile.height = 16;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
			projectile.timeLeft = 2;
            projectile.hide = true;
            projectile.extraUpdates = 90;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).necrotic = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).magic = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if ((target.GetGlobalNPC<NecromancyNPC>().oldCenter - Main.player[projectile.owner].Center).LengthSquared() < projectile.ai[0] * projectile.ai[0])
            {
                Necromancy.DoCustomKnockback(target, (target.Center - Main.player[projectile.owner].Center) / 12f);
            }
            else
            {
                Necromancy.DoCustomKnockback(target, -(target.Center - Main.player[projectile.owner].Center) / 12f);
            }
            target.immune[projectile.owner] = 5;
        }

        public override void AI()
		{
            float distSq = (Main.player[projectile.owner].Center - projectile.Center).LengthSquared();
            projectile.timeLeft = 2;
            if (distSq < 1f)
            {
                projectile.Kill();
            }

            Vector2 newPos = Vector2.UnitX * projectile.ai[0];
            newPos = Main.player[projectile.owner].Center + newPos.RotatedBy(MathHelper.ToRadians(projectile.ai[1]));
            projectile.velocity = newPos - projectile.Center;

            projectile.ai[0] -= 1 / 360f;
            projectile.ai[1] += 1;
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(projectile.Center, 63);
                d.noGravity = true;
                d.velocity += Main.player[projectile.owner].dead ? Vector2.Zero : Main.player[projectile.owner].velocity;
            }
        }
    }
}