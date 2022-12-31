using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Items.Lunars
{
    public class ShapedGlass : ItemBase
    {
        public static ItemDef a;

        public override string Name => ":: Items ::::: Lunars :: Shaped Glass";
        public override string InternalPickupToken => "lunarDagger";

        public override string PickupText => "Increase your damage by 100%... <color=#FF7F7F>BUT reduce your health by 50%.</color>";
        public override string DescText => "Increase base damage by <style=cIsDamage>100%</style> <style=cStack>(+100% per stack)</style>. <style=cIsHealing>Reduce maximum health by 50%</style> <style=cStack>(+50% per stack)</style>.";

        public override void Init()
        {
            a = ScriptableObject.CreateInstance<ItemDef>();
            a.nameToken = "WRB_USELESS_STUPID_ITEM";
            a.deprecatedTier = ItemTier.NoTier;
            a.name = "WRB_stupid_item";
            LanguageAPI.Add("WRB_USELESS_STUPID_ITEM", "WRB Stupid Item");
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
                    args.damageMultAdd += 1f * stack;
                    args.baseCurseAdd += Mathf.Pow(1 / 0.5f, stack) - 1;
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
                Main.WRBLogger.LogError("Failed to apply Shaped Glass Damage hook");
            }
        }
    }
}