using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class StoneFluxPauldron : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Stone Flux Pauldron";
        public override string InternalPickupToken => "halfSpeedDoubleHealth";

        public override string PickupText => "Pull enemies on hit... <color=#FF7F7F>BUT enemies pull you on hit.</color>\n";

        public override string DescText => "Pull enemies on hit. Enemies pull you on hit. <style=cStack>(Pull strength increases per stack)</style>.";

        // slows aren't accurate in ror2
        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }

        // player pulling enemies on hit
        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            var body = damageReport.attackerBody;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    var damageInfo = damageReport.damageInfo;
                    var stack = inventory.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
                    float force = -4000f * damageInfo.procCoefficient * stack;
                    damageInfo.force = Vector3.Scale(damageInfo.force, -Vector3.one);
                    damageInfo.force += body.inputBank.GetAimRay().direction * force;
                    damageInfo.canRejectForce = false;
                    Main.WRBLogger.LogFatal("ERTHUEDUTGFRHUODRTGHJUOIDZSRGTJHUIDRG IT SHOULD BE FUCKING WORKING IM HITTING THE ENEMY");
                    Main.WRBLogger.LogFatal("damageInfo.force is " + damageInfo.force);
                }
            }
        }

        // enemies pulling player on hit
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker)
            {
                var body = self.body;
                if (body)
                {
                    var inventory = body.inventory;
                    var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (inventory && attackerBody)
                    {
                        var stack = inventory.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
                        float mass;
                        if (self.body.characterMotor) mass = self.body.characterMotor.mass;
                        else if (self.body.rigidbody) mass = self.body.rigidbody.mass;
                        else mass = 1f;

                        var force = 40f * stack;
                        damageInfo.force += Vector3.Normalize(attackerBody.corePosition - self.body.corePosition) * force * mass;
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(76),
                    x => x.MatchLdloc(44),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(1f)))
            {
                c.Index += 3;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Stone Flux Pauldron Speed hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdloc(63),
               x => x.MatchLdloc(44),
               x => x.MatchConvR4(),
               x => x.MatchLdcR4(1f)))
            {
                c.Index += 3;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Stone Flux Pauldron Health hook");
            }
        }
    }
}