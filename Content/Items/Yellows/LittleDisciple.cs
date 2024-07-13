using MonoMod.Cil;

namespace WellRoundedBalance.Items.Yellows
{
    public class LittleDisciple : ItemBase<LittleDisciple>
    {
        public override string Name => ":: Items :::: Yellows :: Little Disciple";
        public override ItemDef InternalPickup => RoR2Content.Items.SprintWisp;

        public override string PickupText => "Fire tracking wisps while sprinting.";

        public override string DescText => "Fire a <style=cIsDamage>tracking wisp</style> for <style=cIsDamage>300% <style=cStack>(+300% per stack)</style> damage</style>. Fires every <style=cIsUtility>1.6</style> seconds while sprinting. Fire rate increases with <style=cIsUtility>movement speed</style>.";

        [ConfigField("Range", 25f)]
        public static float range;

        [ConfigField("Proc Coefficient", 0.33f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Items.SprintWispBodyBehavior.Fire += SprintWispBodyBehavior_Fire;
            IL.RoR2.Items.SprintWispBodyBehavior.Fire += ChangeProcCoefficient;
        }

        private void SprintWispBodyBehavior_Fire(On.RoR2.Items.SprintWispBodyBehavior.orig_Fire orig, RoR2.Items.SprintWispBodyBehavior self)
        {
            RoR2.Items.SprintWispBodyBehavior.searchRadius = range;
            orig(self);
        }

        private void ChangeProcCoefficient(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchStfld("RoR2.Orbs.DevilOrb", "procCoefficient")))
            {
                c.Next.Operand = procCoefficient * Items.Greens._ProcCoefficients.globalProc;
            }
            else
            {
                Logger.LogError("Failed to apply Little Disciple Proc Coefficient hook");
            }
        }
    }
}