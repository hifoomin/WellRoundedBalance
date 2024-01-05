namespace WellRoundedBalance.Enemies.Bosses
{
    internal class XiConstruct : EnemyBase<XiConstruct>
    {
        public override string Name => "::: Bosses :: Xi Construct";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.MajorConstruct.Weapon.TerminateLaser.OnEnter += TerminateLaser_OnEnter;
        }

        private void TerminateLaser_OnEnter(On.EntityStates.MajorConstruct.Weapon.TerminateLaser.orig_OnEnter orig, EntityStates.MajorConstruct.Weapon.TerminateLaser self)
        {
            self.PlayAnimation(self.animationLayerName, self.animationStateName, self.animationPlaybackParameterName, self.duration);
            if (self.muzzleEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(self.muzzleEffectPrefab, self.gameObject, self.muzzleName, false);
            }
            Util.PlaySound(self.enterSoundString, self.gameObject);
        }
    }
}