using System;

namespace WellRoundedBalance.Elites {
    public class Voidtouched : EliteBase<Voidtouched> {
        public override string Name => ":: Elites :: Voidtouched";
        
        public static GameObject MortarSmallPrefab;
        public static GameObject MortarDeathPrefab;
        public static GameObject MortarGhost;
        public static GameObject LaserPrefab;

        [ConfigField("Minimum Mortar Count (Skill)", 3)]
        public static int MinMortarCountSkill;
        [ConfigField("Maximum Mortar Count (Skill)", 9)]
        public static int MaxMortarCountSkill;
        [ConfigField("Minimum Mortar Count (Death)", 6)]
        public static int MinMortarCountDeath;
        [ConfigField("Maximum Mortar Count (Death)", 12)]
        public static int MaxMortarCountDeath;

        public override void Init() {
            base.Init();

            LaserPrefab = PrefabAPI.InstantiateClone(new("Voidtouched Beam"), "Voidtouched Laser");

            LaserPrefab.AddComponent<ProjectileController>();
            LaserPrefab.AddComponent<ProjectileDamage>();
            LaserPrefab.AddComponent<VoidtouchedLaserBehaviour>();

            PrefabAPI.RegisterNetworkPrefab(LaserPrefab);

            MortarGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.ClayPotProjectileGhost.Load<GameObject>(), "Voidtouched Mortar Ghost", false);

            Material mat1 = Utils.Paths.Material.matVoidBarnacleBulletOverlay.Load<Material>();
            Material mat2 = Utils.Paths.Material.matVoidBarnacleBullet.Load<Material>();
            Material mat3 = Utils.Paths.Material.matVoidSurvivorCorruptOverlay.Load<Material>();

            ReplaceMaterials(MortarGhost, mat1, mat2, mat3);

            Mesh spiky = Utils.Paths.GameObject.mdlArtifactSpikyBall.Load<GameObject>().GetComponentInChildren<MeshFilter>().mesh;

            MortarGhost.GetComponentInChildren<MeshFilter>().mesh = spiky;

            MortarSmallPrefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.ClayPotProjectile.Load<GameObject>(), "Voidtouched Mortar Small");

            ProjectileSimple simple = MortarSmallPrefab.GetComponent<ProjectileSimple>();
            simple.desiredForwardSpeed = 90f;
            simple.lifetime = 25f;

            MortarSmallPrefab.GetComponent<Rigidbody>().mass = 400f;
            MortarSmallPrefab.GetComponent<SphereCollider>().material = Utils.Paths.PhysicMaterial.physmatVoidSurvivorCrabCannon.Load<PhysicMaterial>();
            MortarSmallPrefab.RemoveComponent<ApplyTorqueOnStart>();

            ProjectileController controller = MortarSmallPrefab.GetComponent<ProjectileController>();
            controller.allowPrediction = false;
            controller.ghostPrefab = MortarGhost;

            ProjectileImpactExplosion impact = MortarSmallPrefab.GetComponent<ProjectileImpactExplosion>();
            impact.blastDamageCoefficient = 1f;
            impact.blastRadius = 1f;
            impact.lifetime = 25f;
            impact.impactEffect = Utils.Paths.GameObject.VoidSurvivorMegaBlasterExplosionCorrupted.Load<GameObject>();

            ProjectileDamage damage = MortarSmallPrefab.GetComponent<ProjectileDamage>();
            damage.damageType = DamageType.Nullify;

            MortarDeathPrefab = PrefabAPI.InstantiateClone(MortarSmallPrefab, "Voidtouched Mortar Death");
            MortarDeathPrefab.transform.localScale *= 2f;

            ProjectileImpactExplosion impactDeath = MortarDeathPrefab.GetComponent<ProjectileImpactExplosion>();
            impactDeath.childrenCount = 1;
            impactDeath.childrenProjectilePrefab = LaserPrefab;
            impactDeath.childrenDamageCoefficient = 1f;
            impactDeath.fireChildren = true;

            ContentAddition.AddProjectile(MortarSmallPrefab);
            ContentAddition.AddProjectile(MortarDeathPrefab);
            ContentAddition.AddProjectile(LaserPrefab);
        }

        public void ReplaceMaterials(GameObject obj, params Material[] mat) {
            foreach (Renderer renderer in obj.GetComponentsInChildren<MeshRenderer>()) {
                renderer.sharedMaterials = mat;
            }
        }

        public override void Hooks() {
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);

            CharacterBody body = damageReport.victimBody;

            if (body && body.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                int count = Mathf.RoundToInt(Util.Remap(body.baseMaxHealth, 1f, 2500f, MinMortarCountDeath, MaxMortarCountDeath));

                Debug.Log("max health: " + body.baseMaxHealth);
                Debug.Log("firing " + count + " mortars");

                for (int i = 0; i < count; i++)
                {
                    FireProjectileInfo info = new();
                    info.owner = body.gameObject;
                    info.projectilePrefab = MortarDeathPrefab;
                    info.position = body.corePosition;
                    info.rotation = Util.QuaternionSafeLookRotation(Util.ApplySpread(Vector3.up, -20f, 20f, 2f, 2f, 0f, 0f));
                    info.damage = body.damage;
                    info.damageColorIndex = DamageColorIndex.Void;

                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }

        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            orig(self);

            CharacterBody body = self.characterBody;

            if (body && body.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                int count = Mathf.RoundToInt(Util.Remap(self.skillDef.baseRechargeInterval, 2f, 14f, MinMortarCountSkill, MaxMortarCountSkill));

                if (self.skillDef.baseRechargeInterval == 0f || self.skillDef.stockToConsume == 0) {
                    count = Util.CheckRoll(35f) ? count : 0;
                }

                for (int i = 0; i < count; i++)
                {
                    FireProjectileInfo info = new();
                    info.owner = body.gameObject;
                    info.projectilePrefab = MortarSmallPrefab;
                    info.position = body.corePosition;
                    info.rotation = Util.QuaternionSafeLookRotation(Util.ApplySpread(body.inputBank.aimDirection, 0f, 3f, 1f, 3f, 0f, 0f));
                    info.damage = body.damage;
                    info.damageColorIndex = DamageColorIndex.Void;

                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }

        public class VoidtouchedLaserBehaviour : MonoBehaviour {
            public ProjectileController controller;
            public ProjectileDamage damage;
            public float stopwatch;
            public float damageStopwatch;
            public GameObject laserInstance;

            public void Start() {
                controller = GetComponent<ProjectileController>();
                damage = GetComponent<ProjectileDamage>();

                laserInstance = GameObject.Instantiate(Utils.Paths.GameObject.VoidRaidCrabSpinBeamVFX.Load<GameObject>(), transform.position - new Vector3(0, 5f, 0), Quaternion.identity);
                laserInstance.transform.forward = Vector3.down;
            }

            public void FixedUpdate() {
                stopwatch += Time.fixedDeltaTime;
                damageStopwatch += Time.fixedDeltaTime;

                if (stopwatch >= 3f) {
                    Destroy(laserInstance);
                    Destroy(gameObject);
                    return;
                }

                if (damageStopwatch >= 0.2f) {
                    damageStopwatch = 0f;

                    BulletAttack bulletAttack = new();
                    bulletAttack.owner = controller.owner;
                    bulletAttack.weapon = gameObject;
                    bulletAttack.origin = transform.position + new Vector3(0, 200f, 0);
                    bulletAttack.aimVector = Vector3.down;
                    bulletAttack.minSpread = 0f;
                    bulletAttack.maxSpread = 0f;
                    bulletAttack.damage = damage.damage * 0.2f;
                    bulletAttack.force = 0f;
                    bulletAttack.hitEffectPrefab = Utils.Paths.GameObject.VoidRaidCrabMultiBeamDotZoneImpact.Load<GameObject>();
                    bulletAttack.isCrit = false;
                    bulletAttack.procChainMask = default;
                    bulletAttack.procCoefficient = 0f;
                    bulletAttack.maxDistance = 200f;
                    bulletAttack.damageType = DamageType.Generic;
                    bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
                    bulletAttack.stopperMask = LayerIndex.world.mask;
                    bulletAttack.radius = 2f;

                    bulletAttack.Fire();
                }
            }
        }
    }
}