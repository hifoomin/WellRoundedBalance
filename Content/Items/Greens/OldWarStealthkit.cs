﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Items;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class OldWarStealthkit : ItemBase<OldWarStealthkit>
    {
        public override string Name => ":: Items :: Greens :: Old War Stealthkit";
        public override ItemDef InternalPickup => RoR2Content.Items.Phasing;

        public override string PickupText => "Turn invisible at low health.";

        public override string DescText => "Falling below <style=cIsHealth>" + d(healthThreshold) + " health</style> causes you to gain <style=cIsUtility>40% movement speed</style> and <style=cIsUtility>invisibility</style> for <style=cIsUtility>5s</style>. Recharges every <style=cIsUtility>30 seconds</style> <style=cStack>(-50% per stack)</style>.";

        [ConfigField("Health Threshold", "Decimal.", 0.5f)]
        public static float healthThreshold;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.PhasingBodyBehavior.FixedUpdate += ChangeThreshold;

            Mechanics.Health.Fragile.AddFragileItem(InternalPickup, new Mechanics.Health.Fragile.FragileInfo { fraction = healthThreshold * 100f });
        }

        private void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<HealthComponent>("get_isHealthLow")
            ))
            {
                c.Index += 1;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, PhasingBodyBehavior, bool>>((Check, self) =>
                {
                    if ((self.body.healthComponent.health + self.body.healthComponent.shield) / self.body.healthComponent.fullCombinedHealth < healthThreshold)
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
                Logger.LogError("Failed to apply Old War Stealthkit Threshold hook");
            }
        }
    }
}