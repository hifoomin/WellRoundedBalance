/*
using RoR2.Navigation;
using System;
using UnityEngine.Events;

namespace WellRoundedBalance.Projectiles
{
    public static class MeteorStorm
    {
        public static GameObject prefab;

        public static void Create()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MeteorStorm.Load<GameObject>(), "Eclipse5MeteorStorm");
        }
    }

    public class Eclipse5MeteorStormController : MonoBehaviour
    {
        private void Start()
        {
            if (NetworkServer.active)
            {
                meteorList = new List<MeteorStormController.Meteor>();
                waveList = new List<MeteorStormController.MeteorWave>();
            }
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            waveTimer -= Time.fixedDeltaTime;
            if (waveTimer <= 0f && wavesPerformed < waveCount)
            {
                wavesPerformed++;
                waveTimer = UnityEngine.Random.Range(waveMinInterval, waveMaxInterval);
                MeteorStormController.MeteorWave meteorWave = new(CharacterBody.readOnlyInstancesList.ToArray(), transform.position);
                waveList.Add(meteorWave);
            }
            for (int i = waveList.Count - 1; i >= 0; i--)
            {
                MeteorStormController.MeteorWave meteorWave2 = waveList[i];
                meteorWave2.timer -= Time.fixedDeltaTime;
                if (meteorWave2.timer <= 0f)
                {
                    meteorWave2.timer = UnityEngine.Random.Range(0.05f, 1f);
                    MeteorStormController.Meteor nextMeteor = meteorWave2.GetNextMeteor();
                    if (nextMeteor == null)
                    {
                        waveList.RemoveAt(i);
                    }
                    else if (nextMeteor.valid)
                    {
                        meteorList.Add(nextMeteor);
                        EffectManager.SpawnEffect(warningEffectPrefab, new EffectData
                        {
                            origin = nextMeteor.impactPosition,
                            scale = blastRadius
                        }, true);
                    }
                }
            }
            float num = Run.instance.time - impactDelay;
            float num2 = num - travelEffectDuration;
            for (int j = meteorList.Count - 1; j >= 0; j--)
            {
                MeteorStormController.Meteor meteor = meteorList[j];
                if (meteor.startTime < num2 && !meteor.didTravelEffect)
                {
                    DoMeteorEffect(meteor);
                }
                if (meteor.startTime < num)
                {
                    meteorList.RemoveAt(j);
                    DetonateMeteor(meteor);
                }
            }
            if (wavesPerformed == waveCount && meteorList.Count == 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            onDestroyEvents.Invoke();
        }

        private void DoMeteorEffect(MeteorStormController.Meteor meteor)
        {
            meteor.didTravelEffect = true;
            if (travelEffectPrefab)
            {
                EffectManager.SpawnEffect(travelEffectPrefab, new EffectData
                {
                    origin = meteor.impactPosition
                }, true);
            }
        }

        private void DetonateMeteor(MeteorStormController.Meteor meteor)
        {
            EffectData effectData = new()
            {
                origin = meteor.impactPosition
            };
            EffectManager.SpawnEffect(impactEffectPrefab, effectData, true);
            new BlastAttack
            {
                inflictor = gameObject,
                baseDamage = blastDamageCoefficient * ownerDamage,
                baseForce = blastForce,
                attackerFiltering = AttackerFiltering.AlwaysHit,
                crit = isCrit,
                falloffModel = BlastAttack.FalloffModel.None,
                attacker = owner,
                bonusForce = Vector3.zero,
                damageColorIndex = DamageColorIndex.Item,
                position = meteor.impactPosition,
                procChainMask = default(ProcChainMask),
                procCoefficient = 1f,
                teamIndex = TeamIndex.Monster,
                radius = blastRadius
            }.Fire();
        }

        public int waveCount = 40;

        public float waveMinInterval = 0.5f;

        public float waveMaxInterval = 1f;

        public GameObject warningEffectPrefab = Utils.Paths.GameObject.MeteorStrikePredictionEffect.Load<GameObject>();

        public GameObject travelEffectPrefab = null;

        public float travelEffectDuration = 0;

        public GameObject impactEffectPrefab = Utils.Paths.GameObject.MeteorStrikeImpact.Load<GameObject>();

        public float impactDelay = 2f;

        public float blastDamageCoefficient = 3f;

        public float blastRadius = 8f;

        public float blastForce = 4000f;

        [NonSerialized]
        public GameObject owner = null;

        [NonSerialized]
        public float ownerDamage = Run.instance ? 1.5f + Mathf.Sqrt(Run.instance.ambientLevel * 50f) : 0f;

        [NonSerialized]
        public bool isCrit = false;

        public UnityEvent onDestroyEvents;

        private List<MeteorStormController.Meteor> meteorList;

        private List<MeteorStormController.MeteorWave> waveList;

        private int wavesPerformed;

        private float waveTimer;
    }

    public class Eclipse5Meteor
    {
        public Vector3 impactPosition;

        public float startTime;

        public bool didTravelEffect;

        public bool valid = true;
    }

    public class Eclipse5MeteorWave
    {
        public Eclipse5MeteorWave(CharacterBody[] targets, Vector3 center)
        {
            targets = new CharacterBody[targets.Length];
            targets.CopyTo(targets, 0);
            Util.ShuffleArray<CharacterBody>(targets);
            center = center;
            nodeGraphSpider = new NodeGraphSpider(SceneInfo.instance.groundNodes, HullMask.Human);
            nodeGraphSpider.AddNodeForNextStep(SceneInfo.instance.groundNodes.FindClosestNode(center, HullClassification.Human, float.PositiveInfinity));
            int num = 0;
            int num2 = 20;
            while (num < num2 && nodeGraphSpider.PerformStep())
            {
                num++;
            }
        }

        public MeteorStormController.Meteor GetNextMeteor()
        {
            if (currentStep >= targets.Length)
            {
                return null;
            }
            CharacterBody characterBody = targets[currentStep];
            MeteorStormController.Meteor meteor = new MeteorStormController.Meteor();
            if (characterBody && UnityEngine.Random.value < hitChance)
            {
                meteor.impactPosition = characterBody.corePosition;
                Vector3 vector = meteor.impactPosition + Vector3.up * 6f;
                Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
                onUnitSphere.y = -1f;
                RaycastHit raycastHit;
                if (Physics.Raycast(vector, onUnitSphere, out raycastHit, 12f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    meteor.impactPosition = raycastHit.point;
                }
                else if (Physics.Raycast(meteor.impactPosition, Vector3.down, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    meteor.impactPosition = raycastHit.point;
                }
            }
            else if (nodeGraphSpider.collectedSteps.Count != 0)
            {
                int num = UnityEngine.Random.Range(0, nodeGraphSpider.collectedSteps.Count);
                SceneInfo.instance.groundNodes.GetNodePosition(nodeGraphSpider.collectedSteps[num].node, out meteor.impactPosition);
            }
            else
            {
                meteor.valid = false;
            }
            meteor.startTime = Run.instance.time;
            currentStep++;
            return meteor;
        }

        private readonly CharacterBody[] targets;

        private int currentStep;

        private readonly float hitChance = 0.5f;

        private readonly Vector3 center;

        public float timer;

        private readonly NodeGraphSpider nodeGraphSpider;
    }
}
*/