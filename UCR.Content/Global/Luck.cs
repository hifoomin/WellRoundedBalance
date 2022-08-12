using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public class Luck : GlobalBase
    {
        public static float luck;
        public override string Name => ": Global :::: Damage";

        public override void Init()
        {
            luck = ConfigOption(0f, "Base Luck", "Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterMaster.OnInventoryChanged += ChangeLuck;
        }

        public static void ChangeLuck(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.0f)))
            {
                c.Next.Operand = luck;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Luck hook");
            }
        }
    }
}