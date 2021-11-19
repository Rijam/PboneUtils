using log4net;
using PboneLib.CustomLoading;
using PboneLib.CustomLoading.Content;
using PboneLib.Services.CrossMod;
using PboneLib.Services.Net;
using PboneLib.Utils;
using PboneUtils.CrossMod.Call;
using PboneUtils.CrossMod.Ref.Content;
using PboneUtils.DataStructures.MysteriousTrader;
using PboneUtils.Helpers;
using PboneUtils.ID;
using PboneUtils.Packets;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PboneUtils
{
    public partial class PboneUtils : Mod
    {
        public override uint ExtraPlayerBuffSlots => (uint)PboneUtilsConfig.Instance.ExtraBuffSlots;

        public static PboneUtils Instance => ModContent.GetInstance<PboneUtils>();
        public static ILog Log => Instance.Logger;

        public static PboneUtilsUI UI => ModContent.GetInstance<PboneUtilsUI>();
        public static PboneUtilsTextures Textures => ModContent.GetInstance<PboneUtilsTextures>();
        public static ModRecipeManager Recipes => Instance.recipes;
        public static CrossModManager CrossMod => Instance.crossModManager;
        public static PacketManager PacketManager => Instance.packetManager;

        private ModRecipeManager recipes;
        private CrossModManager crossModManager;
        private PacketManager packetManager;

        public PboneUtils()
        {
            ContentAutoloadingEnabled = false;
        }

        public override void Load()
        {
            base.Load();

            // Instantiate managers
            recipes = new ModRecipeManager();
            packetManager = new PacketManager(this);
            packetManager.RegisterPacketHandler<SyncMysteriousTraderShop>(PacketID.SyncMysteriousTraderShop);

            crossModManager = new CrossModManager();
            crossModManager.CallManager.RegisterHandler<MysteriousTraderShopInterface>();

            crossModManager.RefManager.RegisterCompatibility<FargowiltasCompatibility>();
            crossModManager.RefManager.RegisterCompatibility<FargowiltasSoulsCompatibility>();
            crossModManager.RefManager.RegisterCompatibility<ExtensibleInventoryCompatibility>();
            crossModManager.RefManager.RegisterCompatibility<CensusCompatability>();
            crossModManager.RefManager.RegisterCompatibility<MagicStorageCompatability>();

            // Load MonoMod hooks
            Load_On();
            Load_IL();

            // Custom loading
            CustomModLoader loader = new CustomModLoader(this);

            // Localizations
            loader.Add(new PboneLib.CustomLoading.Localization.LocalizationLoader(translation => {
                LocalizationLoader.AddTranslation(translation);
            }));

            // ModConfigs
            ContentLoader cLoader = new ContentLoader(content => {
                // TODO Unhardcode me! Method in PConfig
                AddConfig("Feature Config", (ModConfig)content.Content);
            });
            cLoader.Settings.TryToLoadConditions = ContentLoader.ContentLoaderSettings.PresetTryToLoadConfigConditions;
            loader.Add(cLoader);

            // Everything else
            cLoader = new ContentLoader(this.AddContent);
            // Added for clarity, the Action only ctor automatically calls FillWithDefaultConditions
            cLoader.Settings.TryToLoadConditions = ContentLoader.ContentLoaderSettings.PresetNormalTryToLoadConditions;
            loader.Add(cLoader);

            loader.Load();
        }

        public override object Call(params object[] args) => crossModManager.CallManager.HandleCall(args);
        public override void HandlePacket(BinaryReader reader, int whoAmI) => PacketManager.HandlePacket(reader);

        public override void PostSetupContent()
        {
            base.PostSetupContent();

            // TODO improve this
            // These get assinged to something by their ctors, don't worry
#pragma warning disable CA1806 // Do not ignore method results
            new MysteriousTraderShopManager();
            new CompiledMysteriousTraderShop(MysteriousTraderShopManager.Instance);
#pragma warning restore CA1806 // Do not ignore method results
        }

        #region Recipes
        public override void AddRecipes()
        {
            base.AddRecipes();
            recipes.AddRecipes(this);
        }
        public override void AddRecipeGroups()
        {
            base.AddRecipeGroups();
            recipes.AddRecipeGroups();
        }
        #endregion

        // TODO improve nulling upon unload
        public override void Unload()
        {
            base.Unload();

            recipes = null;
            crossModManager = null;
            packetManager = null;

            if (MysteriousTraderShopManager.Instance != null)
                MysteriousTraderShopManager.Unload();
        }
    }
}