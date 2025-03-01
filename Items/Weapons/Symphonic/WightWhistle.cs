using Necromancy.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Items.Weapons.Symphonic
{
	public class WightWhistle : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wight Whistle");
            Tooltip.SetDefault("Empowers allies with bonus mana regeneration");
        }

        public override void SetDefaults()
        {
            item.magic = true;
            item.damage = 41;
            item.width = 36;
			item.height = 46;
			item.useTime = 9;
			item.useAnimation = 9;
			item.useStyle = 5;
            item.noMelee = true;
            Item.staff[item.type] = true;
            item.knockBack = 5;
			item.value = Item.sellPrice(0, 4);
			item.rare = 6;
            item.noUseGraphic = true;
			item.UseSound = SoundID.Item15;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("WhistleRay");
			item.shootSpeed = 16f;
            item.prefix = 0;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).necrotic = true;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).symphonic = true;
            item.GetGlobalItem<NecromancyGlobalItem>(mod).lifeCost = 8;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // random spread of 10 degrees
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(10));
            Projectile proj = Projectile.NewProjectileDirect(position, perturbedSpeed, type, damage, knockBack, player.whoAmI);
            proj.GetGlobalProjectile<NecromancyGlobalProjectile>(mod).shotFrom = item;
            return false;
        }
    }
}