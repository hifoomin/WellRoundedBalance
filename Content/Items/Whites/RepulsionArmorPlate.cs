using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class RepulsionArmorPlate : ItemBase<RepulsionArmorPlate>
    {
        public override string Name => ":: Items : Whites :: Repulsion Armor Plate";
        public override ItemDef InternalPickup => RoR2Content.Items.ArmorPlate;

        public override string PickupText => "Receive flat damage reduction from all attacks.";

        public override string DescText =>
            StackDesc(flatDamageReduction, flatDamageReductionStack, init => $"Reduce all <style=cIsDamage>incoming damage</style> by <style=cIsDamage>{init}</style>{{Stack}}. ") + "Cannot be reduced below " +
            StackDesc(minimumDamage, minimumDamageStack, init => $"<style=cIsDamage>{init}</style>{{Stack}}") +
            StackDesc(minimumPercentDamage, minimumPercentDamageStack, init => (minimumDamage > 0 || minimumDamageStack > 0 ? "or " : "") + $"<style=cIsDamage>{d(init)}</style>{{Stack}} of <style=cIsHealing>maximum health</style>", d) + ".";

        [ConfigField("Flat Damage Reduction", 5f)]
        public static float flatDamageReduction;

        [ConfigField("Flat Damage Reduction per Stack", 5f)]
        public static float flatDamageReductionStack;

        [ConfigField("Flat Damage Reduction is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float flatDamageReductionIsHyperbolic;

        [ConfigField("Minimum Damage", 10f)]
        public static float minimumDamage;

        [ConfigField("Minimum Damage per Stack", 0f)]
        public static float minimumDamageStack;

        [ConfigField("Minimum Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float minimumDamageIsHyperbolic;

        [ConfigField("Minimum Percent Damage", "Decimal.", 0f)]
        public static float minimumPercentDamage;

        [ConfigField("Minimum Percent Damage per Stack", "Decimal.", 0f)]
        public static float minimumPercentDamageStack;

        [ConfigField("Minimum Percent Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float minimumPercentDamageIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        public static void HealthComponent_TakeDamageProcess(ILContext il)
        {
            ILCursor c = new(il);
            int dmg = -1;
            c.TryGotoNext(x => x.MatchLdfld<DamageInfo>(nameof(DamageInfo.damage)), x => x.MatchStloc(out dmg));
            if (c.TryGotoNext(x => x.MatchLdfld<HealthComponent.ItemCounts>(nameof(HealthComponent.ItemCounts.armorPlate))) && c.TryGotoNext(x => x.MatchStloc(dmg)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, dmg);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, HealthComponent, float>>((orig, self) =>
                {
                    float minFlat = StackAmount(minimumDamage, minimumDamageStack, self.itemCounts.armorPlate, minimumDamageIsHyperbolic);
                    float minPercent = self.fullHealth * StackAmount(minimumPercentDamage, minimumPercentDamageStack, self.itemCounts.armorPlate, minimumPercentDamageIsHyperbolic);
                    float reduction = StackAmount(flatDamageReduction, flatDamageReductionStack, self.itemCounts.armorPlate, flatDamageReductionIsHyperbolic);
                    return Mathf.Max(minFlat, minPercent, orig - reduction);
                });
            }
            else Logger.LogError("Failed to apply Repulsion Armor Plate hook");
        }
    }
}