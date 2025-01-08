using RoR2.Orbs;

namespace WellRoundedBalance.Items.Greens
{
    public class Razorwire : ItemBase<Razorwire>
    {
        public static GameObject indicator;
        public override string Name => ":: Items :: Greens :: Razorwire";
        public override ItemDef InternalPickup => RoR2Content.Items.Thorns;

        public override string PickupText => "While out of danger, passively damage nearby enemies with thorns.";
        public override string DescText => "Passively damage all enemies within <style=cIsDamage>" + radius + "m</style> for <style=cIsDamage>" + d(baseDamage) + "</style> <style=cStack>(+" + d(damagePerStack) + " per stack)</style> <style=cIsDamage>damage per second</style> while out of danger.";

        [ConfigField("Base Damage", "Decimal.", 2.5f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 2.5f)]
        public static float damagePerStack;

        [ConfigField("Proc Coefficient", 0f)]
        public static float procCoefficient;

        [ConfigField("Radius", 13f)]
        public static float radius;

        public static GameObject razorwireVFX;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            razorwireVFX = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.RazorwireOrbEffect.Load<GameObject>(), "Razorwire VFX", false);

            var orbEffect = razorwireVFX.GetComponent<OrbEffect>();
            orbEffect.enabled = false;

            var head = razorwireVFX.transform.GetChild(0);
            head.gameObject.SetActive(true);

            ContentAddition.AddEffect(razorwireVFX);

            indicator = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.NearbyDamageBonusIndicator.Load<GameObject>(), "Razorwire Visual", true);
            var radiusTrans = indicator.transform.Find("Radius, Spherical");
            radiusTrans.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);

            var razorMat = Object.Instantiate(Utils.Paths.Material.matNearbyDamageBonusRangeIndicator.Load<Material>());
            var cloudTexture = Utils.Paths.Texture2D.texCloudWaterRipples.Load<Texture2D>();
            razorMat.SetTexture("_MainTex", cloudTexture);
            razorMat.SetTexture("_Cloud1Tex", cloudTexture);
            razorMat.SetColor("_TintColor", new Color32(182, 183, 183, 150));

            radiusTrans.GetComponent<MeshRenderer>().material = razorMat;

            PrefabAPI.RegisterNetworkPrefab(indicator);

            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (NetworkServer.active) characterBody.AddItemBehavior<RazorwireController>(characterBody.inventory.GetItemCount(RoR2Content.Items.Thorns));
        }

        private void HealthComponent_TakeDamageProcess(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "thorns"),
                 x => x.MatchLdcI4(0)))
            {
                c.Index += 1;
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, int.MaxValue);
            }
            else
            {
                Logger.LogError("Failed to apply Razorwire Deletion hook");
            }
        }
    }

    public class RazorwireController : CharacterBody.ItemBehavior
    {
        public float damageInterval = 0.2f;
        public float damage;
        public float timer;
        public float radiusSquared = Razorwire.radius * Razorwire.radius;
        public float distance = Razorwire.radius;
        public TeamIndex ownerIndex;
        public GameObject radiusIndicator;

        private void Start()
        {
            ownerIndex = body.teamComponent.teamIndex;
            enableRadiusIndicator = true;
            var radiusTrans = radiusIndicator.transform.GetChild(1);
            radiusTrans.localScale = new Vector3(Razorwire.radius * 2f, Razorwire.radius * 2f, Razorwire.radius * 2f);
            if (stack > 0)
            {
                damage = (Razorwire.baseDamage + Razorwire.damagePerStack * (stack - 1)) * damageInterval;
            }
            else damage = 0;
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;

            if (timer < damageInterval)
            {
                return;
            }

            if (!body.outOfDanger)
            {
                // Main.WRBLogger.LogError("disabling razorwire indicator");
                enableRadiusIndicator = false;
                return;
            }

            // Main.WRBLogger.LogError("enabling razorwire indicator");

            enableRadiusIndicator = true;

            for (TeamIndex firstIndex = TeamIndex.Neutral; firstIndex < TeamIndex.Count; firstIndex++)
            {
                if (firstIndex == ownerIndex || firstIndex <= TeamIndex.Neutral)
                {
                    continue;
                }

                foreach (TeamComponent teamComponent in TeamComponent.GetTeamMembers(firstIndex))
                {
                    var enemyPosition = teamComponent.transform.position;
                    var corePosition = body.corePosition;
                    if ((enemyPosition - corePosition).sqrMagnitude <= radiusSquared)
                    {
                        Damage(teamComponent);
                        /*
                        if (Random.Range(0f, 1f) < 0.12f)
                        {
                            EffectManager.SpawnEffect(Razorwire.razorwireVFX, new EffectData() { start = corePosition, origin = enemyPosition }, true);
                        }
                        */
                    }
                }
            }

            timer = 0f;
        }

        private void Damage(TeamComponent teamComponent)
        {
            var victimBody = teamComponent.body;
            if (!victimBody)
            {
                return;
            }

            var victimHealthComponent = victimBody.healthComponent;
            if (!victimHealthComponent)
            {
                return;
            }

            if (victimHealthComponent)
            {
                var info = new DamageInfo()
                {
                    attacker = gameObject,
                    crit = false,
                    damage = damage * body.damage,
                    force = Vector3.zero,
                    procCoefficient = Razorwire.procCoefficient * Items.Greens._ProcCoefficients.globalProc,
                    damageType = DamageType.Generic,
                    position = victimBody.corePosition,
                    inflictor = gameObject
                };
                victimHealthComponent.TakeDamageProcess(info);
            }
        }

        private bool enableRadiusIndicator
        {
            get
            {
                return radiusIndicator;
            }
            set
            {
                if (enableRadiusIndicator != value)
                {
                    if (value)
                    {
                        radiusIndicator = Instantiate(Razorwire.indicator, body.corePosition, Quaternion.identity);
                        radiusIndicator.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(gameObject, null);
                    }
                    else
                    {
                        Object.Destroy(radiusIndicator);
                        radiusIndicator = null;
                    }
                }
            }
        }

        private void OnDisable()
        {
            enableRadiusIndicator = false;
        }
    }
}