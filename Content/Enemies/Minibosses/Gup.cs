using RoR2.Skills;
using System;
using UnityEngine;

namespace WellRoundedBalance.Enemies.Minibosses
{
    internal class Gup : EnemyBase<Gup>
    {
        public override string Name => ":: Minibosses :: Gup";

        [ConfigField("Base Max Health", "Disabled if playing Inferno.", 500f)]
        public static float baseMaxHealth;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.EntityStates.Gup.GupSpikesState.OnEnter += GupSpikesState_OnEnter;
            On.EntityStates.Gup.GupSpikesState.FixedUpdate += GupSpikesState_FixedUpdate;
            Changes();
        }

        private void GupSpikesState_FixedUpdate(On.EntityStates.Gup.GupSpikesState.orig_FixedUpdate orig, EntityStates.Gup.GupSpikesState self)
        {
            orig(self);
            GupSpikesController controller = null;
            if (controller == null) controller = self.GetComponent<GupSpikesController>();
            if (!Main.IsInfernoDef() && !controller.hasFired)
            {
                int spikeCount = self.outer.gameObject.name switch
                {
                    "GupBody(Clone)" => 12,
                    "GeepBody(Clone)" => 8,
                    "GipBody(Clone)" => 5,
                    _ => 0
                };
                if (self.isAuthority && self.animator?.GetFloat(self.initialHitboxActiveParameter) > 0.5f)
                {
                    var slices = 360f / spikeCount;
                    var projectedNormal = Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, Vector3.up).normalized;
                    var corePosition = self.characterBody.corePosition;
                    for (int i = 0; i < spikeCount; i++)
                    {
                        var vector = Quaternion.AngleAxis(slices * i, Vector3.up) * projectedNormal;
                        ProjectileManager.instance.FireProjectile(Projectiles.GupSpike.prefab, corePosition, Util.QuaternionSafeLookRotation(vector), self.gameObject, self.characterBody.damage * 2f, -2000f, Util.CheckRoll(self.characterBody.crit, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                    }
                    controller.hasFired = true;
                }
            }
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (Main.IsInfernoDef())
            {
                return;
            }
            switch (body.name)
            {
                case "GupBody(Clone)":
                    body.baseMoveSpeed = 19f;
                    if (body.GetComponent<GupSpikesController>() == null)
                    {
                        body.gameObject.AddComponent<GupSpikesController>();
                    }
                    break;
            }
        }

        private void GupSpikesState_OnEnter(On.EntityStates.Gup.GupSpikesState.orig_OnEnter orig, EntityStates.Gup.GupSpikesState self)
        {
            self.pushAwayForce = 3500f;
            self.damageCoefficient = 3.5f;
            var controller = self.GetComponent<GupSpikesController>();
            if (controller)
            {
                controller.hasFired = false;
            }

            orig(self);
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster master)
        {
            if (Main.IsInfernoDef())
            {
                return;
            }
            switch (master.name)
            {
                case "GupMaster(Clone)":
                    AISkillDriver spike = (from x in master.GetComponents<AISkillDriver>()
                                           where x.customName == "Spike"
                                           select x).First();
                    spike.maxDistance = 45f;
                    break;
            }
        }

        private void Changes()
        {
            var gup = Utils.Paths.GameObject.GupBody12.Load<GameObject>();

            var gupBody = gup.GetComponent<CharacterBody>();
            gupBody.baseMaxHealth = baseMaxHealth;
            gupBody.levelMaxHealth = baseMaxHealth * 0.3f;
            gupBody.baseDamage = 10f;
            gupBody.levelDamage = 2f;

            var modelTransform = gup.transform.GetChild(0).GetChild(0);
            var spikes = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Spikes");
            var mainHitbox = spikes.hitBoxes[0].gameObject;
            mainHitbox.transform.localScale = new Vector3(4f, 4f, 1.7f);
            /*
            var hitboxGroup = modelTransform.GetComponent<HitBoxGroup>();
            hitboxGroup.hitBoxes[0] = spikes.hitBoxes[1];
            Array.Resize(ref hitboxGroup.hitBoxes, 1);
            var newHitbox = hitboxGroup.hitBoxes[0];
            newHitbox.transform.localScale = new Vector3(1.8f, 1.8f, 1.5f);

            var contactDamage = gup.AddComponent<ContactDamage>();
            contactDamage.pushForcePerSecond = 500f;
            contactDamage.damagePerSecondCoefficient = 0.7f;
            contactDamage.hitBoxGroup = hitboxGroup;
            */

            var sd = Utils.Paths.SkillDef.GupSpikes.Load<SkillDef>();
            sd.baseRechargeInterval = 1.5f;
        }
    }

    public class GupSpikesController : MonoBehaviour
    {
        public bool hasFired;
    }
}