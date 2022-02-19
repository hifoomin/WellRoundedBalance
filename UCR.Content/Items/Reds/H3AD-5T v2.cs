using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class Headstompers : ItemBase
    {
        public static float cd;
        public static float minrad;
        public static float maxrad;
        public static float mindmg;
        public static float maxdmg;
        public static float jumpboost;
        public override string Name => ":: Items ::: Reds :: H3AD-5T v2";
        public override string InternalPickupToken => "fallBoots";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Increase <style=cIsUtility>jump height</style>. Creates a <style=cIsDamage>" + minrad + "m-" + maxrad + "m</style> radius <style=cIsDamage>kinetic explosion</style> on hitting the ground, dealing <style=cIsDamage>" + d(mindmg) + "-" + d(maxdmg) + "</style> base damage that scales up with <style=cIsDamage>fall distance</style>. Recharges in <style=cIsDamage>" + cd + "</style> <style=cStack>(-50% per stack)</style> seconds.";
        public override void Init()
        {
            cd = ConfigOption(10f, "Cooldown", "Vanilla is 10");
            jumpboost = ConfigOption(2f, "Jump Height Multiplier", "Vanilla is 2");
            minrad = ConfigOption(5f, "Minimum Range", "Vanilla is 5");
            maxrad = ConfigOption(100f, "Maximum Range", "Vanilla is 100");
            mindmg = ConfigOption(10f, "Minimum Damage", "Vanilla is 10");
            maxdmg = ConfigOption(100f, "Maximum Damage", "Vanilla is 100");
            base.Init();
        }

        public override void Hooks()
        {
            IL.EntityStates.Headstompers.HeadstompersIdle.FixedUpdateAuthority += ChangeJumpHeight;
            Changes();
        }
        public static void Changes()
        {
            On.EntityStates.Headstompers.HeadstompersCooldown.OnEnter += (orig, self) =>
            {
                EntityStates.Headstompers.HeadstompersCooldown.baseDuration = cd;
                orig(self);
            };
            On.EntityStates.Headstompers.HeadstompersFall.OnEnter += (orig, self) =>
            {
                EntityStates.Headstompers.HeadstompersFall.minimumRadius = minrad;
                EntityStates.Headstompers.HeadstompersFall.maximumRadius = maxrad;
                EntityStates.Headstompers.HeadstompersFall.minimumDamageCoefficient = mindmg;
                EntityStates.Headstompers.HeadstompersFall.maximumDamageCoefficient = maxdmg;
                orig(self);
            };
        }
        public static void ChangeJumpHeight(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f)
            );
            c.Next.Operand = jumpboost;
        }
    }
}
