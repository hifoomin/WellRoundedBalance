namespace WellRoundedBalance.Allies
{
    internal class EquipmentDrone : AllyBase<EquipmentDrone>
    {
        public override string Name => ":: Allies :: Equipment Drone";

        [ConfigField("Equipment Cooldown Reduction", "Formula for Equipment Cooldown Reduction: Equipment Cooldown Reduction ^ 15", 0.94f)]
        public static float cdr;

        public override void Hooks()
        {
            IL.RoR2.Inventory.CalculateEquipmentCooldownScale += Inventory_CalculateEquipmentCooldownScale;
        }

        private void Inventory_CalculateEquipmentCooldownScale(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.9f)))
            {
                c.Next.Operand = cdr;
            }
            else
            {
                Logger.LogError("Failed to apply Boost Equipment Recharge Cooldown Reduction hook");
            }
        }
    }
}