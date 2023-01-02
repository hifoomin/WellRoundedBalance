using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidWhites
{
    public class WeepingFungus : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Weeping Fungus";
        public override string InternalPickupToken => "mushroomVoid";

        public override string PickupText => "Heal while sprinting. <style=cIsVoid>Corrupts all Bustling Fungi</style>.";
        public override string DescText => "<style=cIsHealing>Heals</style> for <style=cIsHealing>1%</style> <style=cStack>(+1% per stack)</style> of your <style=cIsHealing>health</style> every second <style=cIsUtility>while sprinting</style>. <style=cIsVoid>Corrupts all Bustling Fungi</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MushroomVoidBehavior.FixedUpdate += ChangeHealing;
        }

        private void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.01f)
            );
            c.Next.Operand = 0.01f * 0.5f;
        }
    }
}