/*
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class Shuriken : ItemBase
    {
        public static float Damage;
        public static float StackDamage;
        public static int Count;
        public static int StackCount;
        public static float RechargeTime;

        public override string Name => ":: Items :: Greens :: Shuriken";
        public override string InternalPickupToken => "primarySkillShuriken";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Activating your <style=cIsUtility>Primary skill</style> also throws a <style=cIsDamage>shuriken</style> that deals <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(StackDamage) + " per stack)</style> base damage. You can hold up to <style=cIsUtility>" + Count + "</style> <style=cStack>(+" + StackCount + " per stack)</style> <style=cIsDamage>shurikens</style> which all reload over <style=cIsUtility>" + RechargeTime + "</style> seconds.";

        public override void Init()
        {
            Damage = ConfigOption(4f, "Base Damage", "Decimal. Vanilla is 4");
            StackDamage = ConfigOption(1f, "Stack Damage", "Decimal. Per Stack. Vanilla is 1");
            Count = ConfigOption(3, "Base Count", "Vanilla is 3");
            StackCount = ConfigOption(1, "Stack Count", "Per Stack. Vanilla is 1");
            RechargeTime = ConfigOption(10f, "Recharge Time", "Vanilla is 10");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.PrimarySkillShurikenBehavior.Awake += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,

            );
        }
    }
}
*/