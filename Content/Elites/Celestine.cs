using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.Rendering.PostProcessing;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Celestine : EliteBase<Celestine>
    {
        public override string Name => ":: Elites ::: Celestine";
        public static BuffDef CelestineBoost;
        public static Material CelestineOverlay;
        public static BuffDef Blindness;
        public static GameObject BlindnessWard;
        public static PostProcessVolume CelestinePPV;

        [ConfigField("Fog Radius", "", 35f)]
        public static float fogRadius;

        [ConfigField("Fog Radius", "Only applies if you have Eclipse Changes enabled.", 55f)]
        public static float fogRadiusE3;

        [ConfigField("Fog Lifetime", "", 10f)]
        public static float fogLifetime;

        [ConfigField("Fog Blindness Duration", "", 2.5f)]
        public static float fogBlindnessDuration;

        [ConfigField("Bubble Armor Gain", "", 20f)]
        public static float bubbleArmorGain;

        [ConfigField("Bubble Attack Speed Gain", "", 0.4f)]
        public static float bubbleAttackSpeedGain;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.AffixHauntedBehavior.FixedUpdate += NoMoreInvis;

            CelestineBoost = ScriptableObject.CreateInstance<BuffDef>();
            CelestineBoost.buffColor = Color.cyan;
            CelestineBoost.canStack = false;
            CelestineBoost.isHidden = true;
            CelestineBoost.name = "Celestine Elite Stat Boost";

            ContentAddition.AddBuffDef(CelestineBoost);

            Blindness = ScriptableObject.CreateInstance<BuffDef>();
            Blindness.buffColor = new Color32(3, 49, 79, byte.MaxValue);
            Blindness.iconSprite = Utils.Paths.BuffDef.bdCloak.Load<BuffDef>().iconSprite;
            Blindness.canStack = false;
            Blindness.isHidden = false;
            Blindness.isDebuff = true;
            Blindness.name = "Celestine Elite Blind";

            ContentAddition.AddBuffDef(Blindness);

            GameObject holder = new("Celestine Fog");
            Object.DontDestroyOnLoad(holder);
            holder.layer = LayerIndex.postProcess.intVal;
            CelestinePPV = holder.AddComponent<PostProcessVolume>();
            Object.DontDestroyOnLoad(CelestinePPV);
            CelestinePPV.isGlobal = true;
            CelestinePPV.weight = 0f;
            CelestinePPV.priority = float.MaxValue;
            PostProcessProfile postProcessProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            Object.DontDestroyOnLoad(postProcessProfile);
            postProcessProfile.name = "Celesting Fog PP";
            RampFog fog = postProcessProfile.AddSettings<RampFog>();
            fog.SetAllOverridesTo(true, true);
            fog.fogColorStart.value = new Color32(0, 0, 0, 165);
            fog.fogColorMid.value = new Color32(1, 2, 44, byte.MaxValue);
            fog.fogColorEnd.value = new Color32(3, 49, 79, byte.MaxValue);
            fog.skyboxStrength.value = 0.02f;
            fog.fogPower.value = 0.35f;
            fog.fogIntensity.value = 1f;
            fog.fogZero.value = 0f;
            fog.fogOne.value = 0.3f;
            DepthOfField dof = postProcessProfile.AddSettings<DepthOfField>();
            dof.SetAllOverridesTo(true, true);
            dof.aperture.value = 5f;
            dof.focalLength.value = 68.31f;
            dof.focusDistance.value = 5f;

            CelestinePPV.sharedProfile = postProcessProfile;

            RecalculateStatsAPI.GetStatCoefficients += StatIncrease;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;

            On.RoR2.CharacterBody.GetVisibilityLevel_CharacterBody += HandleAIBlindness;
            On.RoR2.CharacterBody.FixedUpdate += HandlePlayerBlindness;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;

            CelestineOverlay = Utils.Paths.Material.matMoonbatteryGlassOverlay.Load<Material>();
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            var damageInfo = damageReport.damageInfo;

            if (damageInfo.procCoefficient == 0f || damageInfo.rejected || !NetworkServer.active || damageInfo.dotIndex != DotController.DotIndex.None) return;

            var attackerBody = damageReport.attackerBody;
            if (!attackerBody)
            {
                return;
            }

            var victimBody = damageReport.victimBody;
            if (!victimBody)
            {
                return;
            }

            if (attackerBody.HasBuff(RoR2Content.Buffs.AffixHaunted) && Util.CheckRoll(100f * damageInfo.procCoefficient))
            {
                var ward = GetWard();
                ward.GetComponent<TeamFilter>().teamIndex = attackerBody.teamComponent.teamIndex;
                ward.transform.position = victimBody.footPosition;
                NetworkServer.Spawn(ward);
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixHaunted")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Celestine Elite On Hit hook");
            }
        }

        private static void StatIncrease(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (NetworkServer.active && self.HasBuff(CelestineBoost) && !self.HasBuff(RoR2Content.Buffs.AffixHaunted))
            {
                args.baseAttackSpeedAdd += bubbleAttackSpeedGain;
                args.armorAdd += bubbleArmorGain;
            }
        }

        private static GameObject GetWard() // this sucks lmfao, please replace with PrefabAPI stuff
        {
            bool e3 = Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled;
            BlindnessWard = new("Blindness Ward") { layer = LayerIndex.defaultLayer.intVal };
            TeamFilter filter = BlindnessWard.AddComponent<TeamFilter>();
            BlindnessWard.AddComponent<MeshRenderer>().material = Utils.Paths.Material.matHauntedAura.Load<Material>();
            filter.teamIndex = TeamIndex.None;
            BuffWard ward = BlindnessWard.AddComponent<BuffWard>();
            ward.buffDef = Blindness;
            ward.invertTeamFilter = true;
            ward.radius = e3 ? fogRadius : fogRadiusE3;
            ward.buffDuration = fogBlindnessDuration;
            ward.expires = true;
            ward.expireDuration = fogLifetime;
            ward.interval = 0.2f;
            var indicator = Object.Instantiate(Utils.Paths.GameObject.AffixHauntedWard.Load<GameObject>().transform.Find("Indicator").Find("IndicatorSphere").gameObject);
            indicator.transform.SetParent(BlindnessWard.transform);
            indicator.transform.localScale = new Vector3(0, 0, 0);
            indicator.AddComponent<DestroyOnTimer>().duration = ward.expireDuration;
            indicator.AddComponent<SimpleVisualScale>();

            return BlindnessWard;
        }

        private class SimpleVisualScale : MonoBehaviour // also replace this with ObjectScaleCurve
        {
            private float radius;
            private float _radius;
            private float age;
            private float _age;

            private void Start()
            {
                BuffWard ward = transform.parent.GetComponent<BuffWard>();
                radius = ward.radius * 2;
                age = ward.expireDuration;
                _radius = 0;
                _age = 0;
            }

            private void FixedUpdate()
            {
                if (_age < age)
                {
                    _age += Time.fixedDeltaTime;
                    if (_age >= age) radius = 0;
                }
                if (_radius == radius) return;
                _radius = Mathf.Lerp(_radius, radius, 0.1f);
                transform.localScale = new Vector3(_radius, _radius, _radius);
                if (Mathf.Abs(radius - _radius) < 0.001f)
                {
                    _radius = radius;
                    if (radius > 0) age -= _age;
                }
            }
        }

        private static VisibilityLevel HandleAIBlindness(On.RoR2.CharacterBody.orig_GetVisibilityLevel_CharacterBody orig, CharacterBody self, CharacterBody observer)
        {
            VisibilityLevel ret = orig(self, observer);
            if (observer.HasBuff(Blindness)) return VisibilityLevel.Cloaked;
            return ret;
        }

        private static void HandlePlayerBlindness(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody body)
        {
            orig(body);
            if (LocalUserManager.GetFirstLocalUser()?.cachedBody == body)
            {
                float weight = body.HasBuff(Blindness) ? 1 : 0;
                if (weight == CelestinePPV.weight) return;
                CelestinePPV.weight = Mathf.Lerp(CelestinePPV.weight, weight, 0.1f);
                if (Mathf.Abs(CelestinePPV.weight - weight) < 0.001f) CelestinePPV.weight = weight;
            }
        }

        private static void NoMoreInvis(On.RoR2.CharacterBody.AffixHauntedBehavior.orig_FixedUpdate orig, CharacterBody.AffixHauntedBehavior self)
        {
            orig(self);
            if (self.affixHauntedWard)
            {
                BuffWard ward = self.affixHauntedWard.GetComponent<BuffWard>();
                ward.buffDef = CelestineBoost;
            }
        }
    }
}