using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DemonHelm : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon's Helm");
            Tooltip.SetDefault("60% increased necrotic damage" +
                "\n-10 life cost");
        }

        public override void SetDefaults()
		{
			item.width = 20;
			item.height = 24;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = 10;
			item.defense = 50;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).thoriumRarity = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<NecromancyPlayer>(mod).necroticMult += 0.6f;
            player.GetModPlayer<NecromancyPlayer>(mod).lifeCostModifier -= 10;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == mod.ItemType("DemonGuard") && legs.type == mod.ItemType("DemonTreads");
		}

		public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+500 Max Life" +
                "\nNecrotic melee attacks restore some life for all nearby allies" +
                "\nNecrotic magic attacks life costs are halved";
            player.statLifeMax2 += 500;
            player.GetModPlayer<NecromancyPlayer>().demonHelm = true;
        }

        public override void AddRecipes()
        {
            Mod thorium = ModLoader.GetMod("ThoriumMod");
            if (thorium != null)
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddIngredient(thorium, "InfernoEssence");
                recipe.AddIngredient(thorium, "DeathEssence");
                recipe.AddIngredient(thorium, "OceanEssence");
                recipe.AddTile(TileID.LunarCraftingStation);
                recipe.SetResult(this);
                recipe.AddRecipe();
            }
        }

        public override void ArmorSetShadows(Player player)
        {
            if (IsArmorSet(player.armor[0], player.armor[1], player.armor[2]))
            {
                player.armorEffectDrawOutlines = true;
            }
        }
    }
}