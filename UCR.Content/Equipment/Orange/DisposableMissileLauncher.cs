using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class DisposableMissileLauncher : EquipmentBase
    {
        public override string Name => "::: Equipment :: Disposable Missile Launcher";
        public override string InternalPickupToken => "commandMissile";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Fire a swarm of <style=cIsDamage>" + MissileCount + "</style> missiles that deal <style=cIsDamage>" + MissileCount + "x300% damage</style>.";

        public static int MissileCount;

        public override void Init()
        {
            MissileCount = ConfigOption(12, "Missile Count", "Vanilla is 12");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireCommandMissile += ChangeMissileCount;
        }

        private void ChangeMissileCount(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(12)))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, MissileCount);
                // c.Next.Operand = MissileCount;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Disposable Missile Launcher Count hook");
            }
        }
    }
}