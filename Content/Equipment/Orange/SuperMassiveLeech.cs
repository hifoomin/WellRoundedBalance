using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Equipment.Orange
{
    public class SuperMassiveLeech : EquipmentBase
    {
        public override string Name => "::: Equipment :: Super Massive Leech";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.LifestealOnHit;

        public override string PickupText => "Heal for a percentage of the damage you deal for " + duration + " seconds.";

        public override string DescText => "<style=cIsHealing>Heal</style> for <style=cIsHealing>" + d(percentHealing) + "</style> of the <style=cIsDamage>damage</style> you deal. Lasts <style=cIsHealing>" + duration + "</style> seconds.";

        [ConfigField("Cooldown", "", 50f)]
        public static float cooldown;

        [ConfigField("Buff Duration", "", 8f)]
        public static float duration;

        [ConfigField("Percent Healing", "Decimal.", 0.2f)]
        public static float percentHealing;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireLifeStealOnHit += ChangeDuration;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeHealing;

            var Leech = Utils.Paths.EquipmentDef.LifestealOnHit.Load<EquipmentDef>();
            Leech.cooldown = cooldown;
        }

        private void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdfld<DamageInfo>("damage"),
               x => x.MatchLdcR4(0.2f)))
            {
                c.Index += 2;
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return percentHealing;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Super Massive Leech Healing hook");
            }
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f)))
            {
                c.Next.Operand = duration;
            }
            else
            {
                Logger.LogError("Failed to apply Super Massive Leech Duration hook");
            }
        }
    }
}