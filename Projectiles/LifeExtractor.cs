using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Necromancy.Projectiles
{
	public class LifeExtractor : ModProjectile
	{
        // spear projectile, lots of healing
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Extractor");
        }

        public override void SetDefaults()
        {
            projectile.magic = true;
            projectile.scale = 1.3f;
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 19;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.ownerHitCheck = true; //so you can't hit enemies through walls
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).necrotic = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).radiant = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).healPower = 40;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 4;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).healPower = Math.Max(0, projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).healPower - 10);
        }

        public float MovementFactor // Change this value to alter how fast the spear moves
		{
			get { return projectile.ai[0]; }
			set { projectile.ai[0] = value; }
		}
		// It appears that for this AI, only the ai0 field is used!

		public override void AI()
		{
			// Since we access the owner player instance so much, it's useful to create a helper local variable for this
			// Sadly, Projectile/ModProjectile does not have its own
			Player projOwner = Main.player[projectile.owner];
			// Here we set some of the projectile's owner properties, such as held item and itemtime, along with projectile directio and position based on the player
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			projectile.direction = projOwner.direction;
			projectile.spriteDirection = -projectile.direction;
			projOwner.heldProj = projectile.whoAmI;
			projOwner.itemTime = projOwner.itemAnimation;
			projectile.position.X = ownerMountedCenter.X - (projectile.width / 2);
			projectile.position.Y = ownerMountedCenter.Y - (projectile.height / 2);
			// As long as the player isn't frozen, the spear can move
			if (!projOwner.frozen)
			{
				if (MovementFactor == 0f) // When intially thrown out, the ai0 will be 0f
				{
					MovementFactor = 3f; // Make sure the spear moves forward when initially thrown out
					projectile.netUpdate = true; // Make sure to netUpdate this spear
				}
				if (projOwner.itemAnimation < projOwner.itemAnimationMax / 3) // Somewhere along the item animation, make sure the spear moves back
				{
					MovementFactor -= 2.4f;
				}
				else // Otherwise, increase the movement factor
				{
					MovementFactor += 2.1f;
				}
			}
			// Change the spear position based off of the velocity and the movementFactor
			projectile.position += projectile.velocity * MovementFactor;
			// When we reach the end of the animation, we can kill the spear projectile
			if (projOwner.itemAnimation == 0)
			{
				projectile.Kill();
			}
			// Apply proper rotation, with an offset of 135 degrees due to the sprite's rotation, notice the usage of MathHelper, use this class!
			// MathHelper.ToRadians(xx degrees here)
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.ToRadians(135f);
			// Offset by 90 degrees here
			if (projectile.spriteDirection == -1)
			{
				projectile.rotation -= MathHelper.ToRadians(90f);
			}
		}
	}
}
