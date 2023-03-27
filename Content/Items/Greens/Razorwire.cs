using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Orbs;

namespace WellRoundedBalance.Items.Greens
{
    public class Razorwire : ItemBase
    {
        public static BuffDef razorwireCooldown;

        public override string Name => ":: Items :: Greens :: Razorwire";
        public override ItemDef InternalPickup => RoR2Content.Items.Thorns;

        public override string PickupText => "Retaliate upon taking damage.";
        public override string DescText => "Getting hit causes a razor to <style=cIsDamage>retaliate</style>, dealing <style=cIsDamage>" + d(baseDamage) + "</style> <style=cStack>(+" + d(damagePerStack) + " per stack)</style> damage.";

        [ConfigField("Base Damage", "Decimal.", 4f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 2f)]
        public static float damagePerStack;

        [ConfigField("Proc Chance", 0f)]
        public static float procChance;
        public override void Init()
        {
            razorwireCooldown = ScriptableObject.CreateInstance<BuffDef>();

            razorwireCooldown.isHidden = true;
            razorwireCooldown.isCooldown = false;
            razorwireCooldown.isDebuff = false;
            razorwireCooldown.canStack = false;
            razorwireCooldown.name = "Razorwire Cooldown";

            ContentAddition.AddBuffDef(razorwireCooldown);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage1;
        }

        private void HealthComponent_TakeDamage1(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var victimBody = self.body;

            if (victimBody)
            {
                var attacker = damageInfo.attacker;
                if (attacker)
                {
                    if (attacker != self.gameObject)
                    {
                        var inventory = victimBody.inventory;
                        if (inventory)
                        {
                            var stack = inventory.GetItemCount(RoR2Content.Items.Thorns);
                            if (stack > 0 && !damageInfo.procChainMask.HasProc(ProcType.Thorns))
                            {
                                var attackerHurtBox = Util.FindBodyMainHurtBox(attacker);
                                if (attackerHurtBox && !victimBody.HasBuff(razorwireCooldown))
                                {
                                    LightningOrb lightningOrb = new()
                                    {
                                        attacker = self.body.gameObject,
                                        bouncedObjects = null,
                                        bouncesRemaining = 0,
                                        damageCoefficientPerBounce = 1f,
                                        damageColorIndex = DamageColorIndex.Item,
                                        damageValue = self.body.damage * baseDamage + damagePerStack * (stack - 1),
                                        isCrit = victimBody.RollCrit(),
                                        lightningType = LightningOrb.LightningType.RazorWire,
                                        origin = damageInfo.position,
                                        procChainMask = default,
                                        procCoefficient = procChance * globalProc,
                                        range = 100000f,
                                        teamIndex = victimBody.teamComponent.teamIndex,
                                        target = attackerHurtBox,
                                    };
                                    lightningOrb.procChainMask.AddProc(ProcType.Thorns);

                                    OrbManager.instance.AddOrb(lightningOrb);

                                    victimBody.AddTimedBuff(razorwireCooldown, 1f, 1);
                                }
                            }
                        }
                    }
                }
            }

            orig(self, damageInfo);
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "thorns"),
                 x => x.MatchLdcI4(0)))
            {
                c.Index += 1;
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, int.MaxValue);
            }
            else
            {
                Logger.LogError("Failed to apply Razorwire Deletion hook");
            }
        }
    }
}