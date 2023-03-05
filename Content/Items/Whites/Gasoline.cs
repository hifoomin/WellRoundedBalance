using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    internal class Gasoline : ItemBase
    {
        public override string Name => ":: Items : Whites :: Gasoline";

        public override string InternalPickupToken => "igniteOnKill";

        public override string PickupText => "Killing an enemy ignites other nearby enemies.";

        public override string DescText =>
            StackDesc(baseRange, rangePerStack, init => $"Killing an enemy <style=cIsDamage>ignites</style> all enemies within <style=cIsDamage>{m(init)}</style>{{Stack}}", m) +
            StackDesc(explosionDamage, explosionDamageStack, init => $" for <style=cIsDamage>{d(init)}</style>{{Stack}} base damage.", d) +
            StackDesc(baseBurnDamage, burnDamagePerStack, init => ((explosionDamage > 0 || explosionDamageStack > 0) ? " Additionally, enemies <style=cIsDamage>burn</style>" : ", <style=cIsDamage>burning</style> enemies") + $" for <style=cIsDamage>{d(init)}</style>{{Stack}} base damage.", d);

        [ConfigField("Explosion Damage", "Decimal.", 1f)]
        public static float explosionDamage;

        [ConfigField("Explosion Damage per Stack", "Decimal.", 0f)]
        public static float explosionDamageStack;

        [ConfigField("Explosion Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float explosionDamageIsHyperbolic;

        [ConfigField("Explosion Proc Coefficient", "Decimal.", 0f)]
        public static float explosionProcCoefficient;

        [ConfigField("Base Burn Damage", "Decimal.", 2f)]
        public static float baseBurnDamage;

        [ConfigField("Burn Damage Per Stack", "Decimal.", 2f)]
        public static float burnDamagePerStack;

        [ConfigField("Burn Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float burnDamageIsHyperbolic;

        [ConfigField("Base Range", 12f)]
        public static float baseRange;

        [ConfigField("Range Per Stack", 0f)]
        public static float rangePerStack;

        [ConfigField("Range is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float rangeIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += GlobalEventManager_ProcIgniteOnKill;
        }

        private void GlobalEventManager_ProcIgniteOnKill(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchStloc(0)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_2);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<CharacterBody, int, float>>((self, stack) => self.radius + StackAmount(baseRange, rangePerStack, stack, rangeIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Gasoline Radius hook");
            if (c.TryGotoNext(x => x.MatchStloc(2)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(explosionDamage, explosionDamageStack, stack, explosionDamageIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Gasoline Explosion Damage hook");
            if (c.TryGotoNext(x => x.MatchStloc(5)) && c.TryGotoPrev(MoveType.After, x => x.MatchMul()))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(baseBurnDamage, burnDamagePerStack, stack, burnDamageIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Gasoline Burn Damage hook");
            if (c.TryGotoNext(x => x.MatchStfld<BlastAttack>(nameof(BlastAttack.procCoefficient))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_R4, explosionProcCoefficient);
            }
            else Main.WRBLogger.LogError("Failed to apply Gasoline Explosion Damage hook");
        }
    }
}