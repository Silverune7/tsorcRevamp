using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Ammo;
using tsorcRevamp.Items.Materials;

namespace tsorcRevamp.Items.Weapons.Ranged.Specialist
{
    public class FocusedEnergyBeam : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.damage = 750;
            Item.width = 52;
            Item.height = 22;
            Item.knockBack = 9.0f;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.useAmmo = ModContent.ItemType<TeslaBolt>();
            Item.shootSpeed = 30;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.UseSound = SoundID.Item12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = PriceByRarity.Red_10;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<RedTitanite>(), 3);
            recipe.AddIngredient(ModContent.ItemType<WhiteTitanite>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CompactFrame>());
            recipe.AddIngredient(ModContent.ItemType<DestructionElement>());
            recipe.AddIngredient(ItemID.SpaceGun, 1);
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 50000);

            recipe.AddTile(TileID.DemonAltar);

            recipe.Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
    }
}
