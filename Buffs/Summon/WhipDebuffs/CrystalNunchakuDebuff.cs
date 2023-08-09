using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.NPCs;

namespace tsorcRevamp.Buffs.Summon.WhipDebuffs
{
    public class CrystalNunchakuDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var globalNPC = npc.GetGlobalNPC<tsorcRevampGlobalNPC>();

            globalNPC.markedByCrystalNunchaku = true;
            globalNPC.CrystalNunchakuUpdateTick += 0.0167f;
            Dust.NewDust(npc.Center, 10, 10, DustID.CrystalPulse, Scale:0.5f);

            if (globalNPC.CrystalNunchakuUpdateTick > 15.02f)
            {
                globalNPC.CrystalNunchakuUpdateTick = 0f;
                globalNPC.CrystalNunchakuStacks = 10;
                globalNPC.CrystalNunchakuProc = false;
            }

            if (globalNPC.CrystalNunchakuUpdateTick >= 5f && globalNPC.CrystalNunchakuUpdateTick <= 5.2f)
            {
                Dust.NewDust(npc.Center, 50, 50, DustID.CrystalPulse, Scale:2f);
                SoundEngine.PlaySound(SoundID.Item78 with { Volume = 4f }, npc.Center);
                globalNPC.CrystalNunchakuProc = true;
            }
        }
    }
}