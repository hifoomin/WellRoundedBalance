using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Enemies.Standard
{
    internal class MiniMushrum : EnemyBase<MiniMushrum>
    {
        public override string Name => ":: Enemies :: Mini Mushrum";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.MiniMushroom.Plant.OnEnter += Plant_OnEnter;
            On.EntityStates.MiniMushroom.SporeGrenade.OnEnter += SporeGrenade_OnEnter;
            Changes();
        }

        private void SporeGrenade_OnEnter(On.EntityStates.MiniMushroom.SporeGrenade.orig_OnEnter orig, EntityStates.MiniMushroom.SporeGrenade self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.MiniMushroom.SporeGrenade.baseDuration = 2f;
            orig(self);
        }

        private void Plant_OnEnter(On.EntityStates.MiniMushroom.Plant.orig_OnEnter orig, EntityStates.MiniMushroom.Plant self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.MiniMushroom.Plant.baseMaxDuration = 5f;
                EntityStates.MiniMushroom.Plant.baseMinDuration = 2f;
                EntityStates.MiniMushroom.Plant.mushroomRadius = 13f;
                EntityStates.MiniMushroom.Plant.healFraction = 0.06f;
            }

            orig(self);
        }

        private void Changes()
        {
            var spore = Utils.Paths.GameObject.SporeGrenadeProjectileDotZone.Load<GameObject>();
            spore.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);

            var projectileDotZone = spore.GetComponent<ProjectileDotZone>();
            projectileDotZone.damageCoefficient = 0.45f;
            projectileDotZone.lifetime = 6f;
        }
    }
}