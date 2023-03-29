using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class StridesOfHeresy : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Strides of Heresy";
        public override ItemDef InternalPickup => RoR2Content.Items.LunarUtilityReplacement;

        public override string PickupText => "Replace your Utility Skill with 'Shadowfade'.";
        public override string DescText => "<style=cIsUtility>Replace your Utility Skill</style> with <style=cIsUtility>Shadowfade</style>. \n\nFade away, becoming <style=cIsUtility>intangible</style> and gaining <style=cIsUtility>+30% movement speed</style>. <style=cIsHealing>Heal</style> for <style=cIsHealing>25% <style=cStack>(+25% per stack)</style> of your maximum health</style>. Lasts 3 <style=cStack>(+3 per stack)</style> seconds.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.GhostUtilitySkillState.FixedUpdate += GhostUtilitySkillState_FixedUpdate;
        }

        private void GhostUtilitySkillState_FixedUpdate(On.EntityStates.GhostUtilitySkillState.orig_FixedUpdate orig, EntityStates.GhostUtilitySkillState self)
        {
            var body = self.characterBody;
            if (body)
            {
                var motor = body.characterMotor;
                if (motor && body.GetNotMoving())
                {
                    motor.Motor.ForceUnground();
                }
            }
            orig(self);
        }
    }
}