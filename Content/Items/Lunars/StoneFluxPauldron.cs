using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class StoneFluxPauldron : ItemBase<StoneFluxPauldron>
    {
        public override string Name => ":: Items ::::: Lunars :: Stone Flux Pauldron";
        public override ItemDef InternalPickup => DLC1Content.Items.HalfSpeedDoubleHealth;

        public override string PickupText => "Pull enemies on hit... <color=#FF7F7F>BUT enemies pull you on hit.</color>\n";

        public override string DescText => "Gain <style=cIsHealing>" + armorGain + " armor</style>. <style=cIsUtility>Pull</style> enemies on hit. Enemies <style=cIsUtility>pull</style> you on hit. <style=cStack>(Pull strength increases per stack)</style>.";

        [ConfigField("Armor Gain", "", 15f)]
        public static float armorGain;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
                if (stack > 0)
                {
                    args.armorAdd += armorGain;
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var body = self.body;
                if (body)
                {
                    var attackerBody = attacker.GetComponent<CharacterBody>();
                    var inventory = body.inventory;
                    if (attackerBody)
                    {
                        var inventory2 = attackerBody.inventory;
                        if (inventory)
                        {
                            // enemies pulling player on hit
                            var stack = inventory.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
                            float mass;
                            if (self.body.characterMotor) mass = self.body.characterMotor.mass;
                            else if (self.body.rigidbody) mass = self.body.rigidbody.mass;
                            else mass = 1f;

                            var force = 30f * damageInfo.procCoefficient * stack;
                            damageInfo.force += Vector3.Normalize(attackerBody.corePosition - self.body.corePosition) * force * mass;
                        }
                        if (inventory2 && damageInfo.procCoefficient > 0)
                        {
                            // player pulling enemies on hit
                            var stack = inventory2.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
                            float mass;
                            if (self.body.characterMotor) mass = self.body.characterMotor.mass;
                            else if (self.body.rigidbody) mass = self.body.rigidbody.mass;
                            else mass = 1f;

                            var force = 5f * damageInfo.procCoefficient * stack * Mathf.Sqrt(damageInfo.damage) * (mass / 3f);
                            damageInfo.force += Vector3.Normalize(attackerBody.corePosition - self.body.corePosition) * force;
                        }
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
                Logger.LogError("Failed to apply Stone Flux Pauldron Speed hook");
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
                Logger.LogError("Failed to apply Stone Flux Pauldron Health hook");
            }
        }
    }
}