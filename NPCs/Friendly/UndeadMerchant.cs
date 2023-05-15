﻿using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.Utilities;
using tsorcRevamp.Buffs;

namespace tsorcRevamp.NPCs.Friendly
{
    [AutoloadHead]
    class UndeadMerchant : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Undead Merchant");
            Main.npcFrameCount[NPC.type] = 26;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 200;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 18;
            NPCID.Sets.AttackAverageChance[NPC.type] = 10;
            NPCID.Sets.HatOffsetY[NPC.type] = 4;
        }
        public override List<string> SetNPCNameList() {
            return new List<string> { "Uldred" };
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 50;
            NPC.defense = 15;
            NPC.lifeMax = 1000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.SkeletonMerchant;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            chat.Add("Well, now... You seem to have your wits about you, hmm? Then you are a welcome customer! I trade for souls. Everything's for sale! Nee hee hee hee hee!", 1.5);
            chat.Add("I hope you've brought plenty of souls? Nee hee hee hee hee!");
            chat.Add("Oh, there you are. Where have you been hiding? I guessed you'd hopped the twig for certain. Bah, shows what I know! Nee hee hee hee!");
            chat.Add("Oh? Still not popped your clogs?");
            chat.Add("Oh, there you are. Still keeping your marbles all together? Then, go ahead, don't be a nitwit. Never hurts to splurge when your days are numbered! Nee hee hee hee hee!");
            chat.Add("Eh? I'm not here to chit-chat. We talk business, or we talk nothing at all.");
            chat.Add("[c/ffbf00:If our paths cross again, I may have new items for sale. Don't ask where I got'em!]");

            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Shop";
                return;
            }
        }

        public override void AddShops() {


            
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            NPCShop shop = new(NPC.type);
            shop.Add(new Item(ModContent.ItemType<Items.CharcoalPineResin>()) {
                shopCustomPrice = 20, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.Weapons.Throwing.Firebomb>()) {
                shopCustomPrice = 5, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.BloodredMossClump>()) {
                shopCustomPrice = 4, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.Potions.Lifegem>()) {
                shopCustomPrice = 20, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.Potions.GlowingMushroomSkewer>()) {
                shopCustomPrice = 5, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.Potions.HealingElixir>()) {
                shopCustomPrice = 30, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.Potions.GreenBlossom>()) {
                shopCustomPrice = 30, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ItemID.BattlePotion) {
                shopCustomPrice = 30, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ItemID.WormholePotion) {
                shopCustomPrice = 20, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.Accessories.Defensive.IronShield>()) {
                shopCustomPrice = 200, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.Armors.HollowSoldierHelmet>()) {
                shopCustomPrice = 100, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.Armors.HollowSoldierBreastplate>()) {
                shopCustomPrice = 100, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });

            shop.Add(new Item(ModContent.ItemType<Items.Armors.HollowSoldierWaistcloth>()) {
                shopCustomPrice = 100, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            });


   
            shop.Add(new Item(ModContent.ItemType<Items.ItemCrates.ThrowingKnifeCrate>()) {
                shopCustomPrice = 10, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, Condition.DownedEyeOfCthulhu);
            


            shop.Add(new Item(ItemID.VilePowder) {
                shopCustomPrice = 1, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, Condition.DownedEowOrBoc);
            


            shop.Add(new Item(ItemID.SlimySaddle) {
                shopCustomPrice = 1000, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, Condition.Hardmode);

            shop.Add(new Item(ModContent.ItemType<Items.Potions.RadiantLifegem>()) {
                shopCustomPrice = 60, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, Condition.Hardmode);

            

            shop.Add(new Item(ModContent.ItemType<Items.Weapons.Throwing.BlackFirebomb>()) {
                shopCustomPrice = 50, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, Condition.DownedDestroyer);



            shop.Add(new Item(ItemID.QueenSlimeMountSaddle) {
                shopCustomPrice = 2000, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, Condition.DownedSkeletronPrime);



            shop.Add(new Item(ModContent.ItemType<Items.PurgingStone>()) {
                shopCustomPrice = 10000, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, new Condition("", () => tsorcRevampWorld.SuperHardMode));
            


            shop.Add(new Item(ModContent.ItemType<Items.Armors.MaskOfTheChild>()) {
                shopCustomPrice = 1000, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, Condition.BloodMoon);

            shop.Add(new Item(ModContent.ItemType<Items.Armors.MaskOfTheFather>()) {
                shopCustomPrice = 1000, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, Condition.BloodMoon);

            shop.Add(new Item(ModContent.ItemType<Items.Armors.MaskOfTheMother>()) {
                shopCustomPrice = 1000, 
                shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId
            }, Condition.BloodMoon);
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            if (Main.hardMode)
            {
                damage = 100;
                knockback = 7f;
            }
            else
            {
                damage = 60;
                knockback = 5f;
            }
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 50;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<Projectiles.FirebombProj>();
            attackDelay = 5;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 6.5f;
            gravityCorrection = 30f;
            randomOffset = 0f;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)/* tModPorter Suggestion: Copy the implementation of NPC.SpawnAllowed_Merchant in vanilla if you to count money, and be sure to set a flag when unlocked, so you don't count every tick. */
        {
            int type = ModContent.ItemType<Items.SoulCoin>();

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                {
                    continue;
                }

                if (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || player.CountItem(type, 150) >= 150 || player.HasBuff(ModContent.BuffType<Bonfire>()))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return true;
        }
    }
}
