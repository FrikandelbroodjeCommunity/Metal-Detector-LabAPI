using System;
using FrikanUtils.CustomItems;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using MetalDetector;

namespace MetalDetectorW
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "ByLeTalha";
        public override string Name => "MetalDetector";
        public override string Description => "Allows the scanning of a players inventory";
        public override Version Version => new Version(2, 0, 0);
        public override Version RequiredApiVersion => new Version(LabApiProperties.CompiledVersion);

        public static Plugin Instance;

        private static readonly MetalDetectorItem Item = new MetalDetectorItem();

        public override void Enable()
        {
            Instance = this;
            CustomItemHandler.RegisterCustomItem(Item);
        }

        public override void Disable()
        {
            CustomItemHandler.UnregisterCustomItem(Item);
        }
    }
}