using Necromancy.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Items.Weapons.Magic
{
	public class TrueBookOfTheDead : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Book of the Dead");
        }

        public override void SetDefaults()
        {
            item.magic = true;
            item.damage = 83;
            item.width = 28;
			item.height = 30;
			item.useTime = 15;
			item.useAnimation = 15;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 5;
            item.value = Item.sellPrice(0, 10);
			item.rare = 6;
			item.UseSound = SoundID.Item20;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("RedBlade");
			item.shootSpeed = 16f;
            item.prefix = 0;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).necrotic = true;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).magic = true;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).lifeCost = 25;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // shoots 5 projectiles with an even spread across 15 degrees
            float numberProjectiles = 5;
            float rotation = MathHelper.ToRadians(15);
            position += new Vector2(speedX, speedY) * 3f; // projectiles created ahead of player to avoid collision with the ground if player is standing
            for (int i = 0; i < numberProjectiles; i++)
            {
                // Lerp calculates a number from a minimum number, a maximum number, and a proportion of how far from the minimum to the maximum
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                // if i = 2, it is the middple projectile so it uses YellowBlade instead of RedBlade (type)
                Projectile proj = Projectile.NewProjectileDirect(position, perturbedSpeed, (i == 2) ? mod.ProjectileType("YellowBlade") : type, damage, knockBack, player.whoAmI);
                proj.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).shotFrom = item;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod, "BookOfTheDead");
            recipe.AddIngredient(mod, "BrokenHeroTome");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}