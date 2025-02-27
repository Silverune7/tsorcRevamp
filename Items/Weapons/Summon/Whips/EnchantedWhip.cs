using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;
using tsorcRevamp.Projectiles.Summon.Whips.EnchantedWhip;

namespace tsorcRevamp.Items.Weapons.Summon.Whips
{
    public class EnchantedWhip : ModItem
    {
        public const int BaseDamage = 18;
        public static float SummonTagDamage = 4;
        public static float StarDamageScaling = 44;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SummonTagDamage, StarDamageScaling);
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;  //journey mode lmao
        }

        public override void SetDefaults()
        {
            Item.height = 60;
            Item.width = 52;

            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.damage = BaseDamage;
            Item.knockBack = 1.5f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 3, 50, 0);

            Item.shoot = ModContent.ProjectileType<EnchantedWhipProjectile>();
            Item.shootSpeed = 4;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 40; // for some reason a lower use speed gives it increased range....
            Item.useAnimation = 40;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
        public override bool MeleePrefix()
        {
            return true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BlandWhip, 1);
            recipe.AddIngredient(ItemID.FallenStar, 10);
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 4000);

            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
