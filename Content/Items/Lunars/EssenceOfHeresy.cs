using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    internal class EssenceOfHeresy : ItemBase
    {
        // TODO: Make it only apply up to 10-15 stacks per victim
        public override string Name => ":: Items ::::: Lunars :: Essence of Heresy";

        public override string InternalPickupToken => "lunarSpecialReplacement";

        public override string PickupText => "Replace your Special Skill with 'Ruin'.";

        public override string DescText => "<style=cIsUtility>Replace your Special Skill</style> with <style=cIsUtility>Ruin</style>. \n\nDealing damage has a <style=cIsDamage>50%</style> chance to add a stack of <style=cIsDamage>Ruin</style> for 10 <style=cStack>(+10 per stack)</style> seconds. Activating the skill <style=cIsDamage>detonates</style> all Ruin stacks at unlimited range, dealing <style=cIsDamage>300% damage</style> plus <style=cIsDamage>120% damage per stack of Ruin</style>. Recharges after 8 <style=cStack>(+8 per stack)</style> seconds.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.LunarDetonatorPassiveAttachment.DamageListener.OnDamageDealtServer += DamageListener_OnDamageDealtServer;
        }

        private void DamageListener_OnDamageDealtServer(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(100f)))
            {
                c.Next.Operand = 50f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Essence of Heresy Duration hook");
            }
        }
    }
}