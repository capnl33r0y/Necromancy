using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ID;

// from examplemod
namespace Necromancy.Projectiles
{
    public class Deathblast : ModProjectile
    {
        // sustained laser, copied from examplemod mostly
        // drains health while charging, lasts one second after charging
        private const int MAX_CHARGE = 60;
        private const float MOVE_DISTANCE = 48f;       //The distance charge particle from the player center

        public float Distance
        {
            get { return projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }

        public float Charge
        {
            get { return projectile.localAI[0]; }
            set { projectile.localAI[0] = value; }
        }

        private int count = 0;

        public override void SetDefaults()
        {
            projectile.magic = true;
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.GetGlobalProjectile<NecromancyGlobalProjectile>().necrotic = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Charge == MAX_CHARGE)
            {
                Vector2 unit = projectile.velocity;
                DrawLaser(spriteBatch, Main.projectileTexture[projectile.type],
                    Main.player[projectile.owner].Center, unit, 10, projectile.damage,
                    -1.57f, 1f, 1000f, Color.White, (int)MOVE_DISTANCE);
            }
            return false;

        }

        /// <summary>
        /// The core function of drawing a laser
        /// </summary>
        public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
        {
            Vector2 origin = start;
            float r = unit.ToRotation() + rotation;

            #region Draw laser body
            for (float i = transDist; i <= Distance; i += step)
            {
                Color c = Color.White;
                origin = start + i * unit;
                spriteBatch.Draw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 26, 28, 26), i < transDist ? Color.Transparent : c, r,
                    new Vector2(28 / 2, 26 / 2), scale, 0, 0);
            }
            #endregion

            #region Draw laser tail
            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 28, 26), Color.White, r, new Vector2(28 / 2, 26 / 2), scale, 0, 0);
            #endregion

            #region Draw laser head
            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 28, 26), Color.White, r, new Vector2(28 / 2, 26 / 2), scale, 0, 0);
            #endregion
        }

        /// <summary>
        /// Change the way of collision check of the projectile
        /// </summary>
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Charge == MAX_CHARGE)
            {
                Player p = Main.player[projectile.owner];
                Vector2 unit = projectile.velocity;
                float point = 0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), p.Center, p.Center + unit * Distance, 22, ref point))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Change the behavior after hit a NPC
        /// </summary>
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 2;
        }

        /// <summary>
        /// The AI of the projectile
        /// </summary>
        public override void AI()
        {
            count++;
            if (count > 120)
            {
                projectile.Kill();
            }

            Player owner = Main.player[projectile.owner];
            if (Charge != MAX_CHARGE)
            {
                Necromancy.BroadcastDrainLife(owner, 1);
            }
            if (!owner.channel)
            {
                projectile.Kill();
            }
            if (Charge == MAX_CHARGE - 1)
            {
                Main.PlaySound(SoundID.Item104);
            }
            Vector2 mousePos = Main.MouseWorld;
            Player player = Main.player[projectile.owner];

            #region Set projectile position
            if (projectile.owner == Main.myPlayer) // Multiplayer support
            {
                Vector2 diff = mousePos - player.Center;
                diff.Normalize();
                projectile.velocity = diff;
                projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                projectile.netUpdate = true;
            }
            projectile.position = player.Center + projectile.velocity * MOVE_DISTANCE;
            projectile.timeLeft = 2;
            int dir = projectile.direction;
            player.ChangeDir(dir);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * dir,
                projectile.velocity.X * dir);
            #endregion

            #region Charging process
            // Kill the projectile if the player stops channeling
            if (!player.channel)
            {
                projectile.Kill();
            }
            else
            {
                if (Main.time % 10 < 1 && !player.CheckMana(player.inventory[player.selectedItem].mana, true))
                {
                    projectile.Kill();
                }
                Vector2 offset = projectile.velocity;
                offset *= MOVE_DISTANCE - 20;
                Vector2 pos = player.Center + offset - new Vector2(10, 10);
                if (Charge < MAX_CHARGE)
                {
                    Charge++;
                }
                int chargeFact = (int)(Charge / 20f);
                Vector2 dustVelocity = Vector2.UnitX * 18f;
                dustVelocity = dustVelocity.RotatedBy(projectile.rotation - 1.57f, default(Vector2));
                Vector2 spawnPos = projectile.Center + dustVelocity;
                for (int k = 0; k < chargeFact + 1; k++)
                {
                    Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - (chargeFact * 2));
                    Dust dust = Main.dust[Dust.NewDust(pos, 20, 20, 187, projectile.velocity.X / 2f,
                        projectile.velocity.Y / 2f, 0, default(Color), 1f)];
                    dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
                    dust.noGravity = true;
                    dust.scale = Main.rand.Next(10, 20) * 0.05f;
                }
            }
            #endregion


            #region Set laser tail position and dusts
            if (Charge < MAX_CHARGE) return;
            Vector2 start = player.Center;
            Vector2 unit = projectile.velocity;
            unit *= -1;
            for (Distance = MOVE_DISTANCE; Distance <= 2200f; Distance += 8f)
            {
                start = player.Center + projectile.velocity * Distance;
                if (!Collision.CanHit(player.Center, 1, 1, start, 1, 1))
                {
                    Distance -= 5f;
                    break;
                }
            }

            Vector2 dustPos = player.Center + projectile.velocity * Distance;
            
            //Imported dust code from source because I'm lazy
            for (int i = 0; i < 2; ++i)
            {
                float num1 = projectile.velocity.ToRotation() + (Main.rand.Next(2) == 1 ? -1.0f : 1.0f) * 1.57f;
                float num2 = (float)(Main.rand.NextDouble() * 0.8f + 1.0f);
                Vector2 dustVel = new Vector2((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
                Dust dust = Main.dust[Dust.NewDust(dustPos, 0, 0, 187, dustVel.X, dustVel.Y, 0, new Color(), 1f)];
                dust.noGravity = true;
                dust.scale = 2f;
                // At this part, I was messing with the dusts going across the laser beam very fast, but only really works properly horizontally now
                dust = Main.dust[Dust.NewDust(Main.player[projectile.owner].Center + unit * 5f, 0, 0, 187, unit.X, unit.Y, 0, new Color(), 1f)];
                dust.fadeIn = 0f;
                dust.noGravity = true;
                dust.scale = 0.88f;
            }

            Vector2 direction = projectile.velocity;
            direction.Normalize();
            Vector2 perpendicular = direction.RotatedBy(MathHelper.ToRadians(90));
            for (int i = 0; i < Distance / 32; i++)
            {
                dustPos = player.Center + direction * Main.rand.NextFloat(16f, Distance + 16f) + perpendicular * (Main.rand.NextBool() ? -1f : 1f) * (projectile.width + 12) / 2;
                Vector2 dVel = direction * 32f;
                Dust.NewDustPerfect(dustPos, 187, dVel).noGravity = true;
            }
            #endregion

            //Add lights
            DelegateMethods.v3_1 = new Vector3(0f, 1f, 0f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * (Distance - MOVE_DISTANCE), 26, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit * Distance, (projectile.width + 16) * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }
    }
}
