﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Weapons.Magic.Runeterra;

namespace tsorcRevamp.Projectiles.Magic.Runeterra
{

    public class OrbOfDeceptionOrb : ModProjectile
    {
        public bool Hit = false;
        public bool Full = false;
        private enum AIState
        {
            LaunchingForward,
            Retracting
        }

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        private AIState CurrentAIState
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float StateTimer => ref Projectile.ai[1];
        public ref float CollisionCounter => ref Projectile.localAI[0];
        public ref float SpinningStateTimer => ref Projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // This ensures that the projectile is synced when other players join the world.
            Projectile.width = 50; // The width of your projectile
            Projectile.height = 50; // The height of your projectile
            Projectile.friendly = true; // Deals damage to enemies
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.DamageType = DamageClass.Magic; // Deals melee damage
            Projectile.usesLocalNPCImmunity = true; // Used for hit cooldown changes in the ai hook
            Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;

        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.originalDamage = Projectile.damage;
            SoundEngine.PlaySound(new SoundStyle("tsorcRevamp/Sounds/Runeterra/Magic/OrbOfDeception/OrbCast") with { Volume = OrbOfDeception.OrbSoundVolume });
            if (player.GetModPlayer<tsorcRevampPlayer>().EssenceThief > 8)
            {
                Full = true;
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(player.Center).SafeNormalize(Vector2.Zero) * OrbOfDeception.ShootSpeed;
            switch (CurrentAIState)
            {
                case AIState.LaunchingForward:
                    {
                        if (Projectile.Distance(player.Center) > 800f)
                        {
                            CurrentAIState = AIState.Retracting;
                            StateTimer = 0f;
                            Hit = false;
                            Projectile.damage = Projectile.originalDamage;
                            Projectile.ResetLocalNPCHitImmunity();
                            SoundEngine.PlaySound(new SoundStyle("tsorcRevamp/Sounds/Runeterra/Magic/OrbOfDeception/OrbReturn") with { Volume = OrbOfDeception.OrbSoundVolume });
                            break;
                        }
                        break;
                    }
                case AIState.Retracting:
                    {
                        Projectile.velocity = unitVectorTowardsPlayer;
                        if (Projectile.Hitbox.Intersects(player.Hitbox))
                        {
                            if (player.GetModPlayer<tsorcRevampPlayer>().EssenceThief > 8 && !Full)
                            {
                                SoundEngine.PlaySound(new SoundStyle("tsorcRevamp/Sounds/Runeterra/Magic/OrbOfDeception/OrbFull") with { Volume = OrbOfDeception.OrbSoundVolume * 2 });
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("tsorcRevamp/Sounds/Runeterra/Magic/OrbOfDeception/OrbReturned") with { Volume = OrbOfDeception.OrbSoundVolume });
                            }
                            if (Full)
                            {
                                player.GetModPlayer<tsorcRevampPlayer>().EssenceThief -= 9;
                            }
                            Projectile.Kill();
                        }
                        break;
                    }
            }
            int frameSpeed = 5;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Full)
            {
                Dust.NewDust(Projectile.Center, 2, 2, DustID.PoisonStaff, 0, 0, 150, default, 0.5f);
                Lighting.AddLight(Projectile.Center, OrbOfDeception.FilledColor.ToVector3() * 2f);
            }
            else
            {
                Dust.NewDust(Projectile.Center, 2, 2, DustID.MagicMirror, 0, 0, 150, default, 0.5f);
                Lighting.AddLight(Projectile.Center, Color.LightSteelBlue.ToVector3() * 2f);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += OrbOfDeception.OrbDmgMod / 100f;
            if (Full)
            {
                modifiers.ScalingBonusDamage += OrbOfDeception.FilledOrbDmgMod / 100f;
            }
            if (CurrentAIState == AIState.Retracting)
            {
                modifiers.ScalingBonusDamage += OrbOfDeception.OrbReturnDmgMod / 100f;
            }
            modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (!Hit)
            {
                if (hit.Crit)
                {
                    player.GetModPlayer<tsorcRevampPlayer>().EssenceThief += 2;
                    SoundEngine.PlaySound(new SoundStyle("tsorcRevamp/Sounds/Runeterra/Magic/OrbOfDeception/OrbCrit") with { Volume = OrbOfDeception.OrbSoundVolume });
                }
                else
                {
                    player.GetModPlayer<tsorcRevampPlayer>().EssenceThief += 1;
                    SoundEngine.PlaySound(new SoundStyle("tsorcRevamp/Sounds/Runeterra/Magic/OrbOfDeception/OrbHit") with { Volume = OrbOfDeception.OrbSoundVolume });
                }
                if (Full)
                {
                    player.Heal((int)player.GetTotalDamage(DamageClass.Magic).ApplyTo(player.statManaMax2 / OrbOfDeception.HealManaDivisor) + OrbOfDeception.HealBaseValue);
                }
                Hit = true;
            }
            Projectile.damage = (int)(Projectile.damage * (1f - OrbOfDeception.DmgLossOnPierce / 100f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Full)
            {
                lightColor = OrbOfDeception.FilledColor;
            }
            if (CurrentAIState == AIState.LaunchingForward)
            {
                Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
                }
            }
            return true;
        }
    }
}