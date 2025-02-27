﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Weapons.Melee.Broadswords
{
    class ForgottenSwordbreaker : ModItem
    {
        public const int CoinPrice = 6000;
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("[c/ffbf00:Striking an enemy may temporarily make you deflect attacks.]");
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.rare = ItemRarityID.Pink;
            Item.damage = 93;
            Item.width = 28;
            Item.height = 28;
            Item.knockBack = 3.5f;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.UseSound = SoundID.Item1;
            Item.value = PriceByRarity.Pink_5;
            Item.shoot = ModContent.ProjectileType<Projectiles.Nothing>();
            tsorcInstancedGlobalItem instancedGlobal = Item.GetGlobalItem<tsorcInstancedGlobalItem>();
            instancedGlobal.slashColor = Microsoft.Xna.Framework.Color.Gray;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(20))
            {
                player.AddBuff(ModContent.BuffType<Buffs.Invincible>(), 60);
            }
        }
    }
}
