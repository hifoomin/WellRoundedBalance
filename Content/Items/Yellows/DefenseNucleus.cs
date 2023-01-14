using WellRoundedBalance.Projectiles;

namespace WellRoundedBalance.Items.Yellows
{
    internal class DefenseNucleus : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Defense Nucleus";

        public override string InternalPickupToken => "minorConstructOnKill";

        public override string PickupText => "Unleash a devastating laser upon killing an elite.";

        public override string DescText => "Killing an elite monster summons a devastating laser for <style=cIsUtility>5 seconds</style> that deals <style=cIsDamage>300%</style> <style=cStack>(+100% per stack)</style> damage per second.";

        public override void Hooks()
        {
            GlobalEventManager.onCharacterDeathGlobal += DefenseNucleusBehavior.GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                var stack = body.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill);
                body.AddItemBehavior<DefenseNucleusBehavior>(stack);
            }
        }

        private int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.MinorConstructOnKill)
            {
                return 0;
            }
            else
            {
                return orig(self, slot);
            }
        }
    }

    public class DefenseNucleusBehavior : CharacterBody.ItemBehavior
    {
        // FIX THIS AAAAAAAAAA FUCK I HATE THIS SHIT
        // WHY IS SPAWN EFFECT SO GARBAGE

        public float currentDuration = 0f;
        public GameObject tracerPrefab = DucleusLaser.prefab;
        public GameObject hitEffectPrefab = Utils.Paths.GameObject.Hitspark1.Load<GameObject>();
        public float interval = 0.2f;
        public GameObject laser;
        public ChildLocator childLocator;

        private void Start()
        {
            if (body.modelLocator.modelTransform.GetComponent<ChildLocator>())
            {
                childLocator = body.modelLocator.modelTransform.GetComponent<ChildLocator>();
            }
        }

        private void FixedUpdate()
        {
            if (currentDuration >= interval)
            {
                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = body.inputBank.GetAimRay().origin,
                    aimVector = body.inputBank.GetAimRay().direction,
                    bulletCount = 1,
                    damage = body.damage * 0.6f + 0.2f * (stack - 1),
                    maxDistance = 10000,
                    falloffModel = BulletAttack.FalloffModel.None,
                    force = 150,
                    isCrit = body.RollCrit(),
                    damageType = DamageType.Generic,
                    radius = 6f,
                    procCoefficient = 0.6f,
                    damageColorIndex = DamageColorIndex.Default,
                    tracerEffectPrefab = null,
                    hitEffectPrefab = hitEffectPrefab
                }.Fire();
            }
            if (childLocator)
            {
                var laser = childLocator.FindChild(69);
                if (laser != null)
                {
                    laser.transform.rotation = body.transform.rotation;
                    laser.transform.position = body.corePosition;
                }
            }
            currentDuration -= Time.fixedDeltaTime;
            currentDuration = Mathf.Max(0f, currentDuration);
        }

        private void SummonLaserVFX()
        {
            if (childLocator && childLocator.FindChild(69) == null)
            {
                EffectManager.SpawnEffect(tracerPrefab, new EffectData { rotation = body.transform.rotation, origin = body.corePosition, modelChildIndex = 69 }, true);
            }
        }

        public static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (damageReport.attackerBody)
            {
                if (damageReport.victimIsElite)
                {
                    var ducleus = damageReport.attackerBody.GetComponent<DefenseNucleusBehavior>();
                    if (ducleus != null)
                    {
                        ducleus.currentDuration += 5f;
                        ducleus.SummonLaserVFX();
                    }
                }
            }
        }
    }
}