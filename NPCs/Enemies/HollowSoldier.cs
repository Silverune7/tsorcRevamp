﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Accessories.Defensive.Shields;
using tsorcRevamp.Utilities;
using static tsorcRevamp.SpawnHelper;

namespace tsorcRevamp.NPCs.Enemies
{
    public class HollowSoldier : ModNPC //Don't look at the code, it's muy malo. Look at Lothric Spear Knight for a better example code management-wise
    {
        //AI 
        bool slashing = false;
        bool jumpSlashing = false;
        bool shielding = false;


        //Anim
        int shieldFrame;
        int shieldAnimTimer;
        bool countingUP = false;

        public int hollowLesserSlashDamage = 17;
        public int hollowGreaterSlashDamage = 20;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 17;
        }
        public override void SetDefaults()
        {
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            NPC.damage = 32;
            NPC.defense = 20;
            NPC.height = 40;
            NPC.width = 20;
            NPC.lifeMax = 250;
            if (Main.hardMode)
            {
                NPC.lifeMax = 500;
                NPC.defense = 30;
                NPC.damage = 42;
                NPC.value = 2500; // was 150
                hollowLesserSlashDamage = 26;
                hollowGreaterSlashDamage = 30; // scaling damage added
            }
            if (tsorcRevampWorld.SuperHardMode)
            {
                NPC.lifeMax = 1500;
                NPC.defense = 70;
                NPC.damage = 58;
                NPC.value = 6000; //was 250, now has scaling damage
                hollowLesserSlashDamage = 30;
                hollowGreaterSlashDamage = 34;
            }
            NPC.value = 1250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Banners.HollowSoldierBanner>();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int DustType = 5;
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustType);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X += Main.rand.Next(-50, 51) * 0.04f;
                dust.velocity.Y += Main.rand.Next(-50, 51) * 0.04f;
                dust.scale *= .8f + Main.rand.Next(-30, 31) * 0.01f;
                dust.noGravity = true;
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 80; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 54, 2.5f * hit.HitDirection, -1.5f, 70, default(Color), 1f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, 1.5f * hit.HitDirection, -2.5f, 50, default(Color), 1f);
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            int lifePercentage = (NPC.life * 100) / NPC.lifeMax;
            float acceleration = 0.02f;
            //float top_speed = (lifePercentage * 0.02f) + .2f; //good calculation to remember for decreasing speed the lower the enemy HP%
            float top_speed = (lifePercentage * -0.015f) + 2f; //good calculation to remember for increasing speed the lower the enemy HP%
            float braking_power = 0.1f; //Breaking power to slow down after moving above top_speed
                                        //Main.NewText(Math.Abs(npc.velocity.X));

            #region target/face player, respond to boredom

            /*if (!jumpSlashing && !slashing)
            {
                npc.TargetClosest(true); //  Target the closest player & face him (If passed as a parameter, a bool will determine whether it should face the target or not)
            }

            if (npc.velocity.X == 0f && !jumpSlashing && !shielding && !slashing)
            {
                if (npc.velocity.Y == 0f)
                { // not moving
                    if (npc.ai[0] == 0f)
                        npc.ai[0] = 1f; // facing change delay
                    else
                    { // change movement and facing direction, reset delay
                        npc.direction *= -1;
                        npc.spriteDirection = npc.direction;
                        npc.ai[0] = 0f;
                    }
                }
            }
            else // moving in x direction,
                npc.ai[0] = 0f; // reset facing change delay

            if (npc.direction == 0) // what does it mean if direction is 0?
                npc.direction = 1; // flee right if direction not set? or is initial direction?*/

            if (NPC.ai[0] == 0 && !jumpSlashing && !slashing)
            {
                NPC.TargetClosest(true); //  Target the closest player & face him (If passed as a parameter, a bool will determine whether it should face the target or not)
            }

            if (NPC.velocity.X == 0 && !jumpSlashing && !shielding && !slashing)
            {
                NPC.ai[0]++;
                if (NPC.ai[0] > 120 && NPC.velocity.Y == 0)
                {
                    NPC.direction *= -1;
                    NPC.spriteDirection = NPC.direction;
                    NPC.ai[0] = 50;
                }
            }

            if (Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
            {
                NPC.ai[0] = 0;
            }

            #endregion

            #region melee movement

            if (NPC.ai[1] >= 390 && NPC.ai[1] <= 420)
            {
                top_speed = (lifePercentage * -0.015f) + 2.5f; //good calculation to remember for increasing speed the lower the enemy HP%
            }

            if (Math.Abs(NPC.velocity.X) > top_speed && NPC.velocity.Y == 0)
            {
                NPC.velocity *= (1f - braking_power); //breaking
            }
            if (NPC.velocity.X > 6f) //hard limit of 8f
            {
                NPC.velocity.X = 6f;
            }
            if (NPC.velocity.X < -6f) //both directions
            {
                NPC.velocity.X = -6f;
            }
            else
            {
                NPC.velocity.X += NPC.direction * acceleration; //accelerating
            }

            //breaking power after turning, to turn fast or to "slip"
            if (NPC.direction == 1)
            {
                if (NPC.velocity.X > -top_speed)
                {
                    NPC.velocity.X += 0.085f;
                }
                NPC.netUpdate = true;
            }
            if (NPC.direction == -1)
            {
                if (NPC.velocity.X < top_speed)
                {
                    NPC.velocity.X += -0.085f;
                }
                NPC.netUpdate = true;
            }


            if (Math.Abs(NPC.velocity.X) > 4f) //If moving at high speed, become knockback immune
            {
                NPC.knockBackResist = 0;
            }
            if (Math.Abs(NPC.velocity.Y) > 0.1f) //If moving vertically, become knockback immune
            {
                NPC.knockBackResist = 0;
            }

            else
            {
                NPC.knockBackResist = 0.2f; //If not moving at high speed, default back to taking some knockback
            }

            NPC.noTileCollide = false;

            int y_below_feet = (int)(NPC.position.Y + (float)NPC.height + 8f) / 16;
            if (Main.tile[(int)NPC.position.X / 16, y_below_feet].TileType == TileID.Platforms && Main.tile[(int)(NPC.position.X + (float)NPC.width) / 16, y_below_feet].TileType == TileID.Platforms && NPC.position.Y < (player.position.Y - 4 * 16))
            {
                NPC.noTileCollide = true;
            }


            #endregion

            #region check if standing on a solid tile
            bool standing_on_solid_tile = false;
            if (NPC.velocity.Y == 0f) // no jump/fall
            {
                int x_left_edge = (int)NPC.position.X / 16;
                int x_right_edge = (int)(NPC.position.X + (float)NPC.width) / 16;
                for (int l = x_left_edge; l <= x_right_edge; l++) // check every block under feet
                {
                    if (Main.tile[l, y_below_feet] == null) // null tile means ??
                        return;

                    if (Main.tile[l, y_below_feet].HasTile && Main.tileSolid[(int)Main.tile[l, y_below_feet].TileType]) // tile exists and is solid
                    {
                        standing_on_solid_tile = true;
                        break; // one is enough so stop checking
                    }
                } // END traverse blocks under feet
            } // END no jump/fall
            #endregion

            #region new Tile()s, jumping
            if (standing_on_solid_tile && !slashing && !shielding && !jumpSlashing)  //  if standing on solid tile
            {
                int x_in_front = (int)((NPC.position.X + (float)(NPC.width / 2) + (float)(15 * NPC.direction)) / 16f); // 15 pix in front of center of mass
                int y_above_feet = (int)((NPC.position.Y + (float)NPC.height - 15f) / 16f); // 15 pix above feet

                if (NPC.position.Y > player.position.Y + 3 * 16 && Math.Abs(NPC.Center.X - player.Center.X) < 4f * 16 && Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
                {
                    slashing = true;
                    NPC.ai[3] = 20;
                    NPC.velocity.Y = -8f; // jump with power 8 if directly under player
                    NPC.netUpdate = true;
                }

                if (Main.tile[x_in_front, y_above_feet] == null)
                {
                    Main.tile[x_in_front, y_above_feet].ClearTile();
                }

                if (Main.tile[x_in_front, y_above_feet - 1] == null)
                {
                    Main.tile[x_in_front, y_above_feet - 1].ClearTile();
                }

                if (Main.tile[x_in_front, y_above_feet - 2] == null)
                {
                    Main.tile[x_in_front, y_above_feet - 2].ClearTile();
                }

                if (Main.tile[x_in_front, y_above_feet - 3] == null)
                {
                    Main.tile[x_in_front, y_above_feet - 3].ClearTile();
                }

                if (Main.tile[x_in_front, y_above_feet + 1] == null)
                {
                    Main.tile[x_in_front, y_above_feet + 1].ClearTile();
                }
                //  create? 2 other tiles farther in front
                if (Main.tile[x_in_front + NPC.direction, y_above_feet - 1] == null)
                {
                    Main.tile[x_in_front + NPC.direction, y_above_feet - 1].ClearTile();
                }

                if (Main.tile[x_in_front + NPC.direction, y_above_feet + 1] == null)
                {
                    Main.tile[x_in_front + NPC.direction, y_above_feet + 1].ClearTile();
                }

                else // standing on solid tile but not in front of a passable door
                {
                    if ((NPC.velocity.X < 0f && NPC.spriteDirection == -1) || (NPC.velocity.X > 0f && NPC.spriteDirection == 1))
                    {  //  moving forward
                        if (Main.tile[x_in_front, y_above_feet - 2].HasTile && Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet - 2].TileType])
                        { // 3 blocks above ground level(head height) blocked
                            if (Main.tile[x_in_front, y_above_feet - 3].HasTile && Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet - 3].TileType])
                            { // 4 blocks above ground level(over head) blocked
                                NPC.velocity.Y = -8f; // jump with power 8 (for 4 block steps)
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.velocity.Y = -7f; // jump with power 7 (for 3 block steps)
                                NPC.netUpdate = true;
                            }
                        } // for everything else, head height clear:
                        else if (Main.tile[x_in_front, y_above_feet - 1].HasTile && Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet - 1].TileType])
                        { // 2 blocks above ground level(mid body height) blocked
                            NPC.velocity.Y = -6f; // jump with power 6 (for 2 block steps)
                            NPC.netUpdate = true;
                        }
                        else if (Main.tile[x_in_front, y_above_feet].HasTile && Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet].TileType])
                        { // 1 block above ground level(foot height) blocked
                            NPC.velocity.Y = -5f; // jump with power 5 (for 1 block steps)
                            NPC.netUpdate = true;
                        }
                        else if (NPC.directionY < 0 && (!Main.tile[x_in_front, y_above_feet + 1].HasTile || !Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet + 1].TileType]) && (!Main.tile[x_in_front + NPC.direction, y_above_feet + 1].HasTile || !Main.tileSolid[(int)Main.tile[x_in_front + NPC.direction, y_above_feet + 1].TileType]))
                        { // rising? & jumps gaps & no solid tile ahead to step on for 2 spaces in front
                            NPC.velocity.Y = -8f; // jump with power 8
                            NPC.velocity.X = NPC.velocity.X * 1.5f; // jump forward hard as well; we're trying to jump a gap
                            NPC.netUpdate = true;
                        }
                    } // END moving forward, still: standing on solid tile but not in front of a passable door
                }
            }

            #endregion

            #region attacks


            //Basic Slash Attack
            //Main.NewText(npc.ai[1]);
            //Main.NewText(npc.ai[2]);
            //Main.NewText(npc.ai[3]);
            // Main.NewText(top_speed);
            //Main.NewText(Math.Abs(npc.velocity.X));

            if (NPC.ai[3] < 10)
            {
                ++NPC.ai[3]; //Used for Basic Slash
            }

            if (/*!shielding && */!jumpSlashing)
            {
                if (NPC.ai[3] == 10 && NPC.Distance(player.Center) < 50 && Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
                {
                    slashing = true;
                    shielding = false;
                }

                if (slashing)
                {
                    ++NPC.ai[3];

                    if (NPC.ai[3] < 26)
                    {
                        if (NPC.direction == 1)
                        {
                            NPC.velocity.X -= 0.25f;
                            if (NPC.velocity.X < 0)
                            {
                                NPC.velocity.X = 0;
                            }
                        }

                        else
                        {
                            NPC.velocity.X += 0.25f;
                            if (NPC.velocity.X > 0)
                            {
                                NPC.velocity.X = 0;
                            }
                        }
                    }

                    if (NPC.ai[3] == 26) //If timer is 46
                    {

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item1 with { PitchVariance = .3f }, NPC.Center); //Play slash/swing sound

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (NPC.direction == 1)
                            {
                                if (!standing_on_solid_tile)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(14, -60), new Vector2(0, 4f), ModContent.ProjectileType<Projectiles.Enemy.MediumWeaponSlash>(), hollowLesserSlashDamage, 5, Main.myPlayer, NPC.whoAmI, 0);
                                }
                                else
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(14, -20), new Vector2(0, 4f), ModContent.ProjectileType<Projectiles.Enemy.MediumWeaponSlash>(), hollowLesserSlashDamage, 5, Main.myPlayer, NPC.whoAmI, 0);
                                }
                            }

                            else
                            {
                                if (!standing_on_solid_tile)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(-10, -60), new Vector2(0, 4f), ModContent.ProjectileType<Projectiles.Enemy.MediumWeaponSlash>(), hollowLesserSlashDamage, 5, Main.myPlayer, NPC.whoAmI, 0);
                                }
                                else
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(-10, -20), new Vector2(0, 4f), ModContent.ProjectileType<Projectiles.Enemy.MediumWeaponSlash>(), hollowLesserSlashDamage, 5, Main.myPlayer, NPC.whoAmI, 0);
                                }
                            }
                        } 
                    }

                    if (NPC.ai[3] >= 49) //If timer is 69
                    {
                        slashing = false;
                        NPC.ai[3] = 0; //Reset timer
                    }
                }
            }




            //Telegraphed Jump-slash

            if (NPC.ai[1] < 420)
            {
                ++NPC.ai[1]; //Used for Jump-slash
            }

            if (NPC.ai[1] >= 390 && NPC.ai[1] <= 400)
            {
                if (NPC.direction == 1) //Large eye dust to warn player that a jump-slash is ready...
                {
                    Dust dust2 = Main.dust[Dust.NewDust(new Vector2(NPC.position.X + 9, NPC.position.Y + 1), 4, 4, 183, NPC.velocity.X, NPC.velocity.Y, 180, default(Color), 1.5f)];
                    //dust2.velocity *= 0f;
                    dust2.noGravity = true;
                    dust2.fadeIn = .3f;
                    dust2.velocity += NPC.velocity;
                }

                else
                {
                    Dust dust2 = Main.dust[Dust.NewDust(new Vector2(NPC.position.X + 3, NPC.position.Y + 1), 4, 4, 183, NPC.velocity.X, NPC.velocity.Y, 180, default(Color), 1.5f)];
                    //dust2.velocity *= 0f;
                    dust2.noGravity = true;
                    dust2.fadeIn = .3f;
                    dust2.velocity += NPC.velocity;
                }

            }

            if (NPC.ai[1] >= 400 && NPC.ai[1] < 442)
            {
                if (NPC.direction == 1) //Small eye dust to warn player that a jump-slash is ready...
                {
                    Dust dust2 = Main.dust[Dust.NewDust(new Vector2(NPC.position.X + 9, NPC.position.Y + 1), 4, 4, 183, NPC.velocity.X, NPC.velocity.Y, 180, default(Color), 0.8f)];
                    //dust2.velocity *= 0f;
                    dust2.noGravity = true;
                    dust2.fadeIn = .3f;
                    dust2.velocity += NPC.velocity;
                }

                else
                {
                    Dust dust2 = Main.dust[Dust.NewDust(new Vector2(NPC.position.X + 3, NPC.position.Y + 1), 4, 4, 183, NPC.velocity.X, NPC.velocity.Y, 180, default(Color), 0.8f)];
                    //dust2.velocity *= 0f;
                    dust2.noGravity = true;
                    dust2.fadeIn = .3f;
                    dust2.velocity += NPC.velocity;
                }
            }

            if (/*!shielding &&*/ !slashing)
            {
                if (NPC.ai[1] == 420 && NPC.Distance(player.Center) < 140 && NPC.Distance(player.Center) >= 50 && NPC.velocity.Y == 0 && standing_on_solid_tile && Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0)) //If timer is at 0 and player is within slash range
                {
                    jumpSlashing = true;
                    shielding = false;
                }

                if (jumpSlashing)
                {
                    ++NPC.ai[1];
                    if (NPC.ai[1] < 436)
                    {
                        if (NPC.direction == 1)
                        {
                            NPC.velocity.X -= 0.15f;
                            if (NPC.velocity.X < 0)
                            {
                                NPC.velocity.X = 0;
                            }
                        }

                        else
                        {
                            NPC.velocity.X += 0.15f;
                            if (NPC.velocity.X > 0)
                            {
                                NPC.velocity.X = 0;
                            }
                        }
                    }

                    if (NPC.ai[1] == 436) //If timer is 46
                    {
                        if (NPC.direction == 1)
                        {
                            NPC.velocity.X += 5f;
                            NPC.velocity.Y -= 3f;
                        }

                        else
                        {
                            NPC.velocity.X -= 5f;
                            NPC.velocity.Y -= 3f;
                        }
                    }

                    if (NPC.ai[1] == 442) //If timer is 50
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item1 with { PitchVariance = .3f }, NPC.Center); //Play slash/swing sound

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (NPC.direction == 1)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(24, -20), new Vector2(0, 4f), ModContent.ProjectileType<Projectiles.Enemy.MediumWeaponSlash>(), hollowGreaterSlashDamage, 5, Main.myPlayer, NPC.whoAmI, 0);
                            }

                            else
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(-8, -20), new Vector2(0, 4f), ModContent.ProjectileType<Projectiles.Enemy.MediumWeaponSlash>(), hollowGreaterSlashDamage, 5, Main.myPlayer, NPC.whoAmI, 0);
                            }
                        }                         
                    }
                    if (NPC.ai[1] > 470 && NPC.ai[1] < 510)
                    {
                        if (NPC.direction == 1)
                        {
                            NPC.velocity.X -= 0.3f;
                            if (NPC.velocity.X < 0)
                            {
                                NPC.velocity.X = 0;
                            }
                        }

                        else
                        {
                            NPC.velocity.X += 0.3f;
                            if (NPC.velocity.X > 0)
                            {
                                NPC.velocity.X = 0;
                            }
                        }
                    }
                    if (NPC.ai[1] >= 510) //If timer is 69
                    {
                        jumpSlashing = false;
                        NPC.ai[1] = 0; //Reset timer
                    }
                }
            }




            //Shielding

            if (shielding || NPC.Distance(player.Center) < 220 || NPC.ai[2] > 300)
            {
                NPC.ai[2]++;

                if (!jumpSlashing && !slashing && NPC.velocity.Y == 0)
                {
                    if (NPC.ai[2] > 300 && NPC.ai[2] <= 310)
                    {
                        if (NPC.direction == 1) { NPC.velocity.X -= 0.15f; }
                        else { NPC.velocity.X += 0.15f; }
                    }

                    if (NPC.ai[2] > 310)
                    {
                        NPC.velocity.X = 0;
                        shielding = true;
                    }

                    if (NPC.ai[2] > 500)
                    {
                        shielding = false;
                        NPC.ai[2] = 0;
                    }
                }
            }
            #endregion
        }



        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            //("NPCs/Enemies/HollowSoldier_Shield");
            Texture2D shieldTexture = (Texture2D)Mod.Assets.Request<Texture2D>("NPCs/Enemies/HollowSoldier_Shield");
            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle myrectangle = shieldTexture.Frame(1, 15, 0, shieldFrame);
            if (shielding && NPC.velocity.X == 0 && !jumpSlashing && !slashing)
            {
                if (NPC.spriteDirection == 1)
                {
                    spriteBatch.Draw(shieldTexture, NPC.Center - Main.screenPosition, myrectangle, lightColor, NPC.rotation, new Vector2(34, 27), NPC.scale, effects, 0f);
                }
                else
                {
                    spriteBatch.Draw(shieldTexture, NPC.Center - Main.screenPosition, myrectangle, lightColor, NPC.rotation, new Vector2(34, 27), NPC.scale, effects, 0f);
                }
            }
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (shielding)
            {
                if (NPC.ai[1] < 370)
                {
                    NPC.ai[1] += 50; //Used for Jump-slash
                }
                if (NPC.direction == 1)
                {
                    if (player.position.X > NPC.position.X)
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit4 with { PitchVariance = 0.3f }, NPC.Center); //Play metal tink sound
                        modifiers.SourceDamage.Flat -= 30;
                        if (NPC.ai[2] > 350)
                        {
                            NPC.ai[2] -= 25;
                        }
                    }
                }
                else
                {
                    if (player.position.X < NPC.position.X)
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit4 with { PitchVariance = 0.3f }, NPC.Center); //Play metal tink sound
                        modifiers.SourceDamage.Flat -= 30;
                        if (NPC.ai[2] > 350)
                        {
                            NPC.ai[2] -= 25;
                        }
                    }
                }
            }

            if (NPC.direction == 1) //if enemy facing right
            {
                if (player.position.X < NPC.position.X) //if hit in the back
                {
                    CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Bottom.Y, 10, 10), Color.Crimson, LangUtils.GetTextValue("NPCs.WeakSpot"), false, false);
                    modifiers.FinalDamage *= 2; //bonus damage
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit18 with { PitchVariance = 0.3f }, NPC.Center); //Play fleshy sound
                }
            }
            else //if enemy facing left
            {
                if (player.position.X > NPC.position.X) //if hit in the back
                {
                    CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Bottom.Y, 10, 10), Color.Crimson, LangUtils.GetTextValue("NPCs.WeakSpot"), false, false);
                    modifiers.FinalDamage *= 2; //bonus damage
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit18 with { PitchVariance = 0.3f }, NPC.Center); //Play fleshy sound
                }
            }

            NPC.ai[2] += 10;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[NPC.target];
            if (projectile.type != ModContent.ProjectileType<Items.Weapons.Ranged.Specialist.BlizzardBlasterShot>())
            {
                if (shielding)
                {
                    if (NPC.direction == 1) //if npc facing right
                    {
                        if (projectile.oldPosition.X > NPC.Center.X && projectile.DamageType == DamageClass.Melee && projectile.aiStyle != 19) //if proj moving toward npc front
                        {

                            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit4 with { PitchVariance = 0.3f }, NPC.Center); //Play metal tink sound
                            modifiers.SourceDamage.Flat -= 30;
                            modifiers.Knockback *= 0.1f;
                            if (NPC.ai[1] < 350)
                            {
                                NPC.ai[1] += 50; //Used for Jump-slash
                            }
                            if (NPC.ai[2] > 350)
                            {
                                NPC.ai[2] -= 25;
                            }
                        }

                        else if (modifiers.HitDirection == -1 && (projectile.DamageType != DamageClass.Melee || projectile.aiStyle == 19))
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit4 with { PitchVariance = 0.3f }, NPC.Center); //Play metal tink sound
                            modifiers.SourceDamage.Flat -= 30;
                            modifiers.Knockback *= 0f;

                            if (NPC.ai[1] < 350)
                            {
                                NPC.ai[1] += 60; //Used for Jump-slash
                            }


                            if (NPC.ai[2] > 350)
                            {
                                NPC.ai[2] -= 25;
                            }
                        }
                    }
                    else //if npc facing left
                    {
                        if (projectile.oldPosition.X < NPC.Center.X && projectile.DamageType == DamageClass.Melee && projectile.aiStyle != 19) //if proj moving toward npc front
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit4 with { PitchVariance = 0.3f }, NPC.Center); //Play metal tink sound
                            modifiers.SourceDamage.Flat -= 30;
                            modifiers.Knockback *= 0.1f;
                            if (NPC.ai[1] < 350)
                            {
                                NPC.ai[1] += 50; //Used for Jump-slash
                            }
                            if (NPC.ai[2] > 350)
                            {
                                NPC.ai[2] -= 25;
                            }
                        }
                        else if (modifiers.HitDirection == 1 && (projectile.DamageType != DamageClass.Melee || projectile.aiStyle == 19))
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit4 with { PitchVariance = 0.3f }, NPC.Center); //Play metal tink sound
                            modifiers.SourceDamage.Flat -= 30;

                            modifiers.Knockback *= 0f;
                            if (NPC.ai[1] < 350)
                            {
                                NPC.ai[1] += 60; //Used for Jump-slash
                            }


                            if (NPC.ai[2] > 350)
                            {
                                NPC.ai[2] -= 25;
                            }
                        }
                    }
                }


                if (NPC.direction == 1) //if enemy facing right
                {
                    if (projectile.oldPosition.X < NPC.Center.X && projectile.DamageType == DamageClass.Melee && projectile.aiStyle != 19) //if hit in the back
                    {
                        CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Bottom.Y, 10, 10), Color.Crimson, LangUtils.GetTextValue("NPCs.WeakSpot"), false, false);
                        modifiers.FinalDamage *= 2; //bonus damage
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit18 with { PitchVariance = 0.3f }, NPC.Center); //Play fleshy sound
                    }
                    else if (modifiers.HitDirection == 1)
                    {
                        CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Bottom.Y, 10, 10), Color.Crimson, LangUtils.GetTextValue("NPCs.WeakSpot"), false, false);
                        modifiers.FinalDamage *= 2; //bonus damage
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit18 with { PitchVariance = 0.3f }, NPC.Center); //Play fleshy sound
                    }
                }
                else //if enemy facing left
                {
                    if (projectile.oldPosition.X > NPC.Center.X && projectile.DamageType == DamageClass.Melee && projectile.aiStyle != 19) //if hit in the back
                    {
                        CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Bottom.Y, 10, 10), Color.Crimson, LangUtils.GetTextValue("NPCs.WeakSpot"), false, false);
                        modifiers.FinalDamage *= 2; //bonus damage
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit18 with { PitchVariance = 0.3f }, NPC.Center); //Play fleshy sound
                    }
                    else if (modifiers.HitDirection == -1)
                    {
                        CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Bottom.Y, 10, 10), Color.Crimson, LangUtils.GetTextValue("NPCs.WeakSpot"), false, false);
                        modifiers.FinalDamage *= 2; //bonus damage
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit18 with { PitchVariance = 0.3f }, NPC.Center); //Play fleshy sound
                    }
                }

                if (NPC.Distance(player.Center) > 220 && !shielding)
                {
                    NPC.ai[2] += 120;
                }

                if (NPC.ai[1] < 340)
                {
                    NPC.ai[1] += 10;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float chance = 0;
            bool FrozenOcean = spawnInfo.SpawnTileX > (Main.maxTilesX - 800);
            bool Ocean = spawnInfo.SpawnTileX < 800 || FrozenOcean;

            Player p = spawnInfo.Player;
            if (spawnInfo.Invasion || Sky(p) || spawnInfo.Player.ZoneSnow)
            {
                chance = 0;
                return chance;
            }
            if (spawnInfo.Water) return 0f;
            if (spawnInfo.Player.ZoneGlowshroom) return 0f;

            if (spawnInfo.Player.townNPCs > 1f) return 0f;

            if (!Main.hardMode && spawnInfo.SpawnTileType == TileID.GreenDungeonBrick && !spawnInfo.Water) return 0.12f;
            if (!spawnInfo.Water && (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].WallType == WallID.GreenDungeonSlabUnsafe || Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].WallType == WallID.GreenDungeonUnsafe) && !Main.hardMode && !tsorcRevampWorld.SuperHardMode) return 0.12f;
            if (Main.hardMode && spawnInfo.Lihzahrd) return 0.2f;
            if (Main.hardMode && p.ZoneNormalCaverns && !spawnInfo.Water) return 0.02f;
            if (Main.hardMode && p.ZoneDesert && p.ZoneOverworldHeight && !spawnInfo.Water) return 0.05f;
            if (Main.hardMode && p.ZoneUndergroundDesert && !spawnInfo.Water) return 0.07f;
            if (Main.hardMode && spawnInfo.SpawnTileType == TileID.BlueDungeonBrick && !spawnInfo.Water) return 0.18f;
            if (Main.hardMode && spawnInfo.SpawnTileType == TileID.TungstenBrick && !spawnInfo.Water) return 0.15f;

            if (tsorcRevampWorld.SuperHardMode && !(Ocean || spawnInfo.Player.ZoneJungle || spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson || spawnInfo.Player.ZoneUnderworldHeight)) return 0.23f;
            if (tsorcRevampWorld.SuperHardMode && spawnInfo.Player.ZoneOverworldHeight && !(Ocean || spawnInfo.Player.ZoneJungle || spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson)) return 0.25f;
            if (tsorcRevampWorld.SuperHardMode && spawnInfo.Player.ZoneDesert) return 0.13f;
            if (tsorcRevampWorld.SuperHardMode && spawnInfo.Player.ZoneDungeon && !spawnInfo.Player.ZoneUnderworldHeight) return 0.16f; //.08% is 4.28%

            if (Main.expertMode && Main.bloodMoon && spawnInfo.Player.ZoneOverworldHeight && (NPC.downedBoss2 || NPC.downedBoss3)) return chance = 0.03f;

            if (Main.expertMode && Main.bloodMoon && (NPC.downedBoss2 || NPC.downedBoss3)) return chance = 0.03f;

            if ((NPC.downedBoss2 || NPC.downedBoss3) && spawnInfo.Player.ZoneOverworldHeight && Main.dayTime && !(Ocean || spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson)) return chance = 0.035f;
            if ((NPC.downedBoss2 || NPC.downedBoss3) && spawnInfo.Player.ZoneOverworldHeight && !Main.dayTime && !(Ocean || spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson)) return chance = 0.075f;

            if ((NPC.downedBoss2 || NPC.downedBoss3) && (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight) && Main.dayTime && !(spawnInfo.Player.ZoneJungle || spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson)) return chance = 0.06f;
            if ((NPC.downedBoss2 || NPC.downedBoss3) && (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight) && !Main.dayTime && !(spawnInfo.Player.ZoneJungle || spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson)) return chance = 0.08f;

            if (NPC.downedBoss2 || NPC.downedBoss3 && !(Ocean || spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson)) return chance = 0.025f;

            return chance;

        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.SoulCoin>(), 1, 3, 9));
            npcLoot.Add(ItemDropRule.Common(ItemID.EndurancePotion, 30));
            npcLoot.Add(ItemDropRule.Common(ItemID.IronskinPotion, 30));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.FadingSoul>(), 10));
            //npcLoot.Add(ItemDropRule.Common(ItemID.CobaltShield, 15));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IronShield>(), 15));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Potions.Lifegem>(), 15));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Potions.RadiantLifegem>(), 30));

            int[] armorIDs = new int[] {
                ModContent.ItemType<Items.Armors.Magic.RedClothHat>(),
                ModContent.ItemType<Items.Armors.Magic.RedClothTunic>(),
                ModContent.ItemType<Items.Armors.Magic.RedClothPants>(),
            };
            npcLoot.Add(new DropMultiple(armorIDs, 30, 1, !NPC.downedBoss1));
        }

        #region Drawing and Animation


        public override void FindFrame(int frameHeight)
        {
            //Main.NewText(shieldAnimTimer);
            //Main.NewText(shieldFrame);

            if (NPC.velocity.X != 0) //Walking
            {
                float framecountspeed = Math.Abs(NPC.velocity.X) * 2.2f;
                NPC.frameCounter += framecountspeed;
                NPC.spriteDirection = NPC.direction;

                if (NPC.frameCounter < 12)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else if (NPC.frameCounter < 24)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
                else if (NPC.frameCounter < 36)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                else if (NPC.frameCounter < 48)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                else if (NPC.frameCounter < 60)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
                else if (NPC.frameCounter < 72)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
                else if (NPC.frameCounter < 84)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                else if (NPC.frameCounter < 96)
                {
                    NPC.frame.Y = 9 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }

            if (NPC.velocity.Y != 0 && (!jumpSlashing || !shielding)) //If falling/jumping
            {
                NPC.frame.Y = 1 * frameHeight;
            }

            if (slashing) //If slashing
            {
                NPC.spriteDirection = NPC.direction;

                if (NPC.ai[3] < 18)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
                else if (NPC.ai[3] < 26)
                {
                    NPC.frame.Y = 12 * frameHeight;
                }
                else if (NPC.ai[3] < 29)
                {
                    NPC.frame.Y = 13 * frameHeight;
                }
                else if (NPC.ai[3] < 32)
                {
                    NPC.frame.Y = 14 * frameHeight;
                }
                else if (NPC.ai[3] < 35)
                {
                    NPC.frame.Y = 15 * frameHeight;
                }
                else if (NPC.ai[3] < 49)
                {
                    NPC.frame.Y = 16 * frameHeight;
                }
            }
            if (jumpSlashing) //If jumpslashing
            {
                NPC.spriteDirection = NPC.direction;

                if (NPC.ai[1] < 428)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
                else if (NPC.ai[1] < 436)
                {
                    NPC.frame.Y = 12 * frameHeight;
                }
                else if (NPC.ai[1] < 439)
                {
                    NPC.frame.Y = 13 * frameHeight;
                }
                else if (NPC.ai[1] < 442)
                {
                    NPC.frame.Y = 14 * frameHeight;
                }
                else if (NPC.ai[1] < 445)
                {
                    NPC.frame.Y = 15 * frameHeight;
                }
                else if (NPC.ai[1] < 510)
                {
                    NPC.frame.Y = 16 * frameHeight;
                }
            }

            if (NPC.velocity.X == 0 && NPC.velocity.Y == 0 && shielding && !jumpSlashing && !slashing) //If not moving at all (shielding)
            {
                NPC.spriteDirection = NPC.direction;
                NPC.frame.Y = 10 * frameHeight;
            }

            if (shielding && !jumpSlashing && NPC.ai[1] <= 420)
            {
                shieldFrame = shieldAnimTimer / 4; //Me smart, me figure out how to make loop AND simplify code at the same time!

                if (shieldFrame == 0)
                {
                    countingUP = true;
                }
                if (shieldFrame <= 14 && countingUP)
                {
                    shieldAnimTimer++;
                }
                if (shieldFrame == 14)
                {
                    countingUP = false;
                }
                if (shieldFrame >= 0 && !countingUP)
                {
                    shieldAnimTimer--;
                }
            }
        }

        #endregion

    }
}
