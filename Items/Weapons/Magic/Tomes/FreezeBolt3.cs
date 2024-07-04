﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;

namespace tsorcRevamp.Items.Weapons.Magic.Tomes
{
    class FreezeBolt3 : ModItem
    {

        public override string Texture => "tsorcRevamp/Items/Weapons/Magic/Tomes/FreezeBolt2";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Freeze Bolt III");
            // Tooltip.SetDefault("Casts a super fast-moving bolt of ice");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.damage = 280;
            Item.knockBack = 8;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item21;
            Item.rare = ItemRarityID.Cyan;
            Item.shootSpeed = 16;
            Item.mana = 21;
            Item.value = PriceByRarity.Cyan_9;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<Projectiles.FreezeBolt3>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<FreezeBolt2>(), 1);
            //recipe.AddIngredient(ModContent.ItemType<SoulOfAttraidies>(), 1);
            recipe.AddIngredient(ModContent.ItemType<BlueTitanite>(), 9);
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 110000);
            recipe.AddTile(TileID.DemonAltar);

            recipe.Register();
        }
    }
}
