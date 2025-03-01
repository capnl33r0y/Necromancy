using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Projectiles.Minions
{
    // this is a raincloud that moves above enemies to rain on them
    // rain pierces and moves straight down
    public class RainCloudSummon : ModProjectile
    {
        private float idleAccel = 0.05f;
        private float spacingMult = 1.2f;
        private float viewDist = 400f;
        private float chaseDist = 10f;
        private float chaseAccel = 12f;
        private float inertia = 40f;
        private float shootCool = 8f;
        private float shootSpeed;
        private float height = 150f;

        public override void SetDefaults()
        {
            projectile.magic = true;
            projectile.netImportant = true;
            projectile.width = 60;
            projectile.height = 40;
            Main.projFrames[projectile.type] = 6;
            projectile.penetrate = -1;
            projectile.timeLeft = 3600;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            inertia = 20f;
            shootSpeed = 10f;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).necrotic = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).summon = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).summonCost = 15;
        }

        public override void AI()
        {
            CheckActive();
            Behavior();
        }

        public void SelectFrame()
		{
			projectile.frameCounter++;
			if (projectile.frameCounter >= 5)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % 6;
			}
        }

        public void CreateDust()
        {
            Lighting.AddLight((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f), 0.6f, 0f, 0.6f);
        }

        public void CheckActive()
        {
            Player player = Main.player[projectile.owner];
            if (!player.dead)
            {
                projectile.timeLeft = 2;
            }
        }

        public void Behavior()
        {
            Player player = Main.player[projectile.owner];
            float spacing = (float)projectile.width * spacingMult;
            for (int k = 0; k < 1000; k++)
            {
                Projectile otherProj = Main.projectile[k];
                if (k != projectile.whoAmI && otherProj.active && otherProj.owner == projectile.owner && otherProj.type == projectile.type && System.Math.Abs(projectile.position.X - otherProj.position.X) + System.Math.Abs(projectile.position.Y - otherProj.position.Y) < spacing)
                { // spacing with other of same minion
                    if (projectile.position.X < Main.projectile[k].position.X)
                    {
                        projectile.velocity.X -= idleAccel;
                    }
                    else
                    {
                        projectile.velocity.X += idleAccel;
                    }
                    if (projectile.position.Y < Main.projectile[k].position.Y)
                    {
                        projectile.velocity.Y -= idleAccel;
                    }
                    else
                    {
                        projectile.velocity.Y += idleAccel;
                    }
                }
            }
            Vector2 targetPos = projectile.position + new Vector2(0, -height);
            Vector2 targetPosAbove = projectile.position;
            float targetDistSq = viewDist * viewDist;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                NPC npc = Main.npc[k];
                if (npc.CanBeChasedBy(this, false))
                {
                    // aim above target
                    Vector2 npcAbove = npc.Center + new Vector2(0, -height);
                    float distanceSq = Vector2.DistanceSquared(npcAbove, projectile.Center);
                    if ((distanceSq < targetDistSq || !target) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npcAbove, npc.width, npc.height) && Collision.CanHitLine(npcAbove, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                    {
                        targetDistSq = distanceSq;
                        // lead shots
                        targetPosAbove = npcAbove + Vector2.UnitX * npc.direction * 4f;
                        targetPos = npc.Center;
                        target = true;
                    }
                }
            }
            if (Vector2.DistanceSquared(player.Center, projectile.Center) > (target ? 1000f * 1000f : 500f * 500f))
            { // go back to player
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 1f)
            { // if returning to player
                projectile.tileCollide = false;
            }
            if (target && projectile.ai[0] == 0f)
            { // if going towards a target
                Vector2 direction = targetPosAbove - projectile.Center;
                if (direction.LengthSquared() > chaseDist * chaseDist)
                { // if too far
                    direction.Normalize();
                    projectile.velocity = (projectile.velocity * inertia + direction * chaseAccel) / (inertia + 1);
                }
                else
                { // if close enough
                    projectile.velocity *= (float)Math.Pow(0.97, 40.0 / inertia);
                }
            }
            else
            { // return to player
                if (!Collision.CanHitLine(projectile.Center, 1, 1, player.Center, 1, 1))
                {
                    projectile.ai[0] = 1f;
                }
                float speed = 6f;
                if (projectile.ai[0] == 1f)
                {
                    speed = 15f;
                }
                Vector2 center = projectile.Center;
                Vector2 direction = player.Center - center;
                projectile.ai[1] = 3600f;
                projectile.netUpdate = true;
                int num = 1;
                for (int k = 0; k < projectile.whoAmI; k++)
                {
                    if (Main.projectile[k].active && Main.projectile[k].owner == projectile.owner && Main.projectile[k].type == projectile.type)
                    {
                        num++;
                    }
                }
                direction.X -= (float)((10 + num * 40) * player.direction);
                direction.Y -= 70f;
                float distanceToSq = direction.LengthSquared();
                if (distanceToSq > 200f * 200f && speed < 9f)
                {
                    speed = 9f;
                }
                if (distanceToSq < 100f * 100f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (distanceToSq > 2000f* 2000f)
                {
                    projectile.Center = player.Center;
                }
                if (distanceToSq > 48f * 48f)
                {
                    direction.Normalize();
                    direction *= speed;
                    float temp = inertia / 2f;
                    projectile.velocity = (projectile.velocity * temp + direction) / (temp + 1);
                }
                else
                {
                    projectile.direction = Main.player[projectile.owner].direction;
                    projectile.velocity *= (float)Math.Pow(0.9, 40.0 / inertia);
                }
            }
            projectile.rotation = 0;
            
            SelectFrame();
            CreateDust();

        
            if (projectile.velocity.X > 0f)
            { // face direction
                projectile.spriteDirection = (projectile.direction = -1);
            }
            else if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = (projectile.direction = 1);
            }
            if (projectile.ai[1] > 0f)
            { // shoot cooldown
                projectile.ai[1] += 1f;
                if (Main.rand.Next(3) == 0)
                {
                    projectile.ai[1] += 1f;
                }
            }
            if (projectile.ai[1] > shootCool)
            { // cooldown done
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            { //not returning to player
                if (target)
                { // has target
                    if (projectile.ai[1] == 0f)
                    { // shoot ready
                        projectile.ai[1] = 1f;
                        if (Main.myPlayer == projectile.owner)
                        {
                            Vector2 shootVel = Vector2.UnitY * shootSpeed;

                            Vector2 pos = new Vector2(Main.rand.NextFloat(projectile.position.X + projectile.width * 1 / 4f, projectile.position.X + projectile.width * 3 / 4f), projectile.Center.Y);
                            
                            Dust.NewDust(pos, 2, 2, 242, shootVel.X, shootVel.Y);

                            int proj = Projectile.NewProjectile(pos.X, pos.Y, shootVel.X, shootVel.Y, mod.ProjectileType("SummonRain"), projectile.damage, projectile.knockBack, Main.myPlayer, 0f, 0f);


                            Main.projectile[proj].GetGlobalProjectile<NecromancyGlobalProjectile>(mod).shotFrom = projectile.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).shotFrom;
                        }
                    }
                }
            }
        }
    }
}
 