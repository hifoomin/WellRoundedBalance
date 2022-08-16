using MonoMod.Cil;

namespace UltimateCustomRun.Items.Whites
{
    public class PaulsGoatHoof : ItemBase
    {
        public static float Speed;

        public override string Name => ":: Items : Whites :: Pauls Goat Hoof";
        public override string InternalPickupToken => "hoof";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(Speed) + "</style> <style=cStack>(+" + d(Speed) + " per stack)</style>.";

        public override void Init()
        {
            Speed = ConfigOption(0.14f, "Movement Speed", "Decimal. Per Stack. Vanilla is 0.14");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
        }

        public static void ChangeSpeed(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.14f)))
            {
                c.Index += 1;
                c.Next.Operand = Speed;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Paul's Goat Hoof Speed hook");
            }
        }
    }
}