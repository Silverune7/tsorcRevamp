﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors
{
    [AutoloadEquip(EquipType.Legs)]
    public class AncientBrassGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases minion damage by 3 flat + 8%\nIncreases movement speed by 22%");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 5;
            Item.value = 6000;
            Item.rare = ItemRarityID.Orange;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon).Flat += 3f;
            player.GetDamage(DamageClass.Summon) += 0.08f;
            player.moveSpeed += 0.22f;
        }


        public override void AddRecipes()
        {
            Terraria.Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BeeGreaves, 1);
            recipe.AddIngredient(Mod.Find<ModItem>("DarkSoul").Type, 2600);
            recipe.AddTile(TileID.DemonAltar);

            recipe.Register();
        }
    }
}

