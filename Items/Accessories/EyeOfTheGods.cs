﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;

namespace tsorcRevamp.Items.Accessories
{
    public class EyeOfTheGods : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {

            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.value = PriceByRarity.Green_2;
            Item.vanity = true;
        }
        public override void UpdateEquip(Player player)
        {
            int cursorX = (int)((Main.mouseX + Main.screenPosition.X) / 16);
            int cursorY = (int)((Main.mouseY + Main.screenPosition.Y) / 16);
            Lighting.AddLight(cursorX, cursorY, 2.5f, 2.5f, 2.5f);
        }
        public override void UpdateVanity(Player player)
        {
            int cursorX = (int)((Main.mouseX + Main.screenPosition.X) / 16);
            int cursorY = (int)((Main.mouseY + Main.screenPosition.Y) / 16);
            Lighting.AddLight(cursorX, cursorY, 2.5f, 2.5f, 2.5f);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ShinePotion, 1);
            recipe.AddIngredient(ItemID.SpelunkerPotion, 1);
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 6000);
            recipe.AddTile(TileID.DemonAltar);

            recipe.Register();
        }
    }
}
