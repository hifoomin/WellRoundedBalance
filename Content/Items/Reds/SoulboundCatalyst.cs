using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class SoulboundCatalyst : ItemBase
    {
        public static float BaseCdr;
        public static float StackCdr;
        public override string Name => ":: Items ::: Reds :: Soulbound Catalyst";
        public override string InternalPickupToken => "talisman";

        public override string PickupText => "Kills reduce equipment cooldown.";

        public override string DescText => "<style=cIsDamage>Kills reduce</style> <style=cIsUtility>equipment cooldown</style> by <style=cIsUtility>" + baseEquipmentCooldownReduction + "s</style> <style=cStack>(+" + equipmentCooldownReductionPerStack + "s per stack)</style>.";

        [ConfigField("Base Equipment Cooldown Reduction", 3f)]
        public static float baseEquipmentCooldownReduction;

        [ConfigField("Equipment Cooldown Reduction Per Stack", 2f)]
        public static float equipmentCooldownReductionPerStack;

        public override void Init()
        {
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
                c.Next.Operand = baseEquipmentCooldownReduction - equipmentCooldownReductionPerStack;
                c.Index += 2;
                c.Next.Operand = equipmentCooldownReductionPerStack;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Soulbound Catalyst Cooldown hook");
            }
        }
    }
}