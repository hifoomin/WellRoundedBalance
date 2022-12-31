using MonoMod.Cil;

namespace WellRoundedBalance.Equipment
{
    public class JadeElephant : EquipmentBase
    {
        public override string Name => "::: Equipment :: Jade Elephant";
        public override string InternalPickupToken => "gainArmor";

        public override string PickupText => "Gain massive armor for 10 seconds.";

        public override string DescText => "Gain <style=cIsDamage>200 armor</style> for <style=cIsUtility>10 seconds.</style>";

        public override void Init()
        {
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
                c.Next.Operand = 200f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Jade Elephant Armor hook");
            }
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(5f)))
            {
                c.Next.Operand = 10f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Jade Elephant Duration hook");
            }
        }
    }
}