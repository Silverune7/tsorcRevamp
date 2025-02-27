using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.NPCs;

namespace tsorcRevamp.Buffs.Weapons.Summon.WhipDebuffs
{
    public class CrystalNunchakuDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var globalNPC = npc.GetGlobalNPC<tsorcRevampGlobalNPC>();

            globalNPC.markedByCrystalNunchaku = true;
            globalNPC.CrystalNunchakuUpdateTick++;
            Dust.NewDust(npc.Center, 10, 10, DustID.CrystalPulse, Scale: 0.5f);

            if (npc.buffTime[buffIndex] == 1)
            {
                globalNPC.CrystalNunchakuUpdateTick = 0;
                globalNPC.CrystalNunchakuStacks = 10;
                globalNPC.CrystalNunchakuProc = false;
            }

            if (globalNPC.CrystalNunchakuUpdateTick >= 5 * 60 && globalNPC.CrystalNunchakuUpdateTick <= 5 * 60 + 5)
            {
                Dust.NewDust(npc.Center, 50, 50, DustID.CrystalPulse, Scale: 2f);
                SoundEngine.PlaySound(SoundID.Item78 with { Volume = 4f }, npc.Center);
                globalNPC.CrystalNunchakuProc = true;
            }
        }
    }
}