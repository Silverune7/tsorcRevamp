﻿using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using tsorcRevamp.Utilities;

namespace tsorcRevamp.Items.Accessories.Mobility.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    [LegacyName("DragonWings")]
    public class WingsOfSeath : ModItem
    {
        public bool Slow = false;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(999999999, 10f, 1.4f, true, 3, 3);
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 42;
            Item.accessory = true;
            Item.value = PriceByRarity.Purple_11;
            Item.expert = true;

        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
                    ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 1.1f;
            ascentWhenRising = 0.35f;
            maxCanAscendMultiplier = 1.3f;
            maxAscentMultiplier = 3.6f;
            constantAscend = 0.2f;



            if (player.TryingToHoverDown && player.controlJump && player.wingTime > 0f && !player.merman)
            {
                player.velocity.Y = 0;
                ascentWhenFalling = 0;
                ascentWhenRising = 0;
                maxCanAscendMultiplier = 0;
                maxAscentMultiplier = 0;
                constantAscend = 0;
            }
        }
        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 14f;
            acceleration = 0.45f;
            if (player.TryingToHoverDown && player.controlJump && player.wingTime > 0f && !player.merman)
            {
                speed = 16f;
                acceleration = 1f;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var WingsSpeedToggle = tsorcRevamp.WingsOfSeath.GetAssignedKeys();
            string WingsSpeedToggleString = WingsSpeedToggle.Count > 0 ? WingsSpeedToggle[0] : LangUtils.GetTextValue("Keybinds.Wings of Seath speed toggle.DisplayName") + LangUtils.GetTextValue("CommonItemTooltip.NotBound");
            int ttindex = tooltips.FindIndex(t => t.Name == "Tooltip3");
            if (ttindex != -1)
            {
                tooltips.RemoveAt(ttindex);
                tooltips.Insert(ttindex, new TooltipLine(Mod, "Keybind", Language.GetTextValue("Mods.tsorcRevamp.Items.WingsOfSeath.Keybind1") + WingsSpeedToggleString + Language.GetTextValue("Mods.tsorcRevamp.Items.WingsOfSeath.Keybind2")));
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<tsorcRevampPlayer>().supersonicLevel = 3;
            if (player.TryingToHoverDown && player.controlJump && player.wingTime > 0f && !player.merman)
            {
                player.velocity.Y = 1f;
            }
            player.wingTime = 999999999;
            //player.GetWingStats(22);
            player.jumpBoost = true;
            player.jumpSpeedBoost = 1.5f;
            player.lavaImmune = true;
            player.fireWalk = true;
            player.noKnockback = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.nightVision = true;
            player.AddBuff(BuffID.Hunter, 1);
            if (Main.netMode != NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
            {
                if (tsorcRevamp.WingsOfSeath.JustReleased)
                {
                    Slow = !Slow;
                }
                if (Slow)
                {
                    player.velocity.X *= 0.95f;
                    player.slowFall = true;
                }
            }
        }
    }
}
