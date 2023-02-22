using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Reds
{
    public class LaserScope : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Laser Scope";
        public override string InternalPickupToken => "critDamage";

        public override string PickupText => "Remove bullet falloff. Your 'Critical Strikes' deal an additional 50% damage.";

        public override string DescText => "Remove <style=cIsUtility>bullet falloff</style>. Gain <style=cIsDamage>10% critical chance</style>. <style=cIsDamage>Critical Strikes</style> deal an additional <style=cIsDamage>50%</style> <style=cStack>(+50% per stack)</style> damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            // IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            IL.RoR2.BulletAttack.CalcFalloffFactor += BulletAttack_CalcFalloffFactor;
        }

        private void BulletAttack_CalcFalloffFactor(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f),
                x => x.MatchMul()))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Laser Scope Falloff Removal 1 hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.75f),
                x => x.MatchMul()))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Laser Scope Falloff Removal 2 hook");
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("set_attackSpeed"),
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(2f),
                x => x.MatchLdcR4(1f)))
            {
                c.Index += 4;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((useless, self) =>
                {
                    return 2f + 1f * (self.inventory.GetItemCount(DLC1Content.Items.CritDamage) - 1);
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Laser Scope Crit Damage hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.CritDamage);
                if (stack > 0)
                {
                    args.critAdd += 5f;
                }
            }
        }
    }
}