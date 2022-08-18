using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace UltimateCustomRun.Items.Whites
{
    public class DelicateWatch : ItemBase
    {
        public static float Damage;
        public static float Threshold;
        public override string Name => ":: Items : Whites :: Delicate Watch";
        public override string InternalPickupToken => "fragileDamageBonus";
        public override bool NewPickup => false;

        public override string PickupText => "";
        public override string DescText => "Increase damage by <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style>. Taking damage to below <style=cIsHealth>" + d(Threshold) + " health</style> <style=cIsUtility>breaks</style> this item.";

        public override void Init()
        {
            Damage = ConfigOption(0.2f, "Damage", "Decimal. Per Stack. Vanilla is 0.2");
            Threshold = ConfigOption(0.25f, "Health Threshold", "Decimal. Vanilla is 0.25");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
            IL.RoR2.HealthComponent.UpdateLastHitTime += ChangeThreshold;
        }

        private void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld<HealthComponent.ItemCounts>("fragileDamageBonus"),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out _),
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<HealthComponent>("get_isHealthLow")
            ))
            {
                c.Index += 5;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, HealthComponent, bool>>((Check, self) =>
                {
                    if ((self.health + self.shield) / self.fullCombinedHealth < Threshold)
                    {
                        Check = true;
                        return Check;
                    }
                    else
                    {
                        Check = false;
                        return Check;
                    }
                });
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Delicate Watch Threshold hook");
            }
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBle(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(1),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.2f)))
            {
                c.Index += 5;
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Delicate Watch Damage hook");
            }
        }
    }
}