using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class Purity : ItemBase<Purity>
    {
        public override string Name => ":: Items ::::: Lunars :: Purity";
        public override ItemDef InternalPickup => RoR2Content.Items.LunarBadLuck;

        public override string PickupText => "Reduce your skill cooldowns by " + baseFlatCooldownReduction + " seconds. <color=#FF7F7F>You are unlucky.</color>";
        public override string DescText => "All skill cooldowns are reduced by <style=cIsUtility>" + baseFlatCooldownReduction + "</style> <style=cStack>(+" + flatCooldownReductionPerStack + " per stack)</style> seconds. All random effects are rolled <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> times for an <style=cIsHealth>unfavorable outcome</style>.";

        [ConfigField("Base Flat Cooldown Reduction", "", 3f)]
        public static float baseFlatCooldownReduction;

        [ConfigField("Flat Cooldown Reduction Per Stack", "", 1f)]
        public static float flatCooldownReductionPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCdr;
        }

        private void ChangeCdr(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(2f),
                    x => x.MatchLdcR4(1f)))
            {
                c.Index += 1;
                c.Next.Operand = baseFlatCooldownReduction;
                c.Index += 1;
                c.Next.Operand = flatCooldownReductionPerStack;
            }
            else
            {
                Logger.LogError("Failed to apply Purity Cooldown Reduction hook");
            }
        }
    }
}