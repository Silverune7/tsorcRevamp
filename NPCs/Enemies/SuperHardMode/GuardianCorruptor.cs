﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Weapons.Melee.Broadswords;
using Terraria.GameContent.ItemDropRules;
using tsorcRevamp.Items.Materials;

namespace tsorcRevamp.NPCs.Enemies.SuperHardMode
{
    class GuardianCorruptor : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.CursedInferno,
                    BuffID.Ichor
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }
        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 164;
            NPC.damage = 42;
            NPC.defense = 50;
            NPC.lifeMax = 1500; // 4500 was too much for such a dumb enemy
            NPC.aiStyle = -1;
            NPC.npcSlots = 3;
            NPC.value = 6000; //life / 2.5 : was 1875 but with way more health 
            NPC.knockBackResist = 0.01f;
            NPC.scale = 1f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Banners.GuardianCorruptorBanner>();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float chance = 0;
            Player player = spawnInfo.Player;
            if (tsorcRevampWorld.SuperHardMode)
            {
                if (player.ZoneCorrupt && player.ZoneOverworldHeight && !Main.dayTime) chance = 0.5f;
                else if (player.ZoneCorrupt && player.ZoneRockLayerHeight && !Main.dayTime) chance = 0.5f;
                else if (player.ZoneCorrupt) chance = 0.25f;
                else if (spawnInfo.Water) chance = 0.025f;
            }

            return chance;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedSoul>(), 1, 8, 16));
            npcLoot.Add(new CommonDrop(ItemID.RottenChunk, 100, 1, 5, 33));

        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (hit.Damage >= 1000000)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 10, Mod.Find<ModGore>("Guardian Corruptor Gore 1").Type, 0.9f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 10, Mod.Find<ModGore>("Guardian Corruptor Gore 2").Type, 0.9f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 10, Mod.Find<ModGore>("Guardian Corruptor Gore 3").Type, 0.9f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 10, Mod.Find<ModGore>("Guardian Corruptor Gore 4").Type, 0.9f);
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 10, Mod.Find<ModGore>("Blood Splat").Type, 3f);
                }
            }
            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Guardian Corruptor Gore 1").Type, 0.9f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Guardian Corruptor Gore 2").Type, 0.9f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Guardian Corruptor Gore 3").Type, 0.9f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Guardian Corruptor Gore 4").Type, 0.9f);
                    for (int i = 0; i < 10; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Blood Splat").Type, 0.9f);
                    }
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Weak, 120 * 60, true);
            target.AddBuff(BuffID.BrokenArmor, 60 * 60, true);
            target.AddBuff(BuffID.Poisoned, 300 * 60, true);
        }

        /*public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter < 10)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 20)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 30)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else
            {
                NPC.frameCounter = 0;
            }
        }*/
        public override void AI()
        {
            if (Main.player[Main.myPlayer].HeldItem.type == ModContent.ItemType<CorruptedTooth>())
            {
                Dust.NewDust(NPC.Center, 10, 10, DustID.WaterCandle, 2, 2, Scale: 2);
            }

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest();
            }

            float accelRate = 0.02f;
            Vector2 enemyPos = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float targetPosX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float targetPosY = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            targetPosX = (int)(targetPosX / 8f) * 8;
            targetPosY = (int)(targetPosY / 8f) * 8;
            enemyPos.X = (int)(enemyPos.X / 8f) * 8;
            enemyPos.Y = (int)(enemyPos.Y / 8f) * 8;
            targetPosX -= enemyPos.X;
            targetPosY -= enemyPos.Y;
            float targetDistSegment = (float)Math.Sqrt(targetPosX * targetPosX + targetPosY * targetPosY);
            float targetDistAbsolute = targetDistSegment;
            if (targetDistSegment == 0f)
            {
                targetPosX = NPC.velocity.X;
                targetPosY = NPC.velocity.Y;
            }
            else
            {
                targetDistSegment = 8f / targetDistSegment;
                targetPosX *= targetDistSegment;
                targetPosY *= targetDistSegment;
            }
            NPC.ai[0] += 1f;
            if (NPC.ai[0] > 0f)
            {
                NPC.velocity.Y += 0.01f;
            }
            else
            {
                NPC.velocity.Y -= 0.023f;
            }
            if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
            {
                NPC.velocity.X += 0.023f;
            }
            else
            {
                NPC.velocity.X -= 0.023f;
            }
            if (NPC.ai[0] > 200f)
            {
                NPC.ai[0] = -200f;
            }

            if (targetDistAbsolute < 150f)
            {
                NPC.velocity.X += targetPosX * 0.005f;
                NPC.velocity.Y += targetPosY * 0.005f;
            }
            if (targetDistAbsolute > 600f)
            {
                NPC.velocity.X += targetPosX * 0.005f;
                NPC.velocity.Y += targetPosY * 0.005f;
            }

            if (Main.player[NPC.target].dead)
            {
                targetPosX = (float)NPC.direction * 4.2f / 2f;
                targetPosY = -4.2f / 2f;
            }
            if (NPC.velocity.X < targetPosX)
            {
                NPC.velocity.X += accelRate;
            }
            else if (NPC.velocity.X > targetPosX)
            {
                NPC.velocity.X -= accelRate;

            }
            if (NPC.velocity.Y < targetPosY)
            {
                NPC.velocity.Y += accelRate;

            }
            else if (NPC.velocity.Y > targetPosY)
            {
                NPC.velocity.Y -= accelRate;

            }

            NPC.rotation = (float)Math.Atan2(targetPosY, targetPosX) - 1.57f;


            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * (-0.7f);
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                {
                    NPC.velocity.X = 2f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                {
                    NPC.velocity.X = -2f;
                }
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * (-0.7f);
                if (NPC.velocity.Y > 0f && (double)NPC.velocity.Y < 1.5)
                {
                    NPC.velocity.Y = 2f;
                }
                if (NPC.velocity.Y < 0f && (double)NPC.velocity.Y > -1.5)
                {
                    NPC.velocity.Y = -2f;
                }
            }
            else if (Main.rand.NextBool(12))
            {
                int eaterDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + (float)NPC.height * 0.25f), NPC.width, (int)((float)NPC.height * 0.5f), 18, NPC.velocity.X, 2f, 75);
                Main.dust[eaterDust].velocity.X *= 0.5f;
                Main.dust[eaterDust].velocity.Y *= 0.1f;
            }

            if (NPC.wet)
            {
                if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y *= 0.95f;
                }
                NPC.velocity.Y -= 0.3f;
                if (NPC.velocity.Y < -2f)
                {
                    NPC.velocity.Y = -2f;
                }
            }


            if (Main.netMode != NetmodeID.MultiplayerClient && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
            {
                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] == 30f)
                {
                    if (Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        int shotCount = 3;
                        Vector2 shotOrigin = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                        float distX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - shotOrigin.X;
                        float distY = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - shotOrigin.Y;
                        float distAbs = (float)Math.Sqrt(distX * distX + distY * distY);
                        distAbs = 7f / distAbs;

                        for (int i = 0; i < shotCount; i++)
                        {
                            Vector2 shotDirection = new Vector2((distX * distAbs) / 1.5f, (distY * distAbs) / 1.5f);
                            int guardianSpit = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2) + NPC.velocity.X), (int)(NPC.position.Y + (float)(NPC.height / 2) + NPC.velocity.Y), ModContent.NPCType<ViciousSpit>());
                            Main.npc[guardianSpit].velocity = 1.5f * shotDirection.RotatedBy(MathHelper.ToRadians(12 - (12 * i)));
                        }
                    }
                    NPC.localAI[0] = 0f;
                }
            }

            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }
    }
}
