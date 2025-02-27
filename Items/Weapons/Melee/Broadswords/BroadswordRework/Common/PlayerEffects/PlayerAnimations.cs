﻿using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using tsorcRevamp.Items.Weapons.Melee.Broadswords.BroadswordRework.Utilities;

namespace tsorcRevamp.Items.Weapons.Melee.Broadswords.BroadswordRework.Common.PlayerEffects;

public sealed class PlayerAnimations : ModPlayer
{
    public PlayerFrames? ForcedHeadFrame;
    public PlayerFrames? ForcedBodyFrame;
    public PlayerFrames? ForcedLegFrame;

    public override void PostUpdate()
    {
        static void TryForceFrame(ref Rectangle frame, ref PlayerFrames? newFrame)
        {
            if (newFrame.HasValue)
            {
                frame = newFrame.Value.ToRectangle();

                newFrame = null;
            }
        }

        TryForceFrame(ref Player.headFrame, ref ForcedHeadFrame);
        TryForceFrame(ref Player.bodyFrame, ref ForcedBodyFrame);
        TryForceFrame(ref Player.legFrame, ref ForcedLegFrame);
    }
}
