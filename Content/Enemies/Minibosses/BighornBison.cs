using System;

namespace WellRoundedBalance.Enemies.Minibosses
{
    internal class BighornBison : EnemyBase<BighornBison>
    {
        public override string Name => ":: Minibosses :: Bighorn Bison";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            if (!Main.IsInfernoDef())
            {
                On.EntityStates.Bison.Headbutt.OnEnter += Headbutt_OnEnter;
                On.EntityStates.Bison.Charge.OnEnter += Charge_OnEnter;
                On.EntityStates.Bison.PrepCharge.OnEnter += PrepCharge_OnEnter;
            }

            Changes();
        }

        private void PrepCharge_OnEnter(On.EntityStates.Bison.PrepCharge.orig_OnEnter orig, EntityStates.Bison.PrepCharge self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.Bison.PrepCharge.basePrepDuration = 1.25f;
            orig(self);
        }

        private void Charge_OnEnter(On.EntityStates.Bison.Charge.orig_OnEnter orig, EntityStates.Bison.Charge self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.Bison.Charge.impactDamageCoefficient = 3f;
                EntityStates.Bison.Charge.damageCoefficient = 3.5f;
                EntityStates.Bison.Charge.turnSpeed = 600f;
                EntityStates.Bison.Charge.selfStunDuration = 1f;
                EntityStates.Bison.Charge.chargeMovementSpeedCoefficient = 8.5f;
            }

            orig(self);
        }

        private void Headbutt_OnEnter(On.EntityStates.Bison.Headbutt.orig_OnEnter orig, EntityStates.Bison.Headbutt self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.Bison.Headbutt.baseHeadbuttDuration = 1.85f;
                EntityStates.Bison.Headbutt.damageCoefficient = 4f;
            }
            orig(self);
        }

        private void Changes()
        {
            var bison = Utils.Paths.GameObject.BisonBody20.Load<GameObject>();
            var body = bison.GetComponent<CharacterBody>();
            body.baseDamage = 11f;
            body.levelDamage = 2.2f;
            var modelTransform = bison.transform.GetChild(0).GetChild(0);
            var @base = modelTransform.GetChild(0).GetChild(0).GetChild(1);
            var headbuttHitbox = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Headbutt").hitBoxes[0].gameObject;
            headbuttHitbox.transform.localPosition = new Vector3(0f, 0f, -0.8f);
            headbuttHitbox.transform.localScale = new Vector3(3f, 4.2f, 2.5f);
            headbuttHitbox.transform.parent = @base;

            var chargeHitbox = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Charge").hitBoxes[0].gameObject;
            chargeHitbox.transform.localPosition = new Vector3(0f, 1.41f, 1.2f);
            chargeHitbox.transform.localScale = new Vector3(2.6f, 2.5f, 3.7f);
        }
    }
}