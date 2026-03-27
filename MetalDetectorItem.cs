using MEC;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrikanUtils.CustomItems;
using FrikanUtils.Spawnpoints;
using FrikanUtils.Spawnpoints.LootSpawn;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MetalDetectorW;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace MetalDetector
{
    public class MetalDetectorItem : CustomWeaponItem
    {
        public override string Id => "gamendegamer.metal_detector";
        public override string Name => "Inventory Scanner";
        public override string Description => "Scans players for items.";
        public override ItemType VisualType => ItemType.GunCOM15;

        public override SpawnLocation SpawnLocation => new SpawnLocation
        {
            Points = new ISpawnPoint[]
            {
                new LootSpawnPoint
                {
                    Point = LootPoint.HczCheckpointDesk,
                    Chance = 1
                }
            }
        };

        public override int? Weight => 1;

        private Dictionary<int, float> _cooldowns = new Dictionary<int, float>();
        private readonly int _mask = LayerMask.GetMask("Default", "Player", "Hitbox");

        protected override void SubscribeEvents()
        {
            PlayerEvents.Left += OnLeft;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerEvents.Left -= OnLeft;
            base.UnsubscribeEvents();
        }

        protected override void OnShootingWeapon(PlayerShootingWeaponEventArgs ev)
        {
            if (!Check(ev.FirearmItem.Serial))
            {
                return;
            }

            ev.IsAllowed = false;

            if (_cooldowns.TryGetValue(ev.Player.PlayerId, out var nextUseTime) && Time.time < nextUseTime)
            {
                ev.Player.SendHint(Plugin.Instance.Config.CooldownMessage, 2f);
                return;
            }

            var startPos = ev.Player.Camera.position + ev.Player.Camera.forward * 0.1f;

            // Use Config.MaxDistance
            if (Physics.Raycast(startPos, ev.Player.Camera.forward, out RaycastHit hit,
                    Plugin.Instance.Config.MaxDistance, _mask))
            {
                var target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());

                if (target != null && target != ev.Player && !target.IsSCP)
                {
                    // Use Config.Cooldown
                    _cooldowns[ev.Player.PlayerId] = Time.time + Plugin.Instance.Config.Cooldown;
                    Timing.RunCoroutine(ScanRoutine(ev.Player, target));
                }
                else
                {
                    ev.Player.SendHint(Plugin.Instance.Config.NoPlayerFound, 2f);
                }
            }
            else
            {
                ev.Player.SendHint(Plugin.Instance.Config.NoPlayerFound, 2f);
            }
        }

        private IEnumerator<float> ScanRoutine(Player scanner, Player target)
        {
            scanner.SendHint(Plugin.Instance.Config.ScanStarted.Replace("%player%", target.Nickname), 2f);
            target.SendBroadcast(Plugin.Instance.Config.TargetScanned, 3);

            // Use Config.ScanDuration
            yield return Timing.WaitForSeconds(Plugin.Instance.Config.ScanDuration);

            if (scanner == null || target == null || scanner.IsDestroyed || target.IsDestroyed)
                yield break;

            if (!target.Items.Any())
            {
                scanner.SendHint(Plugin.Instance.Config.NoItems.Replace("%player%", target.Nickname), 4f);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Plugin.Instance.Config.ScanResult.Replace("%player%", target.Nickname));

                foreach (Item item in target.Items)
                {
                    sb.AppendLine($"- {item.Type}");
                }

                scanner.SendHint(sb.ToString(), 6f);
            }
        }

        protected override void Setup(Item item)
        {
            base.Setup(item);

            if (item is FirearmItem firearm)
            {
                firearm.StoredAmmo = 2;
                firearm.ChamberedAmmo = 1;
                firearm.Cocked = true;
            }
        }

        private void OnLeft(PlayerLeftEventArgs ev)
        {
            _cooldowns.Remove(ev.Player.PlayerId);
        }
    }
}