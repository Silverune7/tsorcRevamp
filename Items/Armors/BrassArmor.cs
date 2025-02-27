﻿using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;

namespace tsorcRevamp.Items.Armors
{
    [LegacyName("AncientBrassArmor")]
    [AutoloadEquip(EquipType.Body)]
    public class BrassArmor : ModItem
    {
        public static float DR = 8f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DR);
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 12;
            Item.rare = ItemRarityID.Orange;
            Item.value = PriceByRarity.fromItem(Item);
        }
        public override void UpdateEquip(Player player)
        {
            player.noKnockback = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<BrassHelmet>() && legs.type == ModContent.ItemType<BrassGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.endurance += DR / 100f;
            if ((float)player.statLife > ((float)player.statLifeMax2 * 0.25f))
            {
                player.hasPaladinShield = true;
                if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
                {
                    int myPlayer = Main.myPlayer;
                    if (Main.player[myPlayer].team == player.team && player.team != 0)
                    {
                        float num4 = player.position.X - Main.player[myPlayer].position.X;
                        float num2 = player.position.Y - Main.player[myPlayer].position.Y;
                        if ((float)Math.Sqrt(num4 * num4 + num2 * num2) < 800f)
                        {
                            Main.player[myPlayer].AddBuff(43, 20);
                        }
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.PlatinumChainmail, 1);
            recipe.AddIngredient(ItemID.BeeWax, 3);
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 3000);
            recipe.AddTile(TileID.DemonAltar);

            recipe.Register();
        }
    }
}
