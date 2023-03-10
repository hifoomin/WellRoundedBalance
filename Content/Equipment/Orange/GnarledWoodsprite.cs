using MonoMod.Cil;

namespace WellRoundedBalance.Equipment.Orange
{
    public class GnarledWoodsprite : EquipmentBase
    {
        public override string Name => ":: Equipment :: Gnarled Woodsprite";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.PassiveHealing;
        public override string PickupText => "Heal over time. Activate to send to an ally.";

        public override string DescText => "Gain a Woodsprite follower that heals a friendly target for <style=cIsHealing>" + d(activationPercentHealing) + " of their maximum health</style> instantly, then <style=cIsHealing>" + d(passivePercentHealing) + " of your maximum health</style> every second. \n\nActivating the equipment assigns a new target, or yourself if there are no targets available.";

        [ConfigField("Cooldown", "", 15f)]
        public static float cooldown;

        [ConfigField("Activation Percent Healing", "Decimal.", 0.1f)]
        public static float activationPercentHealing;

        [ConfigField("Passive Percent Healing", "Decimal.", 0.015f)]
        public static float passivePercentHealing;

        [ConfigField("Passive Healing Interval", "", 1f)]
        public static float passiveHealingInterval;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FirePassiveHealing += ChangeBurst;
            Changes();

            var Woodsprite = Utils.Paths.EquipmentDef.PassiveHealing.Load<EquipmentDef>();
            Woodsprite.cooldown = cooldown;
        }

        private void ChangeBurst(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = activationPercentHealing;
            }
            else
            {
                Logger.LogError("Failed to apply Gnarled Woodsprite Burst Healing hook");
            }
        }

        private void Changes()
        {
            var wood = Utils.Paths.GameObject.HealingFollower.Load<GameObject>().GetComponent<HealingFollowerController>();
            wood.fractionHealthBurst = activationPercentHealing;
            wood.fractionHealthHealing = passivePercentHealing;
            wood.healingInterval = passiveHealingInterval;
        }
    }
}