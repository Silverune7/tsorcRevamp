using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Accessories.Defensive
{
    public class WolfRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("One of the rings worn by Artorias." +
                                "\nImmunity to the on-fire and broken-armor debuffs." +
                                "\n+18 defense within the Abyss, +6 defense otherwise." +
                                "\nGrants Acid Venom imbue effect" +
                                "\nImbue effect can be toggled by hiding the accessory.");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.defense = 6;
            //Item.lifeRegen = 8;
            Item.accessory = true;
            Item.value = PriceByRarity.Red_10;
            Item.rare = ItemRarityID.Red;
        }


        public override void UpdateEquip(Player player)
        {
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;

            if (Main.bloodMoon)
            { // Apparently this is the flag used in the Abyss?
                player.statDefense += 12;
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual) {
            if (!hideVisual) player.AddBuff(BuffID.WeaponImbueVenom, 60, false);
        }

    }
}