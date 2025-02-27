﻿using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;

namespace tsorcRevamp.Items.Armors.Summon
{
    [AutoloadEquip(EquipType.Legs)]
    public class TarantulaLegs : ModItem
    {
        public static int MinionSlots = 1;
        public static float MoveSpeed = 22f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionSlots, MoveSpeed);
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 9;
            Item.rare = ItemRarityID.Pink;
            Item.value = PriceByRarity.fromItem(Item);
        }
        public override void UpdateEquip(Player player)
        {
            player.maxMinions += MinionSlots;
            player.moveSpeed += MoveSpeed / 100f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SpiderGreaves);
            recipe.AddIngredient(ItemID.MythrilBar);
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 8400);
            recipe.AddTile(TileID.DemonAltar);

            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.SpiderGreaves);
            recipe2.AddIngredient(ItemID.OrichalcumLeggings);
            recipe2.AddTile(TileID.DemonAltar);

            recipe2.Register();
        }
    }
}