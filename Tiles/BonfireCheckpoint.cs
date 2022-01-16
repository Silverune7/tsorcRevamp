﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using tsorcRevamp.UI;
using System.Collections.Generic;

namespace tsorcRevamp.Tiles
{
	public class BonfireCheckpoint : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 18 };
			animationFrameHeight = 74;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Bonfire Checkpoint");
			AddMapEntry(new Color(215, 60, 0), name);
			dustType = 30;
			disableSmartCursor = true;
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileBlockLight[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileWaterDeath[Type] = false;
			Main.tileLavaDeath[Type] = false;
			adjTiles = new int[] { TileID.Campfire };
		}
        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			Player player = Main.LocalPlayer;

			if (player.name == "Chroma TSORC test" || player.name == "Yournamehere") //feel free to add your players name, you can break it but has to be the top row of tiles for some reason
			{
				return true;
			}

			return false;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			if (Main.tile[i, j].frameY >= 74)
			{
				r = 0.7f;
				g = 0.4f;
				b = 0.1f;
			}
			else
			{
				r = 0.14f;
				g = 0.08f;
				b = 0.02f;
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<BonfirePlaceable>());
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			Player player = Main.LocalPlayer;
			var pos = new Vector2(i + 0.5f, j); // the + .5f makes the effect reach from equal distance to left and right
			var distance = Math.Abs(Vector2.Distance(player.Center, (pos * 16)));

			if (Main.tile[i, j].frameY >= 74 && distance <= 80f && !player.dead)
			{
				player.AddBuff(ModContent.BuffType<Buffs.Bonfire>(), 30);
			}

			if (player.whoAmI == Main.myPlayer && distance > 120f && distance < 300f)
            {
				int buffIndex = 0;

				foreach (int buffType in player.buffType)
				{

					if (buffType == ModContent.BuffType<Buffs.Bonfire>())
					{
						player.buffTime[buffIndex] = 0;
					}
					buffIndex++;
				}
			}
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			// Spend 5 ticks on each of 20 frames
			frameCounter++;
			if (frameCounter > 5)
			{
				frameCounter = 0;
				frame++;
				if (frame > 19)
				{
					frame = 0;
				}
			}
		}

		int bonfireEffectTimer = 0;
		int boneDustEffectTimer = 0;


        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Player player = Main.LocalPlayer;
			var pos = new Vector2(i + 0.5f, j); // the + .5f makes the effect reach from equal distance to left and right
			var distance = Math.Abs(Vector2.Distance(player.Center, (pos * 16)));

			if (player.velocity.X != 0 || player.velocity.Y != 0) //reset if player moves
			{
				bonfireEffectTimer = 0;
			}

			if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)))
			{
				short frameX = tile.frameX;
				short frameY = tile.frameY;

				if (tile.frameY >= 74 && /*player.velocity.X == 0 && player.velocity.Y == 0 &&*/ player.HasBuff(ModContent.BuffType<Buffs.Bonfire>()) && distance < 120f)
				{
					int style = frameY / 74; 

					if (frameY / 18 % 5 == 0 && frameX / 18 % 3 == 0) //this changes the height
					{

						if (player.velocity.X == 0 && player.velocity.Y == 0)
						{
							bonfireEffectTimer++;
							//Main.NewText("bonfireEffectTimer is at " + bonfireEffectTimer);
						}



						if (player.HasBuff(ModContent.BuffType<Buffs.Bonfire>()) && distance < 120f && player.HeldItem.type == ModContent.ItemType<Items.SublimeBoneDust>() && player.itemTime != 0)
						{
							boneDustEffectTimer++;
							if (boneDustEffectTimer == 1)
							{

								Main.PlaySound(SoundID.Item20, new Vector2(i * 16 + 25, j * 16 + 32));

								for (int q = 0; q < 30; q++)
								{
									int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, 270, 0f, 0f, 120, default(Color), 1f);
									Main.dust[z].noGravity = true;
									Main.dust[z].velocity *= 2.75f;
									Main.dust[z].fadeIn = 1.3f;
									Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
									vectorother.Normalize();
									vectorother *= (float)Main.rand.Next(50, 100) * 0.2f;
									Main.dust[z].velocity = vectorother;
									vectorother.Normalize();
									vectorother *= 10f;
									Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
								}

								for (int q = 0; q < 30; q++)
								{
									int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, 270, 0f, 0f, 120, default(Color), 1f);
									Main.dust[z].noGravity = true;
									Main.dust[z].velocity *= 2.75f;
									Main.dust[z].fadeIn = 1.3f;
									Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
									vectorother.Normalize();
									vectorother *= (float)Main.rand.Next(50, 100) * 0.12f;
									Main.dust[z].velocity = vectorother;
									vectorother.Normalize();
									vectorother *= 30f;
									Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
								}
							}
						}
                        else { boneDustEffectTimer = 0; }


						int dustChoice = -1;
						if (style >= 1)
						{
							dustChoice = 270; // A green dust.
						}

						// We can support different dust for different styles here
						if (dustChoice != -1)
						{

							#region Dusts

							if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)) && player.statLife < player.statLifeMax2)
							{
								if (!Main.npc.Any(n => n?.active == true && n.boss && n != Main.npc[200]) /*&& player.statLife < player.statLifeMax2*/) //only heal when no bosses are alive and when hp isn't full
								{
									if (bonfireEffectTimer > 0 && bonfireEffectTimer <= 12 && (player.velocity.X == 0 && player.velocity.Y == 0)) //wind up 1
									{

										if (Main.rand.Next(1) == 0) //was 1/6
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, dustChoice, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.06f; //speed
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 40f; //spawn distance
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}

										for (int q = 0; q < 3; q++)
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, dustChoice, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.015f;
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 10f;
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}
									}


									if (bonfireEffectTimer > 12 && bonfireEffectTimer <= 20 && (player.velocity.X == 0 && player.velocity.Y == 0)) //wind up 2
									{

										for (int q = 0; q < 1; q++)
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, 270, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.07f;
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 45f;
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}

										for (int q = 0; q < 5; q++)
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, 270, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.025f;
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 15f;
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}
									}


									if (bonfireEffectTimer > 20 && bonfireEffectTimer <= 28 && (player.velocity.X == 0 && player.velocity.Y == 0)) //wind up 3
									{

										for (int q = 0; q < 5; q++)
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, 270, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.08f;
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 50f;
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}

										for (int q = 0; q < 10; q++)
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, 270, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.035f;
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 20f;
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}
									}


									if (bonfireEffectTimer == 28 && (player.velocity.X == 0 && player.velocity.Y == 0))//blast
									{
										Main.PlaySound(SoundID.Item20, new Vector2(i * 16 + 25, j * 16 + 32));

										for (int q = 0; q < 30; q++)
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, dustChoice, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.2f;
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 10f;
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}

										for (int q = 0; q < 30; q++)
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, dustChoice, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.12f;
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 30f;
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}
									}


									if (bonfireEffectTimer > 28 && (player.velocity.X == 0 && player.velocity.Y == 0))//full effect
									{

										for (int q = 0; q < 5; q++)
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, 270, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.085f;
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 55f;
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}

										for (int q = 0; q < 17; q++)
										{
											int z = Dust.NewDust(new Vector2(i * 16 + 25, j * 16 + 32), 40, 56, 270, 0f, 0f, 120, default(Color), 1f);
											Main.dust[z].noGravity = true;
											Main.dust[z].velocity *= 2.75f;
											Main.dust[z].fadeIn = 1.3f;
											Vector2 vectorother = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
											vectorother.Normalize();
											vectorother *= (float)Main.rand.Next(50, 100) * 0.045f;
											Main.dust[z].velocity = vectorother;
											vectorother.Normalize();
											vectorother *= 25f;
											Main.dust[z].position = new Vector2(i * 16 + 25, j * 16 + 32) - vectorother;
										}
									}
								}
							}
							#endregion

						}
					}
				}
			}

			Texture2D texture;
			if (Main.canDrawColorTile(i, j))
			{
				texture = Main.tileAltTexture[Type, (int)tile.color()];
			}
			else
			{
				texture = Main.tileTexture[Type];
			}
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			int height = tile.frameY % animationFrameHeight == 54 ? 18 : 16;
			int animate = 0;
			if (tile.frameY >= 74)
			{
				animate = Main.tileFrame[Type] * animationFrameHeight;
			}
			Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.frameX, tile.frameY + animate, 16, height), Lighting.GetColor(i, j), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(mod.GetTexture("Tiles/BonfireCheckpoint_Glow"), new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.frameX, tile.frameY + animate, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			return false;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.showItemIcon = true;
			player.showItemIcon2 = ModContent.ItemType<BonfirePlaceable>();
		}

		public override bool NewRightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			if (tile.frameY / 74 == 0)
			{
				Main.PlaySound(SoundID.Item20, new Vector2(i * 16, j * 16));
				Main.NewText("Bonfire lit!", 250, 110, 90);

				if (tsorcRevampWorld.LitBonfireList == null)
				{
					tsorcRevampWorld.LitBonfireList = new List<Vector2>();
				}

				tsorcRevampWorld.LitBonfireList.Add(new Vector2(i, j));

				int x = i - Main.tile[i, j].frameX / 18 % 3; // 16 pixels in a block + 2 pixels for the buffer. 3 because its 3 blocks wide
				int y = j - Main.tile[i, j].frameY / 18 % 4; // 4 because it is 4 blocks tall
				for (int l = x; l < x + 3; l++)             // this chunk of code basically makes it so that when you right click one tile, 
				{              // because 3x4 tile         // it counts as the whole 3x4 tile, not 4 individual tiles that can all be clicked
					for (int m = y; m < y + 4; m++)         //Code taken from VoidMonolith - example mod
					{
						if (Main.tile[l, m] == null)
						{
							Main.tile[l, m] = new Tile();
						}
						if (Main.tile[l, m].active() && Main.tile[l, m].type == Type)
						{
							if (Main.tile[l, m].frameY < 74)
							{
								Main.tile[l, m].frameY += 74;
							}
							else
							{
								Main.tile[l, m].frameY -= 74;
							}
						}
					}
				}
			}
			else if (tile.frameY / 74 >= 1)
			{
				//Main.NewText("Open/Close UI", 250, 80, 60);
				if (!BonfireUIState.Visible)
				{
					BonfireUIState.Visible = true;
					Main.PlaySound(SoundID.MenuOpen);

				}
				else
				{
					BonfireUIState.Visible = false;
					Main.PlaySound(SoundID.MenuClose);
				}
			}
			return true;
		}

		public class BonfirePlaceable : ModItem
		{
			public override void SetStaticDefaults()
			{
				DisplayName.SetDefault("Dark Souls Bonfire");
				Tooltip.SetDefault("Right-click to light" +
				"\nYou probably shouldn't have this" +
				"\nCan only be placed by devs");
			}

			public override void SetDefaults()
			{
				item.CloneDefaults(ItemID.Bookcase);
				item.createTile = ModContent.TileType<BonfireCheckpoint>();
				item.placeStyle = 0;
			}

			public override bool CanUseItem(Player player)
			{
				if (player.name == "Chroma TSORC test" || player.name == "Yournamehere") //feel free to add your players name
				{
					return true;
				}

				return false;

			}
		}
	}
}