using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class JadeElephant : EquipmentBase
    {
        public override string Name => "::: Equipment :: Jade Elephant";
        public override string InternalPickupToken => "gainArmor";

        public override bool NewPickup => true;

        public override bool NewDesc => true;

        public override string PickupText => "Gain massive armor for " + Duration + " seconds.";

        public override string DescText => "Gain <style=cIsDamage>" + Armor + " armor</style> for <style=cIsUtility>" + Duration + " seconds.</style>";

        public static float Armor;
        public static float Duration;

        public override void Init()
        {
            Armor = ConfigOption(500f, "Armor", "Vanilla is 500");
            Duration = ConfigOption(5, "Duration", "Vanilla is 5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireGainArmor += ChangeDuration;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeArmor;
        }

        private void ChangeArmor(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(500f)))
            {
                c.Next.Operand = Armor;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Jade Elephant Armor hook");
            }
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(5f)))
            {
                c.Next.Operand = Duration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Jade Elephant Duration hook");
            }
        }
    }
}