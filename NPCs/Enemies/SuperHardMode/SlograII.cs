using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items.Materials;

namespace tsorcRevamp.NPCs.Enemies.SuperHardMode
{
    class SlograII : ModNPC
    {
        int tridentDamage = 50;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
        }
        public override void SetDefaults()
        {
            NPC.npcSlots = 3;
            NPC.width = 38;
            NPC.height = 32;
            AnimationType = 104;
            NPC.aiStyle = 26;
            NPC.damage = 68;
            NPC.defense = 50;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = new Terraria.Audio.SoundStyle("tsorcRevamp/Sounds/NPCKilled/Gaibon_Roar");
            NPC.lifeMax = 3000;
            NPC.knockBackResist = 0f;
            NPC.value = 12000; // was 600
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Banners.SlograIIBanner>();

            UsefulFunctions.AddAttack(NPC, 150, ModContent.ProjectileType<Projectiles.Enemy.EarthTrident>(), tridentDamage, 11, SoundID.Item17);
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            tridentDamage = (int)(tridentDamage * tsorcRevampWorld.SHMScale);
        }


        #region Spawn
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player P = spawnInfo.Player; //this shortens our code up from writing this line over and over.

            bool Sky = spawnInfo.SpawnTileY <= (Main.rockLayer * 4);
            bool Meteor = P.ZoneMeteor;
            bool Jungle = P.ZoneJungle;
            bool Dungeon = P.ZoneDungeon;
            bool Corruption = (P.ZoneCorrupt || P.ZoneCrimson);
            bool Hallow = P.ZoneHallow;
            bool AboveEarth = spawnInfo.SpawnTileY < Main.worldSurface;
            bool InBrownLayer = spawnInfo.SpawnTileY >= Main.worldSurface && spawnInfo.SpawnTileY < Main.rockLayer;
            bool InGrayLayer = spawnInfo.SpawnTileY >= Main.rockLayer && spawnInfo.SpawnTileY < (Main.maxTilesY - 200) * 16;
            bool InHell = spawnInfo.SpawnTileY >= (Main.maxTilesY - 200) * 16;
            bool Ocean = spawnInfo.SpawnTileX < 3600 || spawnInfo.SpawnTileX > (Main.maxTilesX - 100) * 16;

            // these are all the regular stuff you get , now lets see......

            if (tsorcRevampWorld.SuperHardMode && !Main.dayTime && !Dungeon && Jungle && AboveEarth && Main.rand.NextBool(40)) return 1;

            if (tsorcRevampWorld.SuperHardMode && !Main.dayTime && !Dungeon && InBrownLayer && Main.rand.NextBool(60)) return 1;

            if (tsorcRevampWorld.SuperHardMode && !Main.dayTime && !Dungeon && InGrayLayer && Main.rand.NextBool(80)) return 1;

            if (tsorcRevampWorld.SuperHardMode && !Main.dayTime && !Dungeon && Jungle && Main.rand.NextBool(35)) return 1;


            return 0;
        }
        #endregion


        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {


            //Insert whatever you want to happen on-hit here
            if (NPC.justHit)
            {
                NPC.GetGlobalNPC<tsorcRevampGlobalNPC>().ProjectileTimer = 100f;
                //npc.knockBackResist = 0.09f;

                //WHEN HIT, CHANCE TO JUMP BACKWARDS && npc.velocity.Y >= -1f
                if (Main.rand.NextBool(10))//was 12
                {

                    NPC.TargetClosest(false);

                    NPC.velocity.Y = -8f;
                    NPC.velocity.X = -4f * NPC.direction;

                    //if (Main.rand.NextBool(1))
                    //{ 
                    NPC.GetGlobalNPC<tsorcRevampGlobalNPC>().ProjectileTimer = 140f;
                    //}

                    NPC.netUpdate = true;
                }

                //WHEN HIT, CHANCE TO DASH STEP BACKWARDS && npc.velocity.Y >= 1f
                else if (Main.rand.NextBool(8))//was 10
                {

                    //npc.TargetClosest(false);

                    NPC.velocity.Y = -4f;
                    NPC.velocity.X = -6f * NPC.direction;

                    //npc.direction *= -1;
                    //npc.spriteDirection = npc.direction;
                    //npc.ai[0] = 0f;
                    //if (Main.rand.NextBool(2))
                    //{
                    NPC.GetGlobalNPC<tsorcRevampGlobalNPC>().ProjectileTimer = 140f;
                    //}

                    //CHANCE TO JUMP AFTER DASH
                    if (Main.rand.NextBool(4))
                    {
                        NPC.TargetClosest(true);
                        NPC.velocity.Y = -7f;
                        NPC.GetGlobalNPC<tsorcRevampGlobalNPC>().ProjectileTimer = 140f;

                    }

                    NPC.netUpdate = true;
                }
            }
        }

        int hitCounter = 0;
        public override void AI()
        {
            tsorcRevampAIs.FighterAI(NPC, 3, 0.09f, 0.2f, true, 0, false, SoundID.Mummy, 2000, 0.1f, 4, true, canPounce: false);
            tsorcRevampAIs.LeapAtPlayer(NPC, 5, 4, 2, 128);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.justHit)
                {
                    hitCounter++;
                }

                if (hitCounter > 6 || (NPC.life < NPC.lifeMax / 10 && Main.rand.NextBool(400)))
                {
                    NPC.velocity = UsefulFunctions.Aim(NPC.Center, Main.player[NPC.target].Center, 15);
                    NPC.netUpdate = true;
                    hitCounter = 0;
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y), NPC.width, NPC.height, 4, 0, 0, 100, default, 2f);
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y), NPC.width, NPC.height, 18, 0, 0, 100, default, 2f);
                    }
                }

                if (Main.rand.NextBool(500) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int Spawned = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<NPCs.Enemies.SuperHardMode.VampireBat>(), 0);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(23, -1, -1, null, Spawned, 0f, 0f, 0f, 0);
                    }
                }
            }
        }

        #region gore
        public override void OnKill()
        {
            if (!Main.dedServ)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Slogra Gore 1").Type, 0.9f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Slogra Gore 2").Type, 0.9f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Slogra Gore 3").Type, 0.9f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Slogra Gore 2").Type, 0.9f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Slogra Gore 3").Type, 0.9f);

                for (int i = 0; i < 6; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Blood Splat").Type, 0.9f);
                }
            }
        }
        #endregion

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FlameOfTheAbyss>()));
        }
        #region Draw Spear
        static Texture2D spearTexture;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (spearTexture == null || spearTexture.IsDisposed)
            {
                spearTexture = (Texture2D)Mod.Assets.Request<Texture2D>("Projectiles/Enemy/EarthTrident");
            }
            if (NPC.GetGlobalNPC<tsorcRevampGlobalNPC>().ProjectileTimer >= 110)
            {
                int dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y), NPC.width, NPC.height, 6, NPC.velocity.X - 6f, NPC.velocity.Y, 150, Color.Red, 1f);
                Main.dust[dust].noGravity = true;

                SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                if (NPC.spriteDirection == -1)
                {
                    spriteBatch.Draw(spearTexture, NPC.Center - Main.screenPosition, new Rectangle(0, 0, spearTexture.Width, spearTexture.Height), drawColor, -MathHelper.PiOver2, new Vector2(8, 38), NPC.scale, effects, 0); // facing left (8, 38 work)
                }
                else
                {
                    spriteBatch.Draw(spearTexture, NPC.Center - Main.screenPosition, new Rectangle(0, 0, spearTexture.Width, spearTexture.Height), drawColor, MathHelper.PiOver2, new Vector2(8, 38), NPC.scale, effects, 0); // facing right, first value is height, higher number is higher
                }
            }
        }
        #endregion

    }
}