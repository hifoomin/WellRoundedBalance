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
        public override string PickupText => "Increase your damage by " + d(DamageIncrease) + "... <color=#FF7F7F>BUT reduce your health by " + d(1 / HpDecrease) + ".</color>";
        public override string DescText => "Increase base damage by <style=cIsDamage>" + d(DamageIncrease) + "</style> <style=cStack>(+" + d(DamageIncrease) + " per stack)</style>. <style=cIsHealing>Reduce maximum health by " + d(1 / HpDecrease) + "</style> <style=cStack>(+" + d(1 / HpDecrease) + " per stack)</style>.";

        public override void Init()
        {
            DamageIncrease = ConfigOption(1f, "Damage", "Decimal. Per Stack. Vanilla is 1");
            HpDecrease = ConfigOption(0.5f, "Curse Gain", "Decimal. Per Stack. Vanilla is 0.5");
            LinearDamage = ConfigOption(false, "Should the Damage Increase be Linear?", "Vanilla is false");
            a = ScriptableObject.CreateInstance<ItemDef>();
            a.nameToken = "UCR_USELESS_STUPID_ITEM";
            a.deprecatedTier = ItemTier.NoTier;
            a.name = "ucr_stupid_item";
            LanguageAPI.Add("UCR_USELESS_STUPID_ITEM", "UCR Stupid Item");
            ContentAddition.AddItemDef(a);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        private void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.LunarDagger);
                if (stack > 0)
                {
                    args.damageMultAdd += LinearDamage ? DamageIncrease * stack : Mathf.Pow(DamageIncrease, stack);
                    args.baseCurseAdd += Mathf.Pow(HpDecrease, stack) - 1;
                }
            }
        }

        private void Changes(ILContext il)
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
        }
    }
}