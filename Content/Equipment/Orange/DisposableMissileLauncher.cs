using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Equipment
{
    public class DisposableMissileLauncher : EquipmentBase
    {
        public override string Name => ":: Equipment :: Disposable Missile Launcher";

        public override EquipmentDef InternalPickup => RoR2Content.Equipment.CommandMissile;

        public override string PickupText => "Fire a swarm of missiles.";

        public override string DescText => "Fire a swarm of <style=cIsDamage>" + missileCount + "</style> missiles that deal <style=cIsDamage>" + missileCount + "x300% damage</style>.";

        [ConfigField("Cooldown", "", 30f)]
        public static float cooldown;

        [ConfigField("Missile Count", "", 12)]
        public static int missileCount;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var DML = Utils.Paths.EquipmentDef.CommandMissile.Load<EquipmentDef>();
            DML.cooldown = cooldown;

            IL.RoR2.EquipmentSlot.FireCommandMissile += EquipmentSlot_FireCommandMissile;
        }

        private void EquipmentSlot_FireCommandMissile(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(12)))
            {
                c.Index += 1;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return missileCount;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Disposable Missile Launcher Missile Count hook");
            }
        }
    }
}