using UnityEngine.Rendering.PostProcessing;

namespace WellRoundedBalance.Elites
{
    internal class Celestine : EliteBase
    {
        public override string Name => "Elites :::: Celestine";
        public static BuffDef CelestineBoost;
        public static Material CelestineOverlay;
        public static BuffDef Blindness;
        public static GameObject BlindnessWard;
        public static PostProcessVolume BlindnessPPV;

        // parameters (config these later)
        public const float BlindnessWardRadius = 6;
        public const float BlindnessWardLifetime = 8;
        public const float BlindnessDuration = 3;

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
            CelestineBoost.name = "Celestine Boost";

            ContentAddition.AddBuffDef(CelestineBoost);

            Blindness = ScriptableObject.CreateInstance<BuffDef>();
            Blindness.buffColor = new Color(124f / 255f, 78f / 255f, 222f / 255f);
            Blindness.iconSprite = Utils.Paths.BuffDef.bdCloak.Load<BuffDef>().iconSprite;
            Blindness.canStack = false;
            Blindness.isHidden = false;
            Blindness.isDebuff = true;
            Blindness.name = "Blindness";

            ContentAddition.AddBuffDef(Blindness);

            GameObject holder = new GameObject("Definitely Not Stolen From Artifact Of Blindness");
            Object.DontDestroyOnLoad(holder);
            holder.layer = LayerIndex.postProcess.intVal;
            BlindnessPPV = holder.AddComponent<PostProcessVolume>();
            Object.DontDestroyOnLoad(BlindnessPPV);
            BlindnessPPV.isGlobal = true;
            BlindnessPPV.weight = 0f;
            BlindnessPPV.priority = float.MaxValue;
            PostProcessProfile postProcessProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            Object.DontDestroyOnLoad(postProcessProfile);
            postProcessProfile.name = "Nuh uh, No Sir-ee Bob!";
            RampFog fog = postProcessProfile.AddSettings<RampFog>();
            fog.SetAllOverridesTo(true, true);
            fog.fogColorStart.value = new Color32(45, 45, 53, 165);
            fog.fogColorMid.value = new Color32(44, 44, 56, byte.MaxValue);
            fog.fogColorEnd.value = new Color32(44, 44, 56, byte.MaxValue);
            fog.skyboxStrength.value = 0.02f;
            fog.fogPower.value = 0.35f;
            fog.fogIntensity.value = 0.99f;
            fog.fogZero.value = 0f;
            fog.fogOne.value = 0.05f;
            ChromaticAberration ab = postProcessProfile.AddSettings<ChromaticAberration>();
            ab.SetAllOverridesTo(true, true);
            ab.intensity.value = 0.15f;
            ab.fastMode.value = false;
            DepthOfField dof = postProcessProfile.AddSettings<DepthOfField>();
            dof.SetAllOverridesTo(true, true);
            dof.aperture.value = 5f;
            dof.focalLength.value = 68.31f;
            dof.focusDistance.value = 5f;
            Grain grain = postProcessProfile.AddSettings<Grain>();
            grain.SetAllOverridesTo(true, true);
            grain.intensity.value = 0.09f;
            grain.size.value = 4.57f;
            grain.lumContrib.value = 5.86f;
            grain.colored.value = true;
            Vignette vn = postProcessProfile.AddSettings<Vignette>();
            vn.SetAllOverridesTo(true, true);
            vn.intensity.value = 0.15f;
            vn.roundness.value = 1f;
            vn.smoothness.value = 0.2f;
            vn.rounded.value = false;
            vn.color.value = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            BlindnessPPV.sharedProfile = postProcessProfile;

            RecalculateStatsAPI.GetStatCoefficients += StatIncrease;
            On.RoR2.CharacterModel.UpdateOverlays += HandleOverlay;
            On.RoR2.GlobalEventManager.OnHitEnemy += HandleFog;

            On.RoR2.CharacterBody.GetVisibilityLevel_CharacterBody += HandleAIBlindness;
            On.RoR2.CharacterBody.FixedUpdate += HandlePlayerBlindness;

            CelestineOverlay = Utils.Paths.Material.matMoonbatteryGlassOverlay.Load<Material>();
        }

        private static void StatIncrease(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (NetworkServer.active && self.HasBuff(CelestineBoost))
            {
                args.baseAttackSpeedAdd += 0.4f;
                args.armorAdd += 20;
            }
        }

        private static void HandleOverlay(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);
            if (self.body && self.body.HasBuff(CelestineBoost)) 
            {
                self.currentOverlays[self.activeOverlayCount++] = CelestineOverlay;
            }
        }

        private static void HandleFog(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (damageInfo.procCoefficient == 0f || damageInfo.rejected || !NetworkServer.active || damageInfo.dotIndex != DotController.DotIndex.None) return;
            if (damageInfo?.attacker?.GetComponent<CharacterBody>()?.HasBuff(RoR2Content.Buffs.AffixHaunted) ?? false)
            {
                GameObject ward = GetWard();
                ward.GetComponent<TeamFilter>().teamIndex = damageInfo.attacker.GetComponent<CharacterBody>().teamComponent.teamIndex;
                ward.transform.position = victim.GetComponent<CharacterBody>().footPosition;
                NetworkServer.Spawn(ward);
            }
        }

        private static GameObject GetWard() // this sucks lmfao, please replace with PrefabAPI stuff 
        {
            BlindnessWard = new("Blindness Ward") { layer = LayerIndex.defaultLayer.intVal };
            TeamFilter filter = BlindnessWard.AddComponent<TeamFilter>();
            BlindnessWard.AddComponent<MeshRenderer>().material = Utils.Paths.Material.matHauntedAura.Load<Material>();
            filter.teamIndex = TeamIndex.None;
            BuffWard ward = BlindnessWard.AddComponent<BuffWard>();
            ward.buffDef = Blindness;
            ward.invertTeamFilter = true;
            ward.radius = BlindnessWardRadius;
            ward.buffDuration = BlindnessDuration;
            ward.expires = true;
            ward.expireDuration = BlindnessWardLifetime;
            ward.interval = 0.2f;
            GameObject indicator = Object.Instantiate(Utils.Paths.GameObject.AffixHauntedWard.Load<GameObject>().transform.Find("Indicator").Find("IndicatorSphere").gameObject);
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
                if (weight == BlindnessPPV.weight) return;
                BlindnessPPV.weight = Mathf.Lerp(BlindnessPPV.weight, weight, 0.1f);
                if (Mathf.Abs(BlindnessPPV.weight - weight) < 0.001f) BlindnessPPV.weight = weight;
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