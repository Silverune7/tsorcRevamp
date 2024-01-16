﻿using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;
using tsorcRevamp.Projectiles.Magic.Runeterra;

namespace tsorcRevamp.Items.Weapons.Magic.Runeterra
{
    public class OrbOfDeception : ModItem
    {
        public static Color FilledColor = Color.YellowGreen;
        public static float OrbDmgMod = 100f;
        public static float OrbReturnDmgMod = 50f;
        public static float DmgLossOnPierce = 12f;
        public static float EssenceThiefOnKillChance = 17f;
        public static float FilledOrbDmgMod = 75f;
        public static float ShootSpeed = 20f;
        public static float OrbSoundVolume = 0.5f;
        public static int HealManaDivisor = 100;
        public static int HealBaseValue = 4;
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = false;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.damage = 20;
            Item.crit = 6;
            Item.mana = 25;
            Item.knockBack = 8;
            Item.UseSound = null;
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = ShootSpeed;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<OrbOfDeceptionOrb>();
            Item.holdStyle = ItemHoldStyleID.HoldLamp;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<OrbOfDeceptionOrb>()] != 0)
            {
                type = ModContent.ProjectileType<OrbOfDeceptionFlame>();
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<OrbOfDeceptionOrb>()] == 0)
            {
                type = ModContent.ProjectileType<OrbOfDeceptionOrb>();
            }
        }
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<OrbOfDeceptionOrb>()] == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<OrbOfDeceptionOrbIdle>()] == 0 && !player.dead)
            {
                Projectile.NewProjectile(Projectile.InheritSource(player), player.Center, Vector2.Zero, ModContent.ProjectileType<OrbOfDeceptionOrbIdle>(), 0, 0, player.whoAmI);
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                int ttindex = tooltips.FindLastIndex(t => t.Mod == "Terraria");
                if (ttindex != -1)
                {
                    tooltips.Insert(ttindex + 1, new TooltipLine(Mod, "Details", Language.GetTextValue("Mods.tsorcRevamp.Items.OrbOfDeception.Details").FormatWith(OrbDmgMod - 100f, (OrbReturnDmgMod * OrbDmgMod) / 100f - 100f, EssenceThiefOnKillChance, FilledOrbDmgMod - 100f)));
                }
            }
            else
            {
                int ttindex = tooltips.FindLastIndex(t => t.Mod == "Terraria");
                if (ttindex != -1)
                {
                    tooltips.Insert(ttindex + 1, new TooltipLine(Mod, "Shift", Language.GetTextValue("Mods.tsorcRevamp.CommonItemTooltip.Details")));
                }
            }
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ShadowOrb);
            recipe.AddIngredient(ModContent.ItemType<WorldRune>());
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 10000);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }


    }
}
