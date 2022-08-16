using MonoMod.Cil;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using Mono.Cecil.Cil;
using System;

namespace UltimateCustomRun.Equipment
{
    public class SuperMassiveLeech : EquipmentBase
    {
        public override string Name => "::: Equipment :: Super Massive Leech";
        public override string InternalPickupToken => "lifestealOnHit";

        public override bool NewPickup => true;

        public override bool NewDesc => true;

        public override string PickupText => "Heal for a percentage of the damage you deal for " + Duration + " seconds.";

        public override string DescText => "<style=cIsHealing>Heal</style> for <style=cIsHealing>" + d(HealPercent) + "</style> of the <style=cIsDamage>damage</style> you deal. Lasts <style=cIsHealing>" + Duration + "</style> seconds.";

        public static float Duration;
        public static float HealPercent;

        public override void Init()
        {
            Duration = ConfigOption(8f, "Duration", "Vanilla is 8");
            HealPercent = ConfigOption(0.2f, "Percent Healing", "Decimal. Vanilla is 0.2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireLifeStealOnHit += ChangeDuration;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeHealing;
        }

        private void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdfld<DamageInfo>("damage"),
               x => x.MatchLdcR4(0.2f)))
            {
                c.Index += 2;
                // c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return HealPercent;
                });
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Super Massive Leech Healing hook");
            }
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f)))
            {
                c.Next.Operand = Duration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Super Massive Leech Duration hook");
            }
        }
    }
}