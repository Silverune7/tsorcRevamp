﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace tsorcRevamp.Items.Debug
{
    class EventReset : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 21;
            Item.height = 21;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Lime;
        }


        public override bool? UseItem(Player player)
        {
            tsorcScriptedEvents.InitializeScriptedEvents();
            tsorcRevampWorld.NewSlain = new System.Collections.Generic.Dictionary<NPCDefinition, int>();
            return true;
        }
    }
}
