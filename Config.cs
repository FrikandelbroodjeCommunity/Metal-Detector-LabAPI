using System.ComponentModel;

namespace MetalDetectorW
{
    public class Config
    {
        public bool Debug { get; set; } = false;

        [Description("The maximum distance to scan.")]
        public float MaxDistance { get; set; } = 5f;

        [Description("Time in seconds to wait before showing results.")]
        public float ScanDuration { get; set; } = 3f;

        [Description("Total cooldown before using again.")]
        public float Cooldown { get; set; } = 5f;
        
        [Description("Message shown to the scanner when they start scanning.")]
        public string ScanStarted { get; set; } = "Scanning %player%...";

        [Description("Message shown to the player being scanned.")]
        public string TargetScanned { get; set; } = "You are being scanned by a Metal Detector.";

        [Description("Header for the scan result.")]
        public string ScanResult { get; set; } = "<color=yellow>Items found on %player%:</color>";

        [Description("Message shown when the target has no items.")]
        public string NoItems { get; set; } = "%player% has no items.";

        [Description("Message shown when no player is found.")]
        public string NoPlayerFound { get; set; } = "No player found.";

        [Description("Message shown when scanner is in cooldown.")]
        public string CooldownMessage { get; set; } = "Metal Detector is cooling down.";
    }
}