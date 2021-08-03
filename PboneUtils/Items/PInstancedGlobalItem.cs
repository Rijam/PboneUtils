﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PboneUtils.Items
{
    public class PInstancedGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public bool AutoswingOrig;
        public bool ChangedAutoswing = false;

        public override void SetDefaults(Item item)
        {
            base.SetDefaults(item);
            AutoswingOrig = Item.autoReuse;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (PboneUtilsConfig.Instance.AutoswingOnEverything)
            {
                // Don't autoswing blacklisted items
                if (PboneUtilsConfig.Instance.AutoswingOnEverythingBlacklist != null)
                {
                    foreach (ItemDefinition itemDefinition in PboneUtilsConfig.Instance.AutoswingOnEverythingBlacklist)
                    {
                        if (itemDefinition.Type == Item.type)
                        {
                            Item.autoReuse = AutoswingOrig;
                            return true;
                        }
                    }
                }

                // Check for spears (at least vanilla ones)
                Projectile projectile = new Projectile();
                projectile.SetDefaults(Item.shoot);
                if (projectile.aiStyle == 19)
                {
                    Item.autoReuse = AutoswingOrig;
                    return true;
                }

                Item.autoReuse = true;
                ChangedAutoswing = true;

                return true;
            }
            else if (!PboneUtilsConfig.Instance.AutoswingOnEverything && ChangedAutoswing)
            {
                Item.autoReuse = AutoswingOrig;
                ChangedAutoswing = false;

                return true;
            }

            return true;
        }

        public override GlobalItem NewInstance(Item item) =>
            new PInstancedGlobalItem() {
                AutoswingOrig = Item.autoReuse,
                ChangedAutoswing = false
            };
    }
}
