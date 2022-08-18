using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class PowerElixir : ItemBase
    {
        public static float Healing;
        public static float Threshold;
        public override string Name => ":: Items : Whites :: Power Elixir";
        public override string InternalPickupToken => "healingPotion";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Taking damage to below <style=cIsHealth>" + d(Threshold) + " health</style> <style=cIsUtility>consumes</style> this item, <style=cIsHealing>healing</style> you for <style=cIsHealing>" + d(Healing) + "</style> of <style=cIsHealing>maximum health</style>.";

        public override void Init()
        {
            Healing = ConfigOption(0.75f, "Percent Healing", "Decimal. Vanilla is 0.75");
            Threshold = ConfigOption(0.25f, "Health Threshold", "Decimal. Vanilla is 0.25");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.UpdateLastHitTime += ChangeHealing;
            IL.RoR2.HealthComponent.UpdateLastHitTime += ChangeThreshold;
        }

        private void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld<HealthComponent.ItemCounts>("healingPotion"),
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
                Main.UCRLogger.LogError("Failed to apply Power Elixir Threshold hook");
            }
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.75f)))
            {
                c.Next.Operand = Healing;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Power Elixir Healing hook");
            }
        }
    }
}