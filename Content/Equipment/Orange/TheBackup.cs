using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Equipment.Orange
{
    public class TheBackup : EquipmentBase
    {
        public override string Name => ":: Equipment :: The Backup";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.DroneBackup;

        public override string PickupText => "Call drones for back up. Lasts " + duration + " seconds.";

        public override string DescText => "Call <style=cIsDamage>" + droneCount + " Strike Drones</style> to fight for you. Lasts " + duration + " seconds.";

        [ConfigField("Drone Count", "", 4)]
        public static int droneCount;

        [ConfigField("Duration", "", 25f)]
        public static float duration;

        public override void Init()
        {
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
                c.Next.Operand = duration;
            }
            else
            {
                Logger.LogError("Failed to apply The Backup Duration hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdcI4(4)))
            {
                c.Index += 1;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return droneCount;
                });
            }
            else
            {
                Logger.LogError("Failed to apply The Backup Drone Count hook");
            }
        }
    }
}