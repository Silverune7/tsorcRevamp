﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Projectiles.VFX;

namespace tsorcRevamp.Projectiles
{
    class TyphoonArrow : DynamicTrail
    {
        public override string Texture => base.Texture;
        public const int DebuffDuration = 4;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SentryShot[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.sentry = true;
            Projectile.width = 58;
            Projectile.height = 2;
            Projectile.timeLeft = 120;
            Projectile.friendly = true;
            Projectile.tileCollide = false; //custom tile collision code
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            trailWidth = 12;
            trailPointLimit = 1000;
            trailCollision = true;
            collisionFrequency = 2;
            collisionEndPadding = 0;
            collisionPadding = 0;
            trailYOffset = 0;
            trailMaxLength = 200;
            NPCSource = false;
            noFadeOut = true;
            noDiscontinuityCheck = true;
            customEffect = ModContent.Request<Effect>("tsorcRevamp/Effects/CursedTormentor", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.originalDamage = (int)Projectile.ai[0];
        }
        public override void AI()
        {
            base.AI();
            if (Main.GameUpdateCount % 5 == 0)
            {
                Projectile.netUpdate = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i = collisionEndPadding; i < trailPositions.Count - collisionFrequency - 1 - collisionPadding; i += collisionFrequency)
            {
                if (UsefulFunctions.IsTileReallySolid(trailPositions[i] / 16))
                {
                    Projectile.Kill();
                }
                if (trailPositions[i + collisionFrequency - 1] == Vector2.Zero)
                {
                    break;
                }
            }
        }
        public override float CollisionWidthFunction(float progress)
        {
            if (progress > 0.9)
            {
                return ((1 - progress) / 0.1f) * trailWidth;
            }

            return trailWidth * progress;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new SoundStyle("tsorcRevamp/Sounds/Runeterra/Summon/GaleforceHit") with { Volume = 0.5f }, target.Center);
            target.AddBuff(BuffID.OnFire3, DebuffDuration * 60);
        }
        Vector2 samplePointOffset1;
        Vector2 samplePointOffset2;
        public override void SetEffectParameters(Effect effect)
        {
            float hostVel = Projectile.velocity.Length();

            float modifiedTime = 0.0007f * hostVel;

            if (Main.gamePaused)
            {
                modifiedTime = 0;
            }

            if (fadeOut == 1)
            {
                samplePointOffset1.X += (modifiedTime);
                samplePointOffset1.Y -= (0.001f);
                samplePointOffset2.X += (modifiedTime * 3.01f);
                samplePointOffset2.Y += (0.001f);

                samplePointOffset1.X += modifiedTime;
                samplePointOffset1.X %= 1;
                samplePointOffset1.Y %= 1;
                samplePointOffset2.X %= 1;
                samplePointOffset2.Y %= 1;
            }
            collisionEndPadding = trailPositions.Count / 2;

            effect.Parameters["noiseTexture"].SetValue(tsorcRevamp.NoiseWavy);
            effect.Parameters["length"].SetValue(trailCurrentLength);
            effect.Parameters["samplePointOffset1"].SetValue(samplePointOffset1);
            effect.Parameters["samplePointOffset2"].SetValue(samplePointOffset2);
            effect.Parameters["fadeOut"].SetValue(fadeOut);
            effect.Parameters["speed"].SetValue(hostVel);
            effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["shaderColor"].SetValue(Color.OrangeRed.ToVector4());
            effect.Parameters["WorldViewProjection"].SetValue(GetWorldViewProjectionMatrix());
        }
    }
}
