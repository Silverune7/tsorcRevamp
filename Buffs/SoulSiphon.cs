﻿using Terraria;
using Terraria.ModLoader;
using tsorcRevamp.Items.Potions;

namespace tsorcRevamp.Buffs
{
    public class SoulSiphon : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<tsorcRevampPlayer>().SoulSiphon = true;
            player.GetModPlayer<tsorcRevampPlayer>().SoulReaper += 5;
            player.GetModPlayer<tsorcRevampPlayer>().ConsSoulChanceMult += SoulSiphonPotion.ConsSoulChanceAmplifier / 5;
        }
    }
}
