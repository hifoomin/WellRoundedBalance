using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class OddlyShapedOpal : ItemBase<OddlyShapedOpal>
    {
        public static BuffDef opalArmor;
        public static GameObject indicator;
        public override string Name => ":: Items : Whites :: Oddly Shaped Opal";
        public override ItemDef InternalPickup => DLC1Content.Items.OutOfCombatArmor;

        public override string PickupText => "Increase armor for every enemy nearby.";

        public override string DescText =>
            StackDesc(armorGain, armorGainStack, init => $"<style=cIsHealing>Increase armor</style> by <style=cIsHealing>{init}</style>{{Stack}} for every enemy within <style=cIsHealing>" + radius + "m</style> up to <style=cIsHealing>" + maxBuffCount + "</style>" +
            (maxBuffCountStack > 0 ? " <style=cStack>(+" + maxBuffCount + " per stack)</style>" : "") + " times.", noop);

        [ConfigField("Armor Gain", 2.5f)]
        public static float armorGain;

        [ConfigField("Armor Gain per Stack", 2.5f)]
        public static float armorGainStack;

        [ConfigField("Base Max Buff Count", 3)]
        public static int maxBuffCount;

        [ConfigField("Max Buff Count Per Stack", 0)]
        public static int maxBuffCountStack;

        [ConfigField("Armor Gain is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float armorGainIsHyperbolic;

        [ConfigField("Radius", 13f)]
        public static float radius;

        public override void Init()
        {
            var opalIcon = Utils.Paths.Texture2D.texBuffUtilitySkillArmor.Load<Texture2D>();

            opalArmor = ScriptableObject.CreateInstance<BuffDef>();
            opalArmor.isHidden = false;
            opalArmor.canStack = true;
            opalArmor.isCooldown = false;
            opalArmor.isDebuff = false;
            opalArmor.iconSprite = Sprite.Create(opalIcon, new Rect(0f, 0f, opalIcon.width, opalIcon.height), new Vector2(0f, 0f));
            opalArmor.buffColor = new Color32(196, 194, 255, 255);
            opalArmor.name = "Oddly-shaped Opal Armor";

            ContentAddition.AddBuffDef(opalArmor);

            base.Init();
        }

        public override void Hooks()
        {
            indicator = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.NearbyDamageBonusIndicator.Load<GameObject>(), "Opal Visual", true);
            var radiusTrans = indicator.transform.Find("Radius, Spherical");
            radiusTrans.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);

            var opalMat = Object.Instantiate(Utils.Paths.Material.matNearbyDamageBonusRangeIndicator.Load<Material>());
            var cloudTexture = Utils.Paths.Texture2D.PerlinNoise.Load<Texture2D>();
            opalMat.SetTexture("_MainTex", cloudTexture);
            opalMat.SetTexture("_Cloud1Tex", cloudTexture);
            opalMat.SetColor("_TintColor", new Color32(54, 20, 176, 128));

            radiusTrans.GetComponent<MeshRenderer>().material = opalMat;

            PrefabAPI.RegisterNetworkPrefab(indicator);

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            On.RoR2.OutOfCombatArmorBehavior.SetProvidingBuff += (_, __, ___) => { };
            On.RoR2.OutOfCombatArmorBehavior.FixedUpdate += (_, __) => { };
            IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += CharacterBody_UpdateAllTemporaryVisualEffects;
        }

        private void CharacterBody_UpdateAllTemporaryVisualEffects(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC1Content.Buffs), nameof(DLC1Content.Buffs.OutOfCombatArmorBuff))))
            {
                c.Emit(OpCodes.Pop);
                c.EmitDelegate(() => Buffs.Useless.oddlyShapedOpalUseless);
            }
            else Logger.LogError("Failed to apply Oddly Shaped Opal VFX hook");
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (inventory)
            {
                var stack = inventory.GetItemCount(DLC1Content.Items.OutOfCombatArmor);
                args.armorAdd += StackAmount(armorGain, armorGainStack, stack, armorGainIsHyperbolic) * sender.GetBuffCount(opalArmor);
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (NetworkServer.active) characterBody.AddItemBehavior<OddlyShapedOpalController>(characterBody.inventory.GetItemCount(DLC1Content.Items.OutOfCombatArmor));
        }
    }

    public class OddlyShapedOpalController : CharacterBody.ItemBehavior
    {
        public float checkInterval = 0.1f;
        public float timer;
        public float radiusSquared = 169f;
        public float distance = OddlyShapedOpal.radius;
        public TeamIndex ownerIndex;
        public GameObject radiusIndicator;
        public int maxBuffs;

        private void Start()
        {
            ownerIndex = body.teamComponent.teamIndex;
            enableRadiusIndicator = true;
            var radiusTrans = radiusIndicator.transform.GetChild(1);
            radiusTrans.localScale = new Vector3(OddlyShapedOpal.radius * 2f, OddlyShapedOpal.radius * 2f, OddlyShapedOpal.radius * 2f);
            if (stack > 0)
                maxBuffs = OddlyShapedOpal.maxBuffCount + OddlyShapedOpal.maxBuffCountStack * (stack - 1);
            else maxBuffs = 0;
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer < checkInterval)
            {
                return;
            }

            var count = 0;
            for (TeamIndex firstIndex = TeamIndex.Neutral; firstIndex < TeamIndex.Count && count < maxBuffs; firstIndex++)
            {
                if (firstIndex == ownerIndex || firstIndex <= TeamIndex.Neutral)
                {
                    continue;
                }

                foreach (TeamComponent teamComponent in TeamComponent.GetTeamMembers(firstIndex))
                {
                    if ((teamComponent.transform.position - body.corePosition).sqrMagnitude <= radiusSquared)
                    {
                        count++;
                    }
                }
            }
            UpdateBuff(count);
            timer = 0f;
        }

        private void UpdateBuff(int buffCountAdd)
        {
            var currentBuffCount = body.GetBuffCount(OddlyShapedOpal.opalArmor);
            if (currentBuffCount != buffCountAdd)
            {
                var buffCountDiff = buffCountAdd - currentBuffCount;
                if (buffCountDiff > 0)
                {
                    for (int j = 0; j < buffCountDiff; j++)
                    {
                        body.AddBuff(OddlyShapedOpal.opalArmor);
                    }
                }
                else
                {
                    for (int k = 0; k < -buffCountDiff; k++)
                    {
                        body.RemoveBuff(OddlyShapedOpal.opalArmor);
                    }
                }
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
                        radiusIndicator = Instantiate(OddlyShapedOpal.indicator, body.corePosition, Quaternion.identity);
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