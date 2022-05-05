using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class Mocha : ItemBase
    {
        public static float AttackSpeed;
        public static float MoveSpeed;

        public override string Name => ":: Items : Whites :: Mocha";
        public override string InternalPickupToken => "attackSpeedAndMoveSpeed";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>" + d(AttackSpeed) + "</style> <style=cStack>(+" + d(AttackSpeed) + " per stack)</style> and <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(MoveSpeed) + "</style> <style=cStack>(+" + d(MoveSpeed) + " per stack)</style>.";

        public override void Init()
        {
            AttackSpeed = ConfigOption(0.075f, "Attack Speed", "Decimal. Per Stack. Vanilla is 0.075");
            MoveSpeed = ConfigOption(0.07f, "Move Speed", "Decimal. Per Stack. Vanilla is 0.07");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeMoveSpeed;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeAttackSpeed;
        }

        private void ChangeMoveSpeed(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.07f)
            );
            c.Next.Operand = MoveSpeed;
        }

        private void ChangeAttackSpeed(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.075f)
            );
            c.Next.Operand = AttackSpeed;
        }
    }
}