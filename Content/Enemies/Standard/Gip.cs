using RoR2.Skills;
using System;

namespace WellRoundedBalance.Enemies.Standard
{
    internal class Gip : EnemyBase<Gip>
    {
        public override string Name => ":: Enemies :: Gip";

        [ConfigField("Base Max Health", "Disabled if playing Inferno.", 125f)]
        public static float baseMaxHealth;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            Changes();
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (Main.IsInfernoDef())
            {
                return;
            }
            switch (body.name)
            {
                case "GipBody(Clone)":
                    body.baseMoveSpeed = 29f;
                    break;
            }
        }

        public void Changes()
        {
            var gip = Utils.Paths.GameObject.GipBody23.Load<GameObject>();

            var gipBody = gip.GetComponent<CharacterBody>();
            gipBody.baseMaxHealth = baseMaxHealth;
            gipBody.levelMaxHealth = baseMaxHealth * 0.3f;

            var modelTransform = gip.transform.GetChild(0).GetChild(0);
            var spikes = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Spikes");
            var mainHitbox = spikes.hitBoxes[0].gameObject;
            mainHitbox.transform.localScale = new Vector3(6.5f, 6.5f, 3.5f);

            /*
            var hitboxGroup = modelTransform.GetComponent<HitBoxGroup>();
            hitboxGroup.hitBoxes[0] = spikes.hitBoxes[1];
            var newHitbox = hitboxGroup.hitBoxes[0];
            newHitbox.transform.localScale = new Vector3(2.75f, 2.75f, 2.25f);

            var contactDamage = gip.AddComponent<ContactDamage>();
            contactDamage.pushForcePerSecond = 500f;
            contactDamage.damagePerSecondCoefficient = 0.7f;
            contactDamage.hitBoxGroup = hitboxGroup;
            */

            var sd = Utils.Paths.SkillDef.GupSpikes.Load<SkillDef>();
            sd.baseRechargeInterval = 1.5f;
        }
    }
}