using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class Bandolier : ItemBase<Bandolier>
    {
        public override string Name => ":: Items :: Greens :: Bandolier";
        public override ItemDef InternalPickup => RoR2Content.Items.Bandolier;

        public override string PickupText => "Chance on kill to drop an ammo pack that resets all skill cooldowns.";

        public override string DescText => "<style=cIsUtility>" + Mathf.Round((1 - (1 / Mathf.Pow(baseValue + 1, exponent))) * 100f) + "%</style> <style=cStack>(+" + (Mathf.Round((1 - (1 / Mathf.Pow(baseValue + 2, exponent))) * 100f) - Mathf.Round((1 - (1 / Mathf.Pow(baseValue + 1, exponent))) * 100f)) + "% per stack)</style> chance on kill to drop an ammo pack that <style=cIsUtility>resets all skill cooldowns</style>.";

        [ConfigField("Base Value", "Formula for drop chance: 1 - 1 / (Base Value + Bandolier) ^ Exponent", 0.9f)]
        public static float baseValue;

        [ConfigField("Exponent", "Formula for drop chance: 1 - 1 / (Base Value + Bandolier) ^ Exponent", 0.18f)]
        public static float exponent;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcI4(1)))
            {
                c.Index += 3;
                c.Next.Operand = baseValue;
            }
            else
            {
                Logger.LogError("Failed to apply Bandolier Base hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchAdd(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.33f)))
            {
                c.Index += 2;
                c.Next.Operand = exponent;
            }
            else
            {
                Logger.LogError("Failed to apply Bandolier Exponent hook");
            }
        }
    }
}