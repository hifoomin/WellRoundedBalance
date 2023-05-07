using System;

namespace WellRoundedBalance.Enemies.Standard
{
    internal class Geep : EnemyBase<Geep>
    {
        public override string Name => ":: Enemies :: Geep";

        [ConfigField("Base Max Health", "Disabled if playing Inferno.", 250f)]
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
                case "GeepBody(Clone)":
                    body.baseMoveSpeed = 24f;
                    break;
            }
        }

        private void Changes()
        {
            var geep = Utils.Paths.GameObject.GeepBody12.Load<GameObject>();

            var geepBody = geep.GetComponent<CharacterBody>();
            geepBody.baseMaxHealth = baseMaxHealth;
            geepBody.levelMaxHealth = baseMaxHealth * 0.3f;

            var modelTransform = geep.transform.GetChild(0).GetChild(0);
            var spikes = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Spikes");
            var mainHitbox = spikes.hitBoxes[0].gameObject;
            mainHitbox.transform.localScale = new Vector3(6f, 6f, 2.5f);

            var hitboxGroup = modelTransform.GetComponent<HitBoxGroup>();
            spikes.hitBoxes[1] = hitboxGroup.hitBoxes[0];
            spikes.hitBoxes[2] = hitboxGroup.hitBoxes[1];
            Array.Resize(ref spikes.hitBoxes, 1);

            var contactDamage = geep.AddComponent<ContactDamage>();
            contactDamage.pushForcePerSecond = 500f;
            contactDamage.damagePerSecondCoefficient = 0.5f;
            contactDamage.hitBoxGroup = hitboxGroup;
        }
    }
}