﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Weapons.Magic {
    class Ice1Tome : ModItem {

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Ice 1 Tome");
            Tooltip.SetDefault("A lost beginner's tome \nCan be upgraded.");
        }
        public override void SetDefaults() {
            item.damage = 10;
            item.height = 10;
            item.knockBack = 0f;
            item.channel = true;
            item.autoReuse = true;
            item.rare = ItemRarityID.Green;
            item.shootSpeed = 9;
            item.magic = true;
            item.noMelee = true;
            item.mana = 5;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 10;
            item.value = 200000;
            item.width = 34;
            item.shoot = ModContent.ProjectileType<Projectiles.Ice1Ball>();
        }

        public int count = 0;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (count < 10)
            {
                count++;
                return true;
            }
            else return false;
        }


        public override void AddRecipes() {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpellTome, 1);
            recipe.AddIngredient(mod.GetItem("DarkSoul"), 3000);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
