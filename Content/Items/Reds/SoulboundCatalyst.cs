using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class SoulboundCatalyst : ItemBase<SoulboundCatalyst>
    {
        public override string Name => ":: Items ::: Reds :: Soulbound Catalyst";
        public override ItemDef InternalPickup => RoR2Content.Items.Talisman;

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
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
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
                Logger.LogError("Failed to apply Soulbound Catalyst Cooldown hook");
            }
        }
    }
}