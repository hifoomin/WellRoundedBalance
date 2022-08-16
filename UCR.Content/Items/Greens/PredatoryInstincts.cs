using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Greens
{
    public class PredatoryInstincts : ItemBase
    {
        // TODO: FIX ALL THE CODE AAAAAAAAA WHY DOESNT IT WORK RANDOMLY
        public static float AttackSpeed;

        public static int BaseCap;
        public static int StackCap;
        public static float Crit;
        public static bool StackCrit;
        public static float Speed;

        public override string Name => ":: Items :: Greens :: Predatory Instincts";
        public override string InternalPickupToken => "attackSpeedOnCrit";
        public override bool NewPickup => true;

        public override string PickupText => "'Critical Strikes' increase" +
                                             (AttackSpeed != 0f ? " attack speed" : "") +
                                             (AttackSpeed != 0f && Speed != 0f ? " and" : "") +
                                             (Speed != 0f ? " movement speed" : ".") +
                                             "Stacks 3 times.";

        public override string DescText => "<style=cIsDamage>Critical strikes</style> increase" +
                                           (AttackSpeed != 0f ? " <style=cIsDamage>attack speed</style> by <style=cIsDamage>" + d(AttackSpeed) + "</style>" +
                                           (AttackSpeed != 0f && Speed != 0f ? " and" : "") : "") +
                                           (Speed != 0f ? " <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(Speed) + "</style>" : "") +
                                           " up to " + BaseCap + " <style=cStack>(+" + StackCap + " per stack)</style> times.";

        public override void Init()
        {
            AttackSpeed = ConfigOption(0.12f, "Buff Attack Speed", "Decimal. Per Buff. Vanilla is 0.12");
            BaseCap = ConfigOption(3, "Base Buff Cap", "Vanilla is 3");
            StackCap = ConfigOption(2, "Stack Buff Cap", "Per Stack. Vanilla is 2");
            Crit = ConfigOption(5f, "Crit Chance", "Vanilla is 5");
            StackCrit = ConfigOption(false, "Stack Crit Chance?", "Vanilla is false");
            Speed = ConfigOption(0f, "Buff Speed", "Decimal. Per Buff. Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += ChangeCapReal;
            // IL.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += ChangeCap;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeAS;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "AttackSpeedOnCrit"),
                    x => x.MatchCallOrCallvirt<CharacterBody>("GetBuffCount"),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.12f)))
            {
                c.Index += 3;
                //c.Remove();
                //c.Emit(OpCodes.Ldc_R4, AttackSpeed);
                c.Next.Operand = AttackSpeed;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Predatory Instincts Buff Attack Speed hook");
            }
        }

        public static void ChangeCap(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(1),
                x => x.MatchLdcI4(1),
                x => x.MatchLdloc(1),
                x => x.MatchLdcI4(2)
            );
            c.Index += 1;
            //c.Next.Operand = BaseCap - StackCap;
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, BaseCap - StackCap);
            c.Index += 2;
            //c.Next.Operand = StackCap;
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, StackCap);
        }

        public static void ChangeCapReal(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(3),
                    x => x.MatchLdloc(2)))
            {
                c.Next.Operand = BaseCap - StackCap;
                // c.Remove();
                // c.Emit(OpCodes.Ldc_I4, BaseCap - StackCap);

                c.Index += 1;
                c.Next.Operand = StackCap;
                // c.Remove();
                // c.Emit(OpCodes.Ldc_I4, StackCap);
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Predatory Instincts Buff Cap hook");
            }
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.AttackSpeedOnCrit);
                int buff = sender.GetBuffCount(RoR2Content.Buffs.AttackSpeedOnCrit);
                if (stack > 0)
                {
                    args.critAdd += StackCrit ? Crit * stack : Crit;

                    if (buff > 0)
                    {
                        args.moveSpeedMultAdd += Speed * buff;
                    }
                }
            }
        }

        // NONE OF THESE WORK PLEASE HELP TO FIX
    }
}