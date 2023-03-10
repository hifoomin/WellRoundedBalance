using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class OldGuillotine : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Old Guillotine";
        public override string InternalPickupToken => "executeLowHealthElite";

        public override string PickupText => "Instantly kill low health Elite monsters.";

        public override string DescText => "Instantly kill Elite monsters below <style=cIsHealth>" + healthThreshold + "% <style=cStack>(+" + healthThreshold + "% per stack)</style> health</style>.";

        [ConfigField("Health Threshold", 25f)]
        public static float healthThreshold;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.OnInventoryChanged += ChangeThreshold;
        }

        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(13f)))
            {
                c.Next.Operand = healthThreshold;
            }
            else
            {
                Logger.LogError("Failed to apply Old Guillotine Threshold hook");
            }
        }
    }
}