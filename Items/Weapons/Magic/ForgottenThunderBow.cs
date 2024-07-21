﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;
using tsorcRevamp.Items.Weapons.Magic.Tomes;

namespace tsorcRevamp.Items.Weapons.Magic
{
    class ForgottenThunderBow : ModItem
    {

        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Casts a bolt of lightning from your bow, doing massive damage over time. ");
        }
        public override void SetDefaults()
        {
            Item.damage = 190;
            Item.height = 78;
            Item.knockBack = 4;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ItemRarityID.Red;
            Item.mana = 100;
            Item.shootSpeed = 33;
            Item.useAnimation = 40;
            Item.UseSound = SoundID.Item5;
            Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = PriceByRarity.Red_10;
            Item.width = 30;
            Item.shoot = ModContent.ProjectileType<Projectiles.Bolt4Ball>();
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ForgottenThunderBowScroll>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Bolt4Tome>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SoulOfArtorias>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Humanity>(), 9);
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 160000);
            recipe.AddTile(TileID.DemonAltar);

            recipe.Register();
        }
    }
}
