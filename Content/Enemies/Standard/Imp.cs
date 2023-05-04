using SimpleJSON;
using System;

namespace WellRoundedBalance.Enemies.Standard
{
    internal class Imp : EnemyBase<Imp>
    {
        public override string Name => ":: Enemies :: Imp";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.ImpMonster.DoubleSlash.OnEnter += DoubleSlash_OnEnter;
            Changes();
        }

        private void DoubleSlash_OnEnter(On.EntityStates.ImpMonster.DoubleSlash.orig_OnEnter orig, EntityStates.ImpMonster.DoubleSlash self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.ImpMonster.DoubleSlash.walkSpeedPenaltyCoefficient = 0.66f;
            orig(self);
        }

        private void Changes()
        {
            var imp = Utils.Paths.GameObject.ImpBody10.Load<GameObject>();
            var modelTransform = imp.transform.GetChild(0).GetChild(0);
            var @base = modelTransform.GetChild(5);
            var handL = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "HandL").hitBoxes[0].gameObject;
            var handR = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "HandR").hitBoxes[0].gameObject;
            handL.transform.parent = @base;
            handR.transform.parent = @base;
        }
    }
}