using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Mending : EliteBase
    {
        public override string Name => "Elites :::: Mending";

        [ConfigField("Healing Beam Radius", "", 25f)]
        public static float radius;

        [ConfigField("Heal Coefficient Per Second", "", 3f)]
        public static float healFraction;

        [ConfigField("Heal Nova Radius", "", 10f)]
        public static float healNovaRadius;

        [ConfigField("On Hit Healing Target Regen Boost", "", 5f)]
        public static float onHitHealingTargetRegenBoost;

        [ConfigField("On Hit Healing Target Regen Boost Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 7f)]
        public static float onHitHealingTargetRegenBoostE3;

        public static BuffDef regenBoost;

        private static GameObject healNovaPrefab;

        public override void Init()
        {
            base.Init();

            var medkitIcon = Utils.Paths.Texture2D.texBuffMedkitHealIcon.Load<Texture2D>();

            regenBoost = ScriptableObject.CreateInstance<BuffDef>();
            regenBoost.isHidden = false;
            regenBoost.canStack = false;
            regenBoost.isDebuff = false;
            regenBoost.iconSprite = Sprite.Create(medkitIcon, new Rect(0f, 0f, medkitIcon.width, medkitIcon.height), new Vector2(0f, 0f));

            ContentAddition.AddBuffDef(regenBoost);

            healNovaPrefab = Utils.Paths.GameObject.RailgunnerMineAltDetonated.Load<GameObject>().InstantiateClone("HealNova");
            healNovaPrefab.RemoveComponent<SlowDownProjectiles>();

            Transform areaIndicator = healNovaPrefab.transform.Find("AreaIndicator");
            Transform softGlow = areaIndicator.Find("SoftGlow");
            Transform sphere = areaIndicator.Find("Sphere");
            Transform light = areaIndicator.Find("Point Light");
            Transform core = areaIndicator.Find("Core");

            core.gameObject.SetActive(false);
            softGlow.gameObject.SetActive(false);

            Light plight = light.GetComponent<Light>();
            plight.color = Color.green;

            MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
            Material[] mats = renderer.sharedMaterials;
            mats[0] = Utils.Paths.Material.matAffixEarthSphereIndicator.Load<Material>();
            mats[1] = Utils.Paths.Material.matAffixEarthSphereIndicator.Load<Material>();
            renderer.SetSharedMaterials(mats, 2);

            healNovaPrefab.RemoveComponent<BuffWard>();

            SphereZone zone = healNovaPrefab.AddComponent<SphereZone>();
            zone.rangeIndicator = areaIndicator;
            zone.radius = healNovaRadius;

            if (!healNovaPrefab.GetComponent<ProjectileDamage>())
            {
                healNovaPrefab.AddComponent<ProjectileDamage>();
            }

            healNovaPrefab.AddComponent<HealNovaController>();

            ContentAddition.AddProjectile(healNovaPrefab);
        }

        public override void Hooks()
        {
            On.RoR2.AffixEarthBehavior.FixedUpdate += Disable;
            On.RoR2.AffixEarthBehavior.Start += Overwrite;
            On.RoR2.GlobalEventManager.OnHitEnemy += Imbue;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(regenBoost))
            {
                bool e3 = Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled;
                var regenStack = e3 ? onHitHealingTargetRegenBoost : onHitHealingTargetRegenBoostE3;
                args.baseRegenAdd += regenStack + 0.2f * regenStack * (sender.level - 1);
            }
        }

        private static void Imbue(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim)
        {
            orig(self, info, victim);
            if (NetworkServer.active && info.attacker)
            {
                MendingController controller = info.attacker.GetComponent<MendingController>();
                if (!controller)
                {
                    return;
                }

                if (Util.CheckRoll(100f * info.procCoefficient))
                {
                    if (Util.HasEffectiveAuthority(info.attacker))
                    {
                        FireProjectileInfo pinfo = new()
                        {
                            position = info.position,
                            projectilePrefab = healNovaPrefab,
                            owner = info.attacker,
                            damage = info.attacker.GetComponent<CharacterBody>().damage * 0.33f
                        };
                        ProjectileManager.instance.FireProjectile(pinfo);
                    }
                }

                if (controller.target)
                {
                    controller.target.body.AddTimedBuff(regenBoost, 2f);
                }
            }
        }

        private static void Disable(On.RoR2.AffixEarthBehavior.orig_FixedUpdate orig, AffixEarthBehavior self)
        {
            return;
        }

        private static void Overwrite(On.RoR2.AffixEarthBehavior.orig_Start orig, AffixEarthBehavior self)
        {
            self.body.gameObject.AddComponent<MendingController>();
            return;
        }

        private class MendingController : MonoBehaviour
        {
            private TetherVfxOrigin vfxOrigin;
            private float healRate => healFraction;
            public HealthComponent target;
            private float stopwatch = 0f;
            private float delay = 1f;
            private TeamIndex team;
            private CharacterBody body;

            private void Start()
            {
                vfxOrigin = base.gameObject.AddComponent<TetherVfxOrigin>();
                vfxOrigin.tetherPrefab = Utils.Paths.GameObject.AffixEarthTetherVFX.Load<GameObject>();
                team = GetComponent<TeamComponent>().teamIndex;
                body = GetComponent<CharacterBody>();
            }

            private void FixedUpdate()
            {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= delay)
                {
                    stopwatch = 0f;
                    target = FetchTarget();
                    if (target && NetworkServer.active)
                    {
                        target.Heal(body.damage * healFraction, new(), true);
                    }

                    if (target)
                    {
                        vfxOrigin.SetTetheredTransforms(new() { target.transform });
                    }
                    else
                    {
                        if (vfxOrigin.tetheredTransforms != null && vfxOrigin.tetheredTransforms.Count > 0)
                        {
                            vfxOrigin.RemoveTetherAt(0);
                        }
                    }

                    if (!body.HasBuff(DLC1Content.Buffs.EliteEarth))
                    {
                        Destroy(vfxOrigin);
                        Destroy(this);
                    }
                }
            }

            private HealthComponent FetchTarget()
            {
                SphereSearch search = new()
                {
                    origin = base.transform.position,
                    radius = radius,
                    mask = LayerIndex.entityPrecise.mask
                };
                search.RefreshCandidates();
                search.OrderCandidatesByDistance();
                search.FilterCandidatesByDistinctHurtBoxEntities();
                return search.GetHurtBoxes().FirstOrDefault(x => CheckIsValid(x.healthComponent))?.healthComponent ?? null;
            }

            private bool CheckIsValid(HealthComponent com)
            {
                if (com.body == body)
                {
                    return false;
                }
                if (com.body.teamComponent.teamIndex != team)
                {
                    return false;
                }
                if (com.body.HasBuff(DLC1Content.Buffs.EliteEarth))
                {
                    return false;
                }
                MendingController[] controllers = GameObject.FindObjectsOfType<MendingController>();
                foreach (MendingController controller in controllers)
                {
                    if (controller != this && controller.target == com)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private class HealNovaController : MonoBehaviour
        {
            private float radius;
            private float maxRadius;
            private float radiusPerSecond => maxRadius / 1.5f;
            private float stopwatch = 0f;
            private float lifespan = 2f;
            private SphereZone zone;
            private float damage;
            private float healStopwatch;
            private float delay = 1f / 3f;
            private TeamIndex index;
            private GameObject owner;
            private float heal;

            private void Start()
            {
                zone = GetComponent<SphereZone>();
                maxRadius = healNovaRadius;
                index = GetComponent<TeamFilter>().teamIndex;
                owner = GetComponent<ProjectileController>().owner;
                heal = GetComponent<ProjectileDamage>().damage;
            }

            private void FixedUpdate()
            {
                stopwatch += Time.fixedDeltaTime;

                if (stopwatch <= lifespan && radius < maxRadius)
                {
                    radius += radiusPerSecond * Time.fixedDeltaTime;
                    zone.radius = radius;
                }
                else if (stopwatch >= lifespan)
                {
                    radius -= radiusPerSecond * Time.fixedDeltaTime;
                    zone.radius = radius;
                    if (radius <= 0)
                    {
                        Destroy(base.gameObject);
                    }
                }

                if (!NetworkServer.active)
                {
                    return;
                }

                healStopwatch += Time.fixedDeltaTime;
                if (healStopwatch >= delay)
                {
                    healStopwatch = 0f;
                    List<TeamComponent> healed = new();

                    foreach (TeamComponent com in TeamComponent.GetTeamMembers(index))
                    {
                        if (!healed.Contains(com) && com.body && zone.IsInBounds(com.body.corePosition) && !com.body.HasBuff(DLC1Content.Buffs.EliteEarth))
                        {
                            com.body.healthComponent.HealFraction(heal, new());
                        }
                    }
                }
            }
        }
    }
}