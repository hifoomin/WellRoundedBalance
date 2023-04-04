using MonoMod.Cil;

namespace WellRoundedBalance.Equipment.Orange
{
    public class JadeElephant : EquipmentBase<JadeElephant>
    {
        public override string Name => ":: Equipment :: Jade Elephant";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.GainArmor;

        public override string PickupText => "Gain massive armor for " + buffDuration + " seconds.";

        public override string DescText => "Gain <style=cIsDamage>" + armorGain + " armor</style> for <style=cIsUtility>" + buffDuration + " seconds.</style>";

        [ConfigField("Armor Gain", "", 200f)]
        public static float armorGain;

        [ConfigField("Buff Duration", "", 10f)]
        public static float buffDuration;

        [ConfigField("Cooldown", "", 45f)]
        public static float cooldown;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireGainArmor += ChangeDuration;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeArmor;

            var Jade = Utils.Paths.EquipmentDef.GainArmor.Load<EquipmentDef>();
            Jade.cooldown = cooldown;
        }

        private void ChangeArmor(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(500f)))
            {
                c.Next.Operand = armorGain;
            }
            else
            {
                Logger.LogError("Failed to apply Jade Elephant Armor hook");
            }
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(5f)))
            {
                c.Next.Operand = buffDuration;
            }
            else
            {
                Logger.LogError("Failed to apply Jade Elephant Duration hook");
            }
        }
    }
}