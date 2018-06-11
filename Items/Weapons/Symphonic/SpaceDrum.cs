using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Necromancy.Items.Weapons.Symphonic
{
	public class SpaceDrum : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Space Drum");
            Tooltip.SetDefault("Creates a pulse of space bass" +
                "\nEmpowers players with stacking movement speed");            
        }

        public override void SetDefaults()
        {
            item.magic = true;
            item.damage = 15;
            item.crit = 4;
            item.width = 38;
			item.height = 38;
			item.useTime = 35;
			item.useAnimation = 35;
			item.useStyle = 5;
            item.knockBack = 5;
			item.value = Item.sellPrice(0, 2, 0, 0);
			item.rare = 2;
			item.UseSound = SoundID.Item10;
			item.autoReuse = true;
            item.shoot = mod.ProjectileType("SpaceBassPulse");
			item.shootSpeed = 32f;
            item.prefix = 0;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).necrotic = true;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).symphonic = true;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).baseLifeCost = 3;
        }
        
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 32;
            float rotation = MathHelper.ToRadians(11.25f);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(rotation * i) * 0.4f;
                Projectile projectile = Main.projectile[Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI)];
                projectile.GetGlobalProjectile<Projectiles.NecromancyGlobalProjectile>(mod).shotFrom = item;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }
}