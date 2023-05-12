using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class ShapedGlass : ItemBase<ShapedGlass>
    {
        public override string Name => ":: Items ::::: Lunars :: Shaped Glass";
        public override ItemDef InternalPickup => RoR2Content.Items.LunarDagger;

        public override string PickupText => $"Increase your damage by {Damage * 100f}%... <color=#FF7F7F>BUT reduce your health by 50%.</color>";
        public override string DescText => $"Increase base damage by <style=cIsDamage>{Damage * 100f}%</style> <style=cStack>(+75% per stack)</style>. <style=cIsHealing>Reduce maximum health by 50%</style> <style=cStack>(+50% per stack)</style>.";

        [ConfigField("Damage Increase", "", 0.75f)]
        public static float Damage;
        public override void Init()
        {
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
                    args.damageMultAdd += Damage * stack;
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
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessItem));
            }
            else
            {
                Logger.LogError("Failed to apply Shaped Glass Damage hook");
            }
        }
    }
}