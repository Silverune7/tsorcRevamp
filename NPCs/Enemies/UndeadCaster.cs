﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;
using tsorcRevamp.Items.Potions;
using static tsorcRevamp.SpawnHelper;

namespace tsorcRevamp.NPCs.Enemies
{
    class UndeadCaster : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.GoblinSorcerer);
            AIType = NPCID.GoblinSorcerer;
            NPC.lifeMax = 30;
            NPC.damage = 0;
            NPC.scale = 1f;
            NPC.knockBackResist = 0.1f;
            NPC.value = 300; // was 40
            NPC.defense = 6;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Banners.UndeadCasterBanner>();
            NPC.height = 44;
            NPC.width = 28;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;

            if (NPC.downedBoss1)
            {
                NPC.lifeMax = 60;
                NPC.defense = 12;
                NPC.value = 600; // was 75
                NPC.damage = 0;
            }

            if (NPC.downedBoss3)
            {
                NPC.lifeMax = 120;
                NPC.defense = 15;
                NPC.value = 900; // was 95
                NPC.damage = 0;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {

            float chance = 0;
            Player p = spawnInfo.Player;
            if (!Main.hardMode && p.ZoneRockLayerHeight && !spawnInfo.Player.ZoneDungeon && !spawnInfo.Player.ZoneJungle && Main.dayTime) return 0.0285f;
            if (!Main.hardMode && p.ZoneRockLayerHeight && !spawnInfo.Player.ZoneDungeon && !spawnInfo.Player.ZoneJungle && !Main.dayTime) return 0.05f;
            if (!Main.hardMode && Sky(p) && !Main.dayTime) return 0.025f;
            if (!Main.hardMode && Sky(p) && Main.dayTime) return 0.05f;
            if (!Main.hardMode && spawnInfo.Player.ZoneSnow) return 0.1f;
            if (!Main.hardMode && (spawnInfo.Player.ZoneRockLayerHeight && !(!spawnInfo.Player.ZoneDungeon && spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson || spawnInfo.Player.ZoneJungle)) && !Main.dayTime) return 0.033f;

            return chance;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, 27, 2 * hit.HitDirection, -1.75f);
                }
                Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));

                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Undead Caster Gore 1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Undead Caster Gore 2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Undead Caster Gore 2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Undead Caster Gore 3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Undead Caster Gore 3").Type, 1f);
                }
            }
        }

        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart, 1);
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart, 1);
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart, 1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.SpellTome, 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeadChicken>(), 25));
            npcLoot.Add(new CommonDrop(ModContent.ItemType<Items.AttraidiesRelic>(), 100, 1, 1, 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Magic.WandOfFire>(), 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Magic.WandOfDarkness>(), 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Magic.WoodenWand>(), 5));
            npcLoot.Add(ItemDropRule.Common(ItemID.Diamond, 1, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.HealingPotion, 12, 2, 2));
            npcLoot.Add(ItemDropRule.Common(ItemID.Diamond, 8));
            npcLoot.Add(ItemDropRule.ByCondition(tsorcRevamp.tsorcItemDropRuleConditions.CursedRule, ModContent.ItemType<Lifegem>(), 4));
            npcLoot.Add(ItemDropRule.ByCondition(tsorcRevamp.tsorcItemDropRuleConditions.CursedRule, ModContent.ItemType<StarlightShard>(), 10));
            int[] armorIDs = new int[] {
                ModContent.ItemType<Items.Armors.Magic.RedClothHat>(),
                ModContent.ItemType<Items.Armors.Magic.RedClothTunic>(),
                ModContent.ItemType<Items.Armors.Magic.RedClothPants>(),
            };
            //i just wanna say that, while terraria's convention is (denominator, numerator)
            //and im following that convention with DropMultiple, i completely detest it
            npcLoot.Add(new DropMultiple(armorIDs, 20, 1, !NPC.downedBoss1));
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D glowTexture = (Texture2D)Mod.Assets.Request<Texture2D>("NPCs/Enemies/UndeadCaster_Glow");
            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (NPC.spriteDirection == 1)
            {
                spriteBatch.Draw(glowTexture, NPC.Center - Main.screenPosition, new Rectangle(NPC.frame.X, NPC.frame.Y, 50, 68), lightColor, NPC.rotation, new Vector2(25, 42), NPC.scale, effects, 0f);
            }
            else
            {
                spriteBatch.Draw(glowTexture, NPC.Center - Main.screenPosition, new Rectangle(NPC.frame.X, NPC.frame.Y, 50, 68), lightColor, NPC.rotation, new Vector2(25, 42), NPC.scale, effects, 0f);
            }

        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            if (NPC.ai[1] > 0f)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
    }
}