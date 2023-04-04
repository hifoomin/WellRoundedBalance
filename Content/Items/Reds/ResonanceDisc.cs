namespace WellRoundedBalance.Items.Reds
{
    public class ResonanceDisc : ItemBase<ResonanceDisc>
    {
        public override string Name => ":: Items ::: Reds :: Resonance Disc";
        public override ItemDef InternalPickup => RoR2Content.Items.LaserTurbine;

        public override string PickupText => "Obtain a Resonance Disc charged by killing enemies. Fires automatically when fully charged.";

        public override string DescText => "Killing <style=cIsDamage>4</style> enemies in <style=cIsUtility>7 seconds</style> charges the Resonance Disc. The disc launches itself toward a target for <style=cIsDamage>300%</style> base damage <style=cStack>(+300% per stack)</style>, piercing all enemies it doesn't kill, and then explodes for <style=cIsDamage>1000%</style> base damage <style=cStack>(+1000% per stack)</style>. Returns to the user, striking all enemies along the way for <style=cIsDamage>300%</style> base damage <style=cStack>(+300% per stack)</style>.";

        [ConfigField("Proc Chance", 0f)]
        public static float procChance;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.LaserTurbine.FireMainBeamState.OnEnter += Changes;
        }

        public static void Changes(On.EntityStates.LaserTurbine.FireMainBeamState.orig_OnEnter orig, EntityStates.LaserTurbine.FireMainBeamState self)
        {
            EntityStates.LaserTurbine.FireMainBeamState.mainBeamProcCoefficient = procChance * globalProc;
            orig(self);
        }
    }
}