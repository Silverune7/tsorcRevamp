﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Projectiles.Ranged
{
    class ToxicCatShot : ModProjectile
    {
        private int toxiccattimer;
        int sinkTimer;
        bool hasCollided;
        bool hasExploded;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 145;
            Projectile.penetrate = 3;

            DrawOffsetX = -3;
            DrawOriginOffsetY = -5;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)Terraria.GameContent.TextureAssets.Projectile[Projectile.type];

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 16, 10, 16), Color.White, Projectile.rotation, new Vector2(5, 9), Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Inflate some target hitboxes if they are beyond 8,8 size
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            // Return if the hitboxes intersects, which means the javelin collides or not
            return projHitbox.Intersects(targetHitbox);
        }

        public override void OnKill(int timeLeft)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath9 with { Volume = 0.3f }, Projectile.Center);
            for (int d = 0; d < 20; d++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 75, Projectile.velocity.X * 1.2f, Projectile.velocity.Y * 1.2f, 30, default(Color), 1f);
                Main.dust[dust].velocity.X = +Main.rand.Next(-50, 51) * 0.05f;
                Main.dust[dust].velocity.Y = +Main.rand.Next(-50, 51) * 0.05f;
                Main.dust[dust].noGravity = true;
            }
        }

        /*
		 * The following showcases recommended practice to work with the ai field
		 * You make a property that uses the ai as backing field
		 * This allows you to contextualize ai better in the code
		 */

        // Are we sticking to a target?
        public bool IsStickingToTarget
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        // Index of the current target
        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private const int MAX_STICKY_JAVELINS = 8; // This is the max. amount of javelins being able to attach
        private readonly Point[] _stickingJavelins = new Point[MAX_STICKY_JAVELINS]; // The point array holding for sticking javelins

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            IsStickingToTarget = true; // we are sticking to a target
            TargetWhoAmI = target.whoAmI; // Set the target whoAmI
            Projectile.timeLeft = 480;
            Projectile.velocity =
                (target.Center - Projectile.Center) *
                0.75f; // Change velocity based on delta center of targets (difference between entity centers)
            Projectile.netUpdate = true; // netUpdate this javelin
            target.AddBuff(ModContent.BuffType<Buffs.ToxicCatDrain>(), 480); // Adds the ExampleJavelin debuff for a very small DoT

            Projectile.damage = 0; // Makes sure the sticking javelins do not deal damage anymore


            // It is recommended to split your code into separate methods to keep code clean and clear
            UpdateStickyJavelins(target);


        }
        /*
		 * The following code handles the javelin sticking to the enemy hit.
		 */
        private void UpdateStickyJavelins(NPC target)
        {
            int currentJavelinIndex = 0; // The javelin index

            for (int i = 0; i < Main.maxProjectiles; i++) // Loop all projectiles
            {
                Projectile currentProjectile = Main.projectile[i];
                if (i != Projectile.whoAmI // Make sure the looped projectile is not the current javelin
                    && currentProjectile.active // Make sure the projectile is active
                    && currentProjectile.owner == Main.myPlayer // Make sure the projectile's owner is the client's player
                    && currentProjectile.type == Projectile.type // Make sure the projectile is of the same type as this javelin
                    && currentProjectile.ModProjectile is ToxicCatShot javelinProjectile // Use a pattern match cast so we can access the projectile like an ExampleJavelinProjectile
                    && javelinProjectile.IsStickingToTarget // the previous pattern match allows us to use our properties
                    && javelinProjectile.TargetWhoAmI == target.whoAmI)
                {

                    _stickingJavelins[currentJavelinIndex++] = new Point(i, currentProjectile.timeLeft); // Add the current projectile's index and timeleft to the point array
                    if (currentJavelinIndex >= _stickingJavelins.Length)  // If the javelin's index is bigger than or equal to the point array's length, break
                        break;
                }
            }

            // Remove the oldest sticky javelin if we exceeded the maximum
            if (currentJavelinIndex >= MAX_STICKY_JAVELINS)
            {
                int oldJavelinIndex = 0;
                // Loop our point array
                for (int i = 1; i < MAX_STICKY_JAVELINS; i++)
                {
                    // Remove the already existing javelin if it's timeLeft value (which is the Y value in our point array) is smaller than the new javelin's timeLeft
                    if (_stickingJavelins[i].Y < _stickingJavelins[oldJavelinIndex].Y)
                    {
                        oldJavelinIndex = i; // Remember the index of the removed javelin
                    }
                }
                // Remember that the X value in our point array was equal to the index of that javelin, so it's used here to kill it.
                Main.projectile[_stickingJavelins[oldJavelinIndex].X].Kill();
            }
        }


        public override void AI()
        {
            if (IsStickingToTarget) StickyAI();
            else NormalAI();
        }
        private void NormalAI()
        {
            Projectile.damage = 1;
            //Change these two variables to affect the rotation of your projectile
            float rotationsPerSecond = Math.Abs(Projectile.velocity.X) * 0.25f + Math.Abs(Projectile.velocity.Y) * 0.25f;
            bool rotateClockwise = true;
            //The rotation is set here
            if (Math.Abs(Projectile.velocity.X) != 0 || Math.Abs(Projectile.velocity.Y) != 0) { Projectile.rotation += (rotateClockwise ? 1 : -1) * MathHelper.ToRadians(rotationsPerSecond * 30f); }

            Lighting.AddLight(Projectile.position, 0.2496f, 0.4584f, 0.130f);

            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft >= 139 && Projectile.timeLeft < 142)
            {
                for (int d = 0; d < 2; d++)
                {
                    Dust dust = Main.dust[Dust.NewDust(new Vector2(Projectile.position.X - 7, Projectile.position.Y - 7), 24, 24, 75, Projectile.velocity.X * .5f, Projectile.velocity.Y * .5f, 100, default(Color), .8f)];
                    dust.noGravity = true;
                }
            }

            if (Math.Abs(Projectile.velocity.X) == 0 && Math.Abs(Projectile.velocity.Y) == 0)
            {
                if (toxiccattimer > 40)
                {
                    toxiccattimer = 0;
                }

                if (++Projectile.frameCounter >= 20) //ticks spent on each frame
                {
                    Projectile.frameCounter = 0;

                    if (++Projectile.frame >= 2)
                    {
                        Projectile.frame = 0;
                    }
                }
            }

            if (hasCollided)
            {
                sinkTimer++;
                Projectile.knockBack = 0;
                Projectile.tileCollide = false;
                Projectile.height = 12;
                Projectile.width = 12;
                if (sinkTimer < 6)
                {
                    Projectile.velocity = Projectile.oldVelocity * 0.3f;
                }
                else
                {
                    Projectile.velocity = new Vector2(0, 0);
                }
            }

            Player player = Main.player[Projectile.owner];
            if (!hasExploded)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active == true && Projectile.Hitbox.Intersects(Main.projectile[i].Hitbox) && (Main.projectile[i].type == ModContent.ProjectileType<ToxicCatDetonator>() || Main.projectile[i].type == ModContent.ProjectileType<ToxicCatExplosion>()))
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item74 with { Volume = 1f, Pitch = 0.2f }, Main.projectile[i].Center);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.NewProjectile(player.GetSource_FromThis(), Main.projectile[i].Center, Main.projectile[i].velocity, ModContent.ProjectileType<ToxicCatExplosion>(), Projectile.damage * 2, 6f, Projectile.owner, 6, 0);
                        }
                        hasExploded = true;
                        Projectile.Kill();
                    }
                }
            }
        }

        private void StickyAI()
        {
            // These 2 could probably be moved to the ModifyNPCHit hook, but in vanilla they are present in the AI
            Projectile.ignoreWater = true; // Make sure the projectile ignores water
            Projectile.tileCollide = false; // Make sure the projectile doesn't collide with tiles anymore
            const int aiFactor = 8; // Change this factor to change the 'lifetime' of this sticking javelin //These are seconds. Keep debuff duration to same duration as is set here.
            Projectile.localAI[0] += 1f;

            /*if (projectile.timeLeft > 2)
			{
				projectile.timeLeft = 100;
			}*/

            Projectile.rotation = Projectile.velocity.ToRotation();
            // Every 30 ticks, the javelin will perform a hit effect
            bool hitEffect = Projectile.localAI[0] % 30f == 0f;
            int projTargetIndex = (int)TargetWhoAmI;

            if (Projectile.localAI[0] >= 60 * aiFactor || projTargetIndex < 0 || projTargetIndex >= 200)
            { // If the index is past its limits, kill it
                Projectile.Kill();
            }

            else if (Main.npc[projTargetIndex].active && !Main.npc[projTargetIndex].dontTakeDamage)
            { // If the target is active and can take damage
              // Set the projectile's position relative to the target's center
                Projectile.Center = Main.npc[projTargetIndex].Center - Projectile.velocity * 2.5f;
                //projectile.rotation = Main.npc[projTargetIndex].Center;
                Projectile.gfxOffY = Main.npc[projTargetIndex].gfxOffY;
                if (hitEffect)
                { // Perform a hit effect here
                    Main.npc[projTargetIndex].HitEffect(0, 1.0);
                }
            }

            else
            { // Otherwise, kill the projectile
                Projectile.Kill();
            }

            //ANIMATION
            if (toxiccattimer > 40)
            {
                toxiccattimer = 0;
            }

            if (++Projectile.frameCounter >= 20) //ticks spent on each frame
            {
                Projectile.frameCounter = 0;

                if (++Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath9 with { Volume = 0.8f }, Projectile.Center);
            for (int d = 0; d < 20; d++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 75, Projectile.velocity.X * 1f, Projectile.velocity.Y * 1f, 30, default(Color), 1f);
                Main.dust[dust].velocity.X = +Main.rand.Next(-50, 51) * 0.05f;
                Main.dust[dust].velocity.Y = +Main.rand.Next(-50, 51) * 0.05f;
                Main.dust[dust].noGravity = true;
            }

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath9 with { Volume = 0.4f }, Projectile.Center);
            hasCollided = true;
            Projectile.timeLeft = 300; //Lives for 5 seconds on surfaces
            return false;
        }
    }
}
