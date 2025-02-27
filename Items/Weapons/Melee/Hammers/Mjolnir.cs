﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;
using tsorcRevamp.Projectiles.Melee;
using tsorcRevamp.Projectiles.Melee.Hammers;

namespace tsorcRevamp.Items.Weapons.Melee.Hammers
{
    public class Mjolnir : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 59;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 33;
            Item.useTime = 10;
            Item.damage = 50;
            Item.knockBack = 15;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.LightRed;
            Item.value = PriceByRarity.LightRed_4;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<Projectiles.Nothing>();
            tsorcInstancedGlobalItem instancedGlobal = Item.GetGlobalItem<tsorcInstancedGlobalItem>();
            instancedGlobal.slashColor = Microsoft.Xna.Framework.Color.Blue;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<MjolnirElectrosphere>(), (int)player.GetTotalDamage(DamageClass.Melee).ApplyTo(Item.damage), player.GetTotalKnockback(DamageClass.Melee).ApplyTo(Item.knockBack), Main.myPlayer, player.GetTotalCritChance(DamageClass.Melee) + Item.crit);
            if (player.wet)
            {
                player.AddBuff(BuffID.Electrified, 90);
            }
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Pwnhammer);
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 16000);
            recipe.AddTile(TileID.DemonAltar);

            recipe.Register();
        }

        public override void MeleeEffects(Player player, Rectangle rectangle)
        {
            Color color = new Color();
            //This is the same general effect done with the Fiery Greatsword
            int dust = Dust.NewDust(new Vector2((float)rectangle.X, (float)rectangle.Y), rectangle.Width, rectangle.Height, 15, (player.velocity.X * 0.2f) + (player.direction * 3), player.velocity.Y * 0.2f, 100, color, 1.0f);
            Main.dust[dust].noGravity = true;
        }

    }
}
