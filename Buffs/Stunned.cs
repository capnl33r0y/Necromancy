using Terraria;
using Terraria.ModLoader;

namespace Necromancy.Buffs
{
    public class Stunned : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Stunned");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.boss) npc.velocity.X *= 0.05f;
        }
    }
}
