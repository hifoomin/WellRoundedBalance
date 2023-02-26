namespace WellRoundedBalance.Items.Whites
{
    public class StickyBomb : ItemBase
    {
        public override string Name => ":: Items : Whites :: Sticky Bomb";
        public override string InternalPickupToken => "stickyBomb";

        public override string PickupText => "Chance on hit to attach a bomb to enemies.";

        public override string DescText => "<style=cIsDamage>5%</style> <style=cStack>(+5% per stack)</style> chance on hit to attach a <style=cIsDamage>bomb</style> to an enemy, detonating for <style=cIsDamage>180%</style> TOTAL damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var StickyBombImpact = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/stickybomb").GetComponent<ProjectileImpactExplosion>();
            StickyBombImpact.lifetime = 1f;
            StickyBombImpact.falloffModel = BlastAttack.FalloffModel.None;
        }
    }
}