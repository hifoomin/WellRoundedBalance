using MonoMod.Cil;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using Mono.Cecil.Cil;

namespace UltimateCustomRun.Equipment
{
    public class TheBackup : EquipmentBase
    {
        public override string Name => "::: Equipment :: The Backup";
        public override string InternalPickupToken => "droneBackup";

        public override bool NewPickup => true;

        public override bool NewDesc => true;

        public override string PickupText => "Call drones for back up. Lasts " + Duration + " seconds.";

        public override string DescText => "Call <style=cIsDamage>" + DroneCount + " Strike Drones</style> to fight for you. Lasts " + Duration + " seconds.";

        public static int DroneCount;
        public static float Duration;

        public override void Init()
        {
            DroneCount = ConfigOption(4, "Drone Count", "Vanilla is 4");
            Duration = ConfigOption(25f, "Duration", "Vanilla is 8");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireDroneBackup += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdcR4(25f)))
            {
                c.Next.Operand = Duration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply The Backup Duration hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdcI4(4)))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, DroneCount);
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply The Backup Drone Count hook");
            }
        }
    }
}