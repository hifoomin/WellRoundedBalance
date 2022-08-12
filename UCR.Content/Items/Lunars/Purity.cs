using MonoMod.Cil;

namespace UltimateCustomRun.Items.Lunars
{
    public class Purity : ItemBase
    {
        public static float Cdr;
        public static float StackCdr;

        public override string Name => ":: Items ::::: Lunars :: Purity";
        public override string InternalPickupToken => "lunarBadLuck";
        public override bool NewPickup => true;
        public override string PickupText => "Reduce your skill cooldowns by 2 seconds. <color=#FF7F7F>You are unlucky.</color>";
        public override string DescText => "Using a Shrine summons <style=cIsHealth>enemies</style> nearby. <style=cIsUtility>Scales over time.</style>";

        public override void Init()
        {
            Cdr = ConfigOption(2f, "Base Flat Cooldown Reduction", "Vanilla is 2");
            StackCdr = ConfigOption(1f, "Stack Flat Cooldown Reduction", "Per Stack. Vanilla is 1");
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
                c.Next.Operand = Cdr;
                c.Index += 1;
                c.Next.Operand = StackCdr;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Purity Cooldown Reduction hook");
            }
        }
    }
}