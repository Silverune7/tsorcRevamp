using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.NPCs.Bosses.Fiends
{
	[AutoloadBossHead]
	class WaterFiendKraken : ModNPC
	{
		public override void SetDefaults()
		{
			npc.scale = 1;
			npc.npcSlots = 1;
			Main.npcFrameCount[npc.type] = 8;
			npc.width = 150;
			npc.height = 260;
			npc.damage = 65;
			npc.defense = 35;
			npc.aiStyle = 22;
			animationType = -1;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath6;
			npc.lifeMax = 200000;
			npc.timeLeft = 22500;
			npc.friendly = false;
			npc.noTileCollide = true;
			npc.noGravity = true;
			npc.knockBackResist = 0f;
			npc.lavaImmune = true;
			npc.boss = true;
			npc.value = 600000;
			npc.buffImmune[BuffID.Poisoned] = true;
			npc.buffImmune[BuffID.Confused] = true;
			npc.buffImmune[BuffID.CursedInferno] = true;
			bossBag = ModContent.ItemType<Items.BossBags.KrakenBag>();
			despawnHandler = new NPCDespawnHandler("Water Fiend Kraken submerges into the depths...", Color.DeepSkyBlue, 180);
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Water Fiend Kraken");
		}

		public float ChargeTimer = 0;
		
		int cursedFlamesDamage = 45;
		int plasmaOrbDamage = 75;
		int hypnoticDisruptorDamage = 35;
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.damage = (int)(npc.damage * 1.3 / 2);
			npc.defense = npc.defense += 12;
		}

		bool chargeDamageFlag = false;
		NPCDespawnHandler despawnHandler;

		#region AI
		public override void AI()
		{
			despawnHandler.TargetAndDespawn(npc.whoAmI);

			bool flag25 = false;
			ChargeTimer++;
			if (ChargeTimer >= 800)
			{
				int dust = Dust.NewDust(npc.position, npc.width, npc.height, 29, npc.velocity.X, npc.velocity.Y, 200, new Color(), 5);
				Main.dust[dust].velocity = UsefulFunctions.GenerateTargetingVector(npc.Center, Main.dust[dust].position, 5);
			}
			if (ChargeTimer >= 900)
			{				
				chargeDamageFlag = true;
				npc.velocity = UsefulFunctions.GenerateTargetingVector(npc.Center, Main.player[npc.target].Center, 14);
				ChargeTimer = 1f;
				npc.netUpdate = true;				
			}
			if (chargeDamageFlag == true)
			{
				npc.damage = 90;
			}
			if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 20)
			{
				chargeDamageFlag = false;
				npc.damage = 65;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (Main.rand.Next(50) == 1)
				{						
					Vector2 projVector = UsefulFunctions.GenerateTargetingVector(npc.Center, Main.player[npc.target].Center, 10);
					projVector += Main.rand.NextVector2Circular(3, 3);
					Projectile.NewProjectile(npc.Center.X, npc.Center.Y, projVector.X, projVector.Y, ModContent.ProjectileType<Projectiles.Enemy.EnemyCursedFlames>(), cursedFlamesDamage, 0f, Main.myPlayer);
					Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 17);
				}
				if (Main.rand.Next(120) == 1 && !chargeDamageFlag)
				{						
					Vector2 projVector = UsefulFunctions.GenerateTargetingVector(npc.Center, Main.player[npc.target].Center, 15);
					projVector += (Main.player[npc.target].velocity / 2);
					Projectile.NewProjectile(npc.Center.X, npc.Center.Y, projVector.X, projVector.Y, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
					Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 17);
				}
				if (Main.rand.Next(200) == 1)
				{
					Vector2 projVector = UsefulFunctions.GenerateTargetingVector(npc.Center, Main.player[npc.target].Center, 5);
					projVector += Main.rand.NextVector2Circular(10, 10);
					Projectile.NewProjectile(npc.Center.X, npc.Center.Y, projVector.X, projVector.Y, ModContent.ProjectileType<Projectiles.Enemy.HypnoticDisrupter>(), hypnoticDisruptorDamage, 0f, Main.myPlayer);
					Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 17);
				}

				if (Main.rand.Next(900) == 0)
				{
					NPC.NewNPC((int)Main.player[this.npc.target].position.X - 636 - this.npc.width / 2, (int)Main.player[this.npc.target].position.Y - 216 - this.npc.width / 2, NPCID.CursedHammer, 0);
					NPC.NewNPC((int)Main.player[this.npc.target].position.X + 636 - this.npc.width / 2, (int)Main.player[this.npc.target].position.Y + 216 - this.npc.width / 2, NPCID.CursedHammer, 0);						
				}					
			}
			

			if (npc.justHit)
			{
				npc.ai[2] = 0f;
			}
			if (npc.ai[2] >= 0f)
			{
				int num258 = 16;
				bool flag26 = false;
				bool flag27 = false;
				if (npc.position.X > npc.ai[0] - (float)num258 && npc.position.X < npc.ai[0] + (float)num258)
				{
					flag26 = true;
				}
				else
				{
					if ((npc.velocity.X < 0f && npc.direction > 0) || (npc.velocity.X > 0f && npc.direction < 0))
					{
						flag26 = true;
					}
				}
				num258 += 24;
				if (npc.position.Y > npc.ai[1] - (float)num258 && npc.position.Y < npc.ai[1] + (float)num258)
				{
					flag27 = true;
				}
				if (flag26 && flag27)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 30f && num258 == 16)
					{
						flag25 = true;
					}
					if (npc.ai[2] >= 60f)
					{
						npc.ai[2] = -200f;
						npc.direction *= -1;
						npc.velocity.X = npc.velocity.X * -1f;
						npc.collideX = false;
					}
				}
				else
				{
					npc.ai[0] = npc.position.X;
					npc.ai[1] = npc.position.Y;
					npc.ai[2] = 0f;
				}
			}
			else
			{
				npc.ai[2] += 1f;
				if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) > npc.position.X + (float)(npc.width / 2))
				{
					npc.direction = -1;
				}
				else
				{
					npc.direction = 1;
				}
			}
			int num259 = (int)((npc.position.X + (float)(npc.width / 2)) / 16f) + npc.direction * 2;
			int num260 = (int)((npc.position.Y + (float)npc.height) / 16f);
			bool flag28 = true;
			//bool flag29; //What is this? It doesn't seem to do anything, so i'm commenting it out.
			int num261 = 3;
			for (int num269 = num260; num269 < num260 + num261; num269++)
			{
				if (Main.tile[num259, num269] == null)
				{
					Main.tile[num259, num269] = new Tile();
				}
				if ((Main.tile[num259, num269].active() && Main.tileSolid[(int)Main.tile[num259, num269].type]) || Main.tile[num259, num269].liquid > 0)
				{
				//	if (num269 <= num260 + 1)
				//	{
				//		flag29 = true;
				//	}
					flag28 = false;
					break;
				}
			}
			if (flag25)
			{
			//	flag29 = false;
				flag28 = true;
			}
			if (flag28)
			{
				npc.velocity.Y = npc.velocity.Y + 0.1f;
				if (npc.velocity.Y > 3f)
				{
					npc.velocity.Y = 3f;
				}
			}
			else
			{
				if (npc.directionY < 0 && npc.velocity.Y > 0f)
				{
					npc.velocity.Y = npc.velocity.Y - 0.1f;
				}
				if (npc.velocity.Y < -4f)
				{
					npc.velocity.Y = -4f;
				}
			}
			if (npc.collideX)
			{
				npc.velocity.X = npc.oldVelocity.X * -0.4f;
				if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 1f)
				{
					npc.velocity.X = 1f;
				}
				if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -1f)
				{
					npc.velocity.X = -1f;
				}
			}
			if (npc.collideY)
			{
				npc.velocity.Y = npc.oldVelocity.Y * -0.25f;
				if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
				{
					npc.velocity.Y = 1f;
				}
				if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
				{
					npc.velocity.Y = -1f;
				}
			}
			float num270 = 2f;
			if (npc.direction == -1 && npc.velocity.X > -num270)
			{
				npc.velocity.X = npc.velocity.X - 0.1f;
				if (npc.velocity.X > num270)
				{
					npc.velocity.X = npc.velocity.X - 0.1f;
				}
				else
				{
					if (npc.velocity.X > 0f)
					{
						npc.velocity.X = npc.velocity.X + 0.05f;
					}
				}
				if (npc.velocity.X < -num270)
				{
					npc.velocity.X = -num270;
				}
			}
			else
			{
				if (npc.direction == 1 && npc.velocity.X < num270)
				{
					npc.velocity.X = npc.velocity.X + 0.1f;
					if (npc.velocity.X < -num270)
					{
						npc.velocity.X = npc.velocity.X + 0.1f;
					}
					else
					{
						if (npc.velocity.X < 0f)
						{
							npc.velocity.X = npc.velocity.X - 0.05f;
						}
					}
					if (npc.velocity.X > num270)
					{
						npc.velocity.X = num270;
					}
				}
			}
			if (npc.directionY == -1 && (double)npc.velocity.Y > -1.5)
			{
				npc.velocity.Y = npc.velocity.Y - 0.04f;
				if ((double)npc.velocity.Y > 1.5)
				{
					npc.velocity.Y = npc.velocity.Y - 0.05f;
				}
				else
				{
					if (npc.velocity.Y > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + 0.03f;
					}
				}
				if ((double)npc.velocity.Y < -1.5)
				{
					npc.velocity.Y = -1.5f;
				}
			}
			else
			{
				if (npc.directionY == 1 && (double)npc.velocity.Y < 1.5)
				{
					npc.velocity.Y = npc.velocity.Y + 0.04f;
					if ((double)npc.velocity.Y < -1.5)
					{
						npc.velocity.Y = npc.velocity.Y + 0.05f;
					}
					else
					{
						if (npc.velocity.Y < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - 0.03f;
						}
					}
					if ((double)npc.velocity.Y > 1.5)
					{
						npc.velocity.Y = 1.5f;
					}
				}
			}
			Lighting.AddLight((int)npc.position.X / 16, (int)npc.position.Y / 16, 0.4f, 0f, 0.25f);
		}
		#endregion

		#region Frames
		public override void FindFrame(int currentFrame)
		{
			int num = 1;
			if (!Main.dedServ)
			{
				num = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
			}
			if (npc.velocity.X < 0)
			{
				npc.spriteDirection = -1;
			}
			else
			{
				npc.spriteDirection = 1;
			}
			npc.rotation = npc.velocity.X * 0.08f;
			npc.frameCounter += 1.0;
			if (npc.frameCounter >= 5.0)
			{
				npc.frame.Y = npc.frame.Y + num;
				npc.frameCounter = 0.0;
			}
			if (npc.frame.Y >= num * Main.npcFrameCount[npc.type])
			{
				npc.frame.Y = 0;
			}
			if (npc.ai[3] == 0)
			{
				npc.alpha = 0;
			}
			else
			{
				npc.alpha = 200;
			}
		}
		#endregion
		public override bool CheckActive()
		{
			return false;
		}
		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.GreaterHealingPotion;
		}
		public override void NPCLoot()
		{
			Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Water Fiend Kraken Gore 1"), 1f);
			Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Water Fiend Kraken Gore 2"), 1f);
			Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Water Fiend Kraken Gore 3"), 1f);
			Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Water Fiend Kraken Gore 4"), 1f);
			Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Water Fiend Kraken Gore 5"), 1f);
			Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Water Fiend Kraken Gore 6"), 1f);
			Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Water Fiend Kraken Gore 7"), 1f);
			Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Water Fiend Kraken Gore 8"), 1f);

			if (Main.expertMode)
			{
				npc.DropBossBags();
			}
			else
			{
				Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Accessories.DragonHorn>(), 1);
				Item.NewItem(npc.getRect(), ModContent.ItemType<Items.GuardianSoul>(), 1);
				Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Weapons.Melee.ForgottenRisingSun>(), 10);
				if (!tsorcRevampWorld.Slain.ContainsKey(npc.type))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<Items.DarkSoul>(), 30000);
				}
			}
		}
	}
}