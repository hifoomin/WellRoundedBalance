using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class LittleDisciple : ItemBase
    {
        public static float Damage;
        public static float ProcCoefficient;
        public static float Range;
        public static float FireRate;

        public override string Name => ":: Items :::: Yellows :: Little Disciple";
        public override string InternalPickupToken => "sprintWisp";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Fire a <style=cIsDamage>tracking wisp</style> for <style=cIsDamage>" + d(Damage) + " <style=cStack>(+" + d(Damage) + " per stack)</style> damage</style>. Fires every <style=cIsUtility>1.6</style> seconds while sprinting. Fire rate increases with <style=cIsUtility>movement speed</style>.";

        public override void Init()
        {
            Damage = ConfigOption(3f, "Damage", "Decimal. Per Stack. Vanilla is 3");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            Range = ConfigOption(40f, "Range", "Vanilla is 40");
            FireRate = ConfigOption(0.08571429f, "Fire Rate", "Vanilla is 0.08571429");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Items.SprintWispBodyBehavior.Fire += Changes;
            IL.RoR2.Items.SprintWispBodyBehavior.Fire += ChangeProcCoefficient;
        }

        private void ChangeProcCoefficient(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchStfld("RoR2.Orbs.DevilOrb", "procCoefficient")))
            {
                c.Next.Operand = ProcCoefficient;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Little Disciple Proc Coefficient hook");
            }
        }

        private void Changes(On.RoR2.Items.SprintWispBodyBehavior.orig_Fire orig, RoR2.Items.SprintWispBodyBehavior self)
        {
            RoR2.Items.SprintWispBodyBehavior.damageCoefficient = Damage;
            RoR2.Items.SprintWispBodyBehavior.searchRadius = Range;
            RoR2.Items.SprintWispBodyBehavior.fireRate = FireRate;
            orig(self);
        }
    }
}