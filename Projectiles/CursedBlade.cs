using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Projectiles
{
	public class CursedBlade : ModProjectile
	{
        // slow projectile, moves towards mouse and spins
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Blade");
        }

        public override void SetDefaults()
        {
            projectile.magic = true;
            projectile.width = 30;
			projectile.height = 40;
			projectile.friendly = true;
            projectile.netImportant = true;
			projectile.penetrate = 3;
			projectile.timeLeft = 600;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).necrotic = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).melee = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).cursedfire = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).lifeSteal = 2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void AI()
		{
            projectile.netUpdate = true;
            if (projectile.owner == Main.myPlayer)
            {
                Vector2 toMouse = Main.MouseWorld - projectile.Center;
                toMouse.Normalize();
                projectile.velocity = toMouse * 1f;
            }
            projectile.rotation += 0.5f * projectile.ai[1];
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override bool PreAI()
        {
            Lighting.AddLight(projectile.position, 0f, .6f, 0f);
            return true;
        }
    }
}