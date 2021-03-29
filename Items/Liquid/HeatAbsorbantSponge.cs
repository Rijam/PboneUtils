﻿using PboneUtils.Helpers;
using PboneUtils.ID;
using Terraria;
using Terraria.ID;

namespace PboneUtils.Items.Liquid
{
    public class HeatAbsorbantSponge : PItem
    {
        public override bool ShowItemIconWhenInRange => true;

        public override void SetDefaults()
        {
            base.SetDefaults();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 12;
            item.useTime = 5;
            item.useTurn = true;
            item.autoReuse = true;
            item.rare = ItemRarityID.Lime;
            item.value = Item.sellPrice(0, 10, 0, 0);
            item.tileBoost += 2;
        }

        public override bool UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.IsTargetTileInItemRange(item))
                {
                    if (LiquidHelper.DrainLiquid(Player.tileTargetX, Player.tileTargetY, LiquidID.Lava))
                    {
                        Main.PlaySound(SoundID.Splash, (int)player.position.X, (int)player.position.Y);
                        return true;
                    }
                }
            }

            return base.UseItem(player);
        }
    }
}
