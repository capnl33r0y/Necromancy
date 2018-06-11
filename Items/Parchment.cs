using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Items
{
	public class Parchment : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Parchment");
            Tooltip.SetDefault("Used to make mystical scrolls or basic spell tomes");
        }

        public override void SetDefaults()
		{
			item.width = 36;
			item.height = 36;
            item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 0, 20);
			item.rare = 2;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<NecromancyPlayer>(mod).regenMult *= 0.5f;
            player.GetModPlayer<NecromancyPlayer>(mod).lifeStealMult *= 1.2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Leather, 2);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(mod, "Parchment");
            recipe.AddRecipe();

            // for leather in crimson
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Vertebrae, 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(ItemID.Leather);
            recipe.AddRecipe();
        }
    }
}