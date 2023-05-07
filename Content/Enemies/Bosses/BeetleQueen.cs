using RoR2.Skills;
using UnityEngine.XR;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class BeetleQueen : EnemyBase<BeetleQueen>
    {
        public static GameObject buffWard;
        public static BuffDef inspire;
        public override string Name => "::: Bosses :: Beetle Queen";

        public override void Init()
        {
            base.Init();

            inspire = ScriptableObject.CreateInstance<BuffDef>();
            inspire.isCooldown = false;
            inspire.canStack = false;
            inspire.isHidden = false;
            inspire.buffColor = new Color32(214, 201, 58, 255);
            inspire.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texBuffInspire.png");
            inspire.name = "Beetle Queen Inspire";

            ContentAddition.AddBuffDef(inspire);

            buffWard = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.WarbannerWard.Load<GameObject>(), "BeetleQueenInspire");
            var mdl = buffWard.transform.GetChild(1);
            mdl.gameObject.SetActive(false);

            var inspireMat = Object.Instantiate(Utils.Paths.Material.matWarbannerSphereIndicator2.Load<Material>());
            inspireMat.SetTexture("_RemapTex", Utils.Paths.Texture2D.texRampThermite2.Load<Texture2D>());

            var meshRenderer = buffWard.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
            meshRenderer.material = inspireMat;

            var ward = buffWard.GetComponent<BuffWard>();
            ward.radius = 24f;
            ward.interval = 4f;
            ward.buffDuration = 4f;
            ward.expires = true;
            ward.expireDuration = 16f;
            ward.radiusCoefficientCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 2f));
            ward.buffDef = inspire;

            PrefabAPI.RegisterNetworkPrefab(buffWard);
        }

        public override void Hooks()
        {
            On.EntityStates.BeetleQueenMonster.SummonEggs.OnEnter += SummonEggs_OnEnter;
            On.EntityStates.BeetleQueenMonster.FireSpit.OnEnter += FireSpit_OnEnter;
            On.EntityStates.BeetleQueenMonster.SpawnWards.OnEnter += SpawnWards_OnEnter;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.BuffWard.BuffTeam += BuffWard_BuffTeam;
            Changes();
        }

        private void BuffWard_BuffTeam(On.RoR2.BuffWard.orig_BuffTeam orig, BuffWard self, IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
        {
            if (self.buffDef && NetworkServer.active && self.buffDef == inspire)
            {
                foreach (TeamComponent teamComponent in recipients)
                {
                    var distance = teamComponent.transform.position - currentPosition;
                    if (distance.sqrMagnitude <= radiusSqr)
                    {
                        var characterBody = teamComponent.GetComponent<CharacterBody>();
                        if (characterBody && (!self.requireGrounded || !characterBody.characterMotor || characterBody.characterMotor.isGrounded))
                        {
                            characterBody.AddTimedBuff(self.buffDef, self.buffDuration, 2147483647);
                        }
                    }
                }
            }

            orig(self, recipients, radiusSqr, currentPosition);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                args.cooldownMultAdd -= sender.GetBuffCount(inspire) * 0.03f;
                args.baseAttackSpeedAdd += sender.GetBuffCount(inspire) * 0.03f;
                args.moveSpeedMultAdd += sender.GetBuffCount(inspire) * 0.03f;
            }
        }

        private void SpawnWards_OnEnter(On.EntityStates.BeetleQueenMonster.SpawnWards.orig_OnEnter orig, EntityStates.BeetleQueenMonster.SpawnWards self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.SpawnWards.baseDuration = 3f;
                EntityStates.BeetleQueenMonster.SpawnWards.orbTravelSpeed = 20f;
                if (self.gameObject.GetComponent<Inspire>() == null)
                    self.gameObject.AddComponent<Inspire>();
            }

            orig(self);
        }

        private void FireSpit_OnEnter(On.EntityStates.BeetleQueenMonster.FireSpit.orig_OnEnter orig, EntityStates.BeetleQueenMonster.FireSpit self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.FireSpit.damageCoefficient = 0.4f;
                EntityStates.BeetleQueenMonster.FireSpit.force = 1200f;
                EntityStates.BeetleQueenMonster.FireSpit.yawSpread = 20f;
                EntityStates.BeetleQueenMonster.FireSpit.minSpread = 15f;
                EntityStates.BeetleQueenMonster.FireSpit.maxSpread = 30f;
                EntityStates.BeetleQueenMonster.FireSpit.projectileHSpeed = 40f;
                EntityStates.BeetleQueenMonster.FireSpit.projectileCount = 9;
            }
            orig(self);
        }

        private void SummonEggs_OnEnter(On.EntityStates.BeetleQueenMonster.SummonEggs.orig_OnEnter orig, EntityStates.BeetleQueenMonster.SummonEggs self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.SummonEggs.summonInterval = 2f;
                EntityStates.BeetleQueenMonster.SummonEggs.randomRadius = 13f;
                EntityStates.BeetleQueenMonster.SummonEggs.baseDuration = 3f;
            }

            orig(self);
        }

        private void Changes()
        {
            var summonBeetleGuards = Utils.Paths.SkillDef.BeetleQueen2BodySummonEggs.Load<SkillDef>();
            summonBeetleGuards.baseRechargeInterval = 60f;

            var spitProjectile = Utils.Paths.GameObject.BeetleQueenSpit.Load<GameObject>();
            var projectileImpactExplosion = spitProjectile.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;

            var spitDoT = Utils.Paths.GameObject.BeetleQueenAcid.Load<GameObject>();
            var projectileDotZone = spitDoT.GetComponent<ProjectileDotZone>();
            projectileDotZone.lifetime = 9f;
            projectileDotZone.damageCoefficient = 3f;
            spitDoT.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);

            var hitBox = spitDoT.transform.GetChild(0).GetChild(2);
            hitBox.localPosition = new Vector3(0f, 0f, -0.5f);
            hitBox.localScale = new Vector3(4f, 1.5f, 4f);

            var beetleWard = Utils.Paths.GameObject.BeetleWard.Load<GameObject>();
            var buffWard = beetleWard.GetComponent<BuffWard>();
            buffWard.radius = 7.5f;
            buffWard.interval = 0.5f;
            buffWard.buffDuration = 3f;
            buffWard.expireDuration = 10f;

            var egg = Utils.Paths.SkillDef.BeetleQueen2BodySpawnWards.Load<SkillDef>();
            egg.baseRechargeInterval = 12f;
        }
    }

    public class Inspire : MonoBehaviour
    {
        public float timer;
        public float delay = 4f;
        public float cooldown = 0f;
        public float maxDuration = 20f;
        public GameObject ward;
        public GameObject wardInstance;

        public void Start()
        {
            ward = BeetleQueen.buffWard;
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            delay -= Time.fixedDeltaTime;
            cooldown -= Time.fixedDeltaTime;
            if (delay <= 0 && cooldown <= 0)
            {
                wardInstance = Instantiate(ward, gameObject.transform.position, Quaternion.identity);
                wardInstance.GetComponent<TeamFilter>().teamIndex = gameObject.GetComponent<TeamComponent>().teamIndex;
                NetworkServer.Spawn(wardInstance);
                cooldown = 50f;
            }
            if (timer >= maxDuration)
            {
                NetworkServer.Destroy(wardInstance);
            }
        }
    }
}