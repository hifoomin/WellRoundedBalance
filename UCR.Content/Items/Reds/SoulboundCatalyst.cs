using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class SoulboundCatalyst : ItemBase
    {
        public static float BaseCdr;
        public static float StackCdr;
        public override string Name => ":: Items ::: Reds :: Soulbound Catalyst";
        public override string InternalPickupToken => "talisman";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>Kills reduce</style> <style=cIsUtility>equipment cooldown</style> by <style=cIsUtility>" + BaseCdr + "s</style> <style=cStack>(+" + StackCdr + "s per stack)</style>.";

        public override void Init()
        {
            BaseCdr = ConfigOption(4f, "Base Equipment Cooldown Reduction", "Vanilla is 4");
            StackCdr = ConfigOption(2f, "Stack Equipment Cooldown Reduction", "Per Stack. Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(2f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(2f)))
            {
                c.Index += 1;
                c.Next.Operand = BaseCdr - StackCdr;
                c.Index += 2;
                c.Next.Operand = StackCdr;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Soulbound Catalyst Cooldown hook");
            }
        }
    }
}