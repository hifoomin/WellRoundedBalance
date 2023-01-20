namespace WellRoundedBalance.Items.Yellows
{
    internal class DefenseNucleus : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Defense Nucleus";

        public override string InternalPickupToken => GetToken(Utils.Paths.ItemDef.MinorConstructOnKill);

        public override string PickupText => "Reduce damage taken. Store damage taken and release it as a devastating laser upon using your special.";

        public override string DescText => "Gain <style=cIsUtility>25%</style> damage resistance. Upon using your <style=cIsUtility>special</style>, unleash a devastating laser for <style=cIsDamage>50%</style> <style=cStack>(+50% per stack)</style> of the <style=cIsUtility>resisted damage</style> per second.";
        public static GameObject BubbleShieldEffectPrefab;

        public override void Init()
        {
            BubbleShieldEffectPrefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MajorConstructBubbleShield.Load<GameObject>(), "DucleusShield");
            BubbleShieldEffectPrefab.RemoveComponent<TeamFilter>();
            BubbleShieldEffectPrefab.RemoveComponent<NetworkedBodyAttachment>();

            PrefabAPI.RegisterNetworkPrefab(BubbleShieldEffectPrefab);
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += Resistance;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
            On.RoR2.CharacterBody.OnSkillActivated += SkillActivated;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += (orig, self, info) =>
            {
                if (info.projectilePrefab == GlobalEventManager.CommonAssets.minorConstructOnKillProjectile)
                {
                }
                else
                {
                    orig(self, info);
                }
            };
        }

        private void SkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
        {
            orig(self, skill);
            if (self.GetComponent<DefenseNucleusBehavior>() && NetworkServer.active)
            {
                DefenseNucleusBehavior behavior = self.GetComponent<DefenseNucleusBehavior>();
                if (self.skillLocator && self.skillLocator.FindSkillSlot(skill) == SkillSlot.Special && behavior.StoredDamage > 0)
                {
                    behavior.Fire();
                }
            }
        }

        private void Resistance(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damage)
        {
            if (self.GetComponent<DefenseNucleusBehavior>() && NetworkServer.active)
            {
                DefenseNucleusBehavior behavior = self.GetComponent<DefenseNucleusBehavior>();
                float amount = damage.damage * 0.75f;
                behavior.StoredDamage += amount;
                damage.damage *= 0.75f;
            }
            orig(self, damage);
        }

        private void AddBehavior(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (body && body.inventory)
            {
                body.AddItemBehavior<DefenseNucleusBehavior>(body.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill));
            }
        }

        private class DefenseNucleusBehavior : CharacterBody.ItemBehavior
        {
            public float StoredDamage = 0f;
            private float TotalStoredDamage = 0f;

            private bool shouldFireLaser = false;
            private float coefficientPerSecond => stack * 0.5f;
            private float ticks = 5;
            private float coeffPerTick => coefficientPerSecond / ticks;
            private float delay => 1f / ticks;
            private GameObject laserPrefab => Utils.Paths.GameObject.LaserMajorConstruct.Load<GameObject>();
            private GameObject hitEffectPrefab => Utils.Paths.GameObject.Hitspark1.Load<GameObject>();
            private GameObject laserInstance;
            private Transform laserEndTransform;
            private GameObject bubbleInstance;

            private float stopwatch = 0f;

            private void Start()
            {
                bubbleInstance = GameObject.Instantiate(BubbleShieldEffectPrefab, body.transform);
            }

            private void FixedUpdate()
            {
                if (shouldFireLaser)
                {
                    stopwatch += Time.fixedDeltaTime;

                    if (stopwatch >= delay)
                    {
                        BulletAttack attack = new()
                        {
                            origin = body.corePosition,
                            damage = TotalStoredDamage * coeffPerTick,
                            maxDistance = 10000f,
                            aimVector = body.inputBank.aimDirection,
                            procChainMask = new(),
                            procCoefficient = 0.5f,
                            hitEffectPrefab = hitEffectPrefab,
                            radius = 5f,
                            smartCollision = false,
                            owner = body.gameObject,
                            weapon = body.gameObject,
                            isCrit = Util.CheckRoll(body.crit, body.master)
                        };
                        attack.Fire();
                        StoredDamage -= (TotalStoredDamage * coeffPerTick);

                        stopwatch = 0f;
                    }

                    if (StoredDamage <= 0)
                    {
                        shouldFireLaser = false;
                        Unfire();
                    }
                }

                if (laserInstance && laserEndTransform)
                {
                    Vector3 pos = body.inputBank.GetAimRay().GetPoint(1000);
                    laserEndTransform.position = pos;
                }
            }

            private void Unfire()
            {
                GameObject.Destroy(laserInstance);
                laserEndTransform = null;
                laserInstance = null;
            }

            public void Fire()
            {
                if (shouldFireLaser) return;
                TotalStoredDamage = StoredDamage;
                laserInstance = GameObject.Instantiate(laserPrefab, body.transform);
                laserEndTransform = laserInstance.GetComponent<ChildLocator>().FindChild("LaserEnd");
                shouldFireLaser = true;
            }

            private void OnDestroy()
            {
                if (bubbleInstance)
                {
                    GameObject.Destroy(bubbleInstance);
                }
            }
        }
    }
}