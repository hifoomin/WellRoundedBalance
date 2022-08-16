/*
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Lunars
{
    public class ShapedGlass : ItemBase
    {
        public static float DamageIncrease;
        public static float HpDecrease;
        public static bool LinearDamage;
        public static ItemDef a;

        public override string Name => ":: Items ::::: Lunars :: Shaped Glass";
        public override string InternalPickupToken => "lunarDagger";
        public override bool NewPickup => true;
        public override string PickupText => "Increase your damage by " + d(DamageIncrease) + "... <color=#FF7F7F>BUT reduce your health by " + d(HpDecrease) + ".</color>";
        public override string DescText => "Increase <style=cIsHealing>max health</style> by <style=cIsHealing>" + d(DamageIncrease) + " <style=cStack>(+" + d(DamageIncrease) + " per stack)</style></style>. Reduce <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(HpDecrease) + "</style> <style=cStack>(+" + d(HpDecrease) + " per stack)</style>.";

        public override void Init()
        {
            DamageIncrease = ConfigOption(2f, "Damage", "Decimal. Per Stack. Vanilla is 2");
            HpDecrease = ConfigOption(0.5f, "Max HP Decrease", "Decimal. Per Stack. Vanilla is 0.5");
            LinearDamage = ConfigOption(false, "Should the Damage Increase be Linear?", "Vanilla is false");
            a = ScriptableObject.CreateInstance<ItemDef>();
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealth;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeDamage;
            RecalculateStatsAPI.GetStatCoefficients += ChangeDamage2;
        }

        private void ChangeDamage2(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.LunarDagger);
                args.damageMultAdd += LinearDamage ? DamageIncrease * stack : Mathf.Pow(DamageIncrease, stack - 1);
            }
        }

        private void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Items), "LunarDagger")))
            {
                c.Remove();
                c.Emit<ShapedGlass>(OpCodes.Ldsfld, nameof(a));
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Shaped Glass Damage hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(2f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchCallOrCallvirt<UnityEngine.Mathf>("Pow"),
                    x => x.MatchLdcR4(1f)))
            {
                c.Index += 1;
                c.Next.Operand = 0f;
                c.Index += 4;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Shaped Glass Damage hook");
            }
        }

        private void ChangeHealth(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(2f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchCallOrCallvirt<UnityEngine.Mathf>("Pow"),
                    x => x.MatchCallOrCallvirt<CharacterBody>("set_cursePenalty")))
            {
                c.Next.Operand = 1f / HpDecrease;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Shaped Glass Health hook");
            }
        }
    }
}
*/