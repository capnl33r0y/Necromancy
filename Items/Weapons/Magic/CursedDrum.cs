using Necromancy.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Necromancy.Items.Weapons.Magic
{
	public class CursedDrum : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Drum");
            Tooltip.SetDefault("Unleashes a ring of cursed flames");
        }

        public override void SetDefaults()
        {
            item.magic = true;
            item.damage = 40;
            item.width = 38;
			item.height = 38;
			item.useTime = 25;
			item.useAnimation = 25;
			item.useStyle = 4;
            item.noUseGraphic = true;
            item.knockBack = 5;
			item.value = Item.sellPrice(0, 2);
			item.rare = 4;
			item.UseSound = SoundID.Item20;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("CursedFireball");
			item.shootSpeed = 16f;
            item.prefix = 0;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).necrotic = true;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).magic = true;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).lifeCost = 8;
        }
        
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 8;
            float rotation = MathHelper.ToRadians(360 / numberProjectiles);
            for (int i = 0; i < numberProjectiles; i++) // shoots 8 projectiles at equally spaced angles around the player
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(rotation * i) * 0.4f;
                Projectile projectile = Main.projectile[Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI)];
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod, "CursedBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}