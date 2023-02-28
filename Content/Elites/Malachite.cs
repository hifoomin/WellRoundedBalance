using WellRoundedBalance.Eclipse;

namespace WellRoundedBalance.Elites
{
    public class Malachite : EliteBase<Malachite>
    {
        public override string Name => "Elites :::: Malachite";

        [ConfigField("Turret Count", "", 2)]
        public static int TurretCount;

        [ConfigField("Turret Count Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 3)]
        public static int turretCountE3;

        [ConfigField("Safe Zone Radius", "", 50f)]
        public static float SafeZoneRadius;

        [ConfigField("Healing Multiplier", "", 0.5f)]
        public static float HealingMultiplier;

        internal static GameObject MalachiteTurret;
        internal static GameObject MalachiteDebuffZone;

        public override void Hooks()
        {
            On.RoR2.CharacterBody.UpdateAffixPoison += (orig, self, delta) =>
            {
                if (self.HasBuff(RoR2Content.Buffs.AffixPoison) && !self.GetComponent<TurretSpawner>())
                {
                    self.gameObject.AddComponent<TurretSpawner>();
                }

                if (!self.HasBuff(RoR2Content.Buffs.AffixPoison) && self.GetComponent<TurretSpawner>())
                {
                    self.gameObject.RemoveComponent<TurretSpawner>();
                }
            };

            MalachiteTurret = PrefabAPI.InstantiateClone(new("e"), "MalachiteTurret", false);
            MalachiteTurret.AddComponent<NetworkIdentity>();
            MalachiteTurret.AddComponent<TurretController>();
            Rigidbody rb = MalachiteTurret.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.mass = 300;
            GameObject turretMdl = GameObject.Instantiate(Utils.Paths.GameObject.mdlUrchinTurret.Load<GameObject>());
            turretMdl.transform.SetParent(MalachiteTurret.transform);
            turretMdl.transform.localScale *= 0.2f;
            turretMdl.transform.rotation = Quaternion.Euler(-90, 0, 0);
            SkinnedMeshRenderer srenderer = turretMdl.GetComponentInChildren<SkinnedMeshRenderer>();
            MeshRenderer mrenderer = turretMdl.GetComponentInChildren<MeshRenderer>();
            if (srenderer)
            {
                srenderer.material = Utils.Paths.Material.matEliteUrchinCrown.Load<Material>();
            }
            if (mrenderer)
            {
                mrenderer.material = Utils.Paths.Material.matEliteUrchinCrown.Load<Material>();
            }
            PrefabAPI.RegisterNetworkPrefab(MalachiteTurret);

            MalachiteDebuffZone = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.RailgunnerMineAltDetonated.Load<GameObject>(), "AntihealZone");
            Transform areaIndicator = MalachiteDebuffZone.transform.Find("AreaIndicator");
            Transform softGlow = areaIndicator.Find("SoftGlow");
            Transform sphere = areaIndicator.Find("Sphere");
            Transform light = areaIndicator.Find("Point Light");
            Transform core = areaIndicator.Find("Core");

            softGlow.gameObject.SetActive(false);
            light.gameObject.SetActive(false);
            core.gameObject.SetActive(false);

            MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
            Material[] mats = renderer.sharedMaterials;
            mats[0] = Utils.Paths.Material.matElitePoisonOverlay.Load<Material>();
            mats[1] = Utils.Paths.Material.matElitePoisonAreaIndicator.Load<Material>();
            renderer.SetSharedMaterials(mats, 2);

            MalachiteDebuffZone.RemoveComponent<BuffWard>();

            SphereZone zone = MalachiteDebuffZone.AddComponent<SphereZone>();
            zone.rangeIndicator = areaIndicator;
            zone.isInverted = true;
            zone.radius = 30;

            MalachiteDebuffZone.RemoveComponent<SlowDownProjectiles>();

            MalachiteDebuffZone.AddComponent<ZoneController>();
        }

        internal class TurretSpawner : MonoBehaviour
        {
            private List<TurretController> activeTurrets = new();
            private float startTime;
            private CharacterBody body;
            private GameObject zoneInstance;
            private int turretCount;

            private void Start()
            {
                body = GetComponent<CharacterBody>();
                startTime = Run.instance.GetRunStopwatch();
                bool e3 = Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled;
                turretCount = e3 ? turretCountE3 : TurretCount;

                for (int i = 0; i < turretCount; i++)
                {
                    GameObject turret = Instantiate(MalachiteTurret);
                    TurretController controller = turret.GetComponent<TurretController>();
                    controller.owner = body;
                    activeTurrets.Add(controller);
                    NetworkServer.Spawn(turret);
                }

                zoneInstance = GameObject.Instantiate(MalachiteDebuffZone, base.transform);
                NetworkServer.Spawn(zoneInstance);
            }

            private void FixedUpdate()
            {
                for (int i = 0; i < turretCount; i++)
                {
                    if (!activeTurrets[i].rb)
                    {
                        continue;
                    }

                    float elapsed = Run.instance.GetRunStopwatch() - startTime;
                    Vector3 plane1 = Vector3.up;
                    Vector3 plane2 = Vector3.forward;

                    Vector3 targetPosition = (body.footPosition + new Vector3(0, 2, 0)) + Quaternion.AngleAxis(360 / turretCount * i + elapsed / 10 * 360, plane1) * plane2 * (3);
                    float vel = body.isSprinting ? body.moveSpeed * body.sprintingSpeedMultiplier * 1.35f : body.moveSpeed * 1.35f;

                    Vector3 currentPos = activeTurrets[i].rb.position;
                    Vector3 lerpedPosition = Vector3.Lerp(currentPos, targetPosition, vel * Time.fixedDeltaTime);

                    activeTurrets[i].rb.MovePosition(lerpedPosition);
                }
            }

            private void OnDestroy()
            {
                for (int i = 0; i < activeTurrets.Count; i++)
                {
                    activeTurrets[i].Suicide();
                }

                if (zoneInstance)
                {
                    Destroy(zoneInstance);
                }
            }

            private void OnDisable()
            {
                for (int i = 0; i < activeTurrets.Count; i++)
                {
                    activeTurrets[i].Suicide();
                }

                if (zoneInstance)
                {
                    Destroy(zoneInstance);
                }
            }
        }

        internal class TurretController : MonoBehaviour
        {
            public HurtBox target;
            internal CharacterBody owner;
            internal Rigidbody rb;
            private float stopwatch = 0f;
            private float delay = 1.2f;

            private void Start()
            {
                rb = GetComponent<Rigidbody>();
            }

            private void FixedUpdate()
            {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= delay)
                {
                    stopwatch = 0f;
                    RefreshTarget();
                    if (target)
                    {
                        Vector3 aim = (target.transform.position - base.transform.position).normalized;
                        FireProjectileInfo info = new()
                        {
                            damage = Run.instance ? 5f + Mathf.Sqrt(Run.instance.ambientLevel * 120f) / Mathf.Sqrt(Run.instance.participatingPlayerCount) : 0f,
                            position = base.transform.position,
                            rotation = Util.QuaternionSafeLookRotation(aim),
                            owner = owner.gameObject,
                            projectilePrefab = Utils.Paths.GameObject.UrchinSeekingProjectile.Load<GameObject>()
                        };

                        ProjectileManager.instance.FireProjectile(info);
                        AkSoundEngine.PostEvent(Events.Play_elite_antiHeal_turret_shot, base.gameObject);
                    }
                }
            }

            private void RefreshTarget()
            {
                SphereSearch search = new();
                search.radius = 30f;
                search.origin = owner.footPosition;
                search.mask = LayerIndex.entityPrecise.mask;
                search.RefreshCandidates();
                search.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(owner.teamComponent.teamIndex));
                search.OrderCandidatesByDistance();
                target = search.GetHurtBoxes().FirstOrDefault();
            }

            internal void Suicide()
            {
                Destroy(base.gameObject);
            }
        }

        internal class ZoneController : MonoBehaviour
        {
            private SphereZone zone;

            private void Start()
            {
                zone = GetComponent<SphereZone>();
                On.RoR2.HealthComponent.Heal += Reduce;
            }

            private float Reduce(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask mask, bool regen)
            {
                if (zone && zone.IsInBounds(self.body.footPosition))
                {
                    amount *= HealingMultiplier;
                }
                return orig(self, amount, mask, regen);
            }

            private void OnDestroy()
            {
                On.RoR2.HealthComponent.Heal -= Reduce;
            }
        }
    }
}