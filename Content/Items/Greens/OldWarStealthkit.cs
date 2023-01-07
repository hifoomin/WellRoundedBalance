using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Items;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class OldWarStealthkit : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Old War Stealthkit";
        public override string InternalPickupToken => "phasing";

        public override string PickupText => "Turn invisible at low health.";

        public override string DescText => "Falling below <style=cIsHealth>50% health</style> causes you to gain <style=cIsUtility>40% movement speed</style> and <style=cIsUtility>invisibility</style> for <style=cIsUtility>5s</style>. Recharges every <style=cIsUtility>30 seconds</style> <style=cStack>(-50% per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.PhasingBodyBehavior.FixedUpdate += ChangeThreshold;
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
                    if ((self.body.healthComponent.health + self.body.healthComponent.shield) / self.body.healthComponent.fullCombinedHealth <= 0.5f)
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
                Main.WRBLogger.LogError("Failed to apply Old War Stealthkit Threshold hook");
            }

            // this NREs
        }
    }
}