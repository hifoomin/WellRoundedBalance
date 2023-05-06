using EntityStates;
using RoR2.Skills;

namespace WellRoundedBalance.Items.Lunars
{
    public class StridesOfHeresy : ItemBase<StridesOfHeresy>
    {
        public override string Name => ":: Items ::::: Lunars :: Strides of Heresy";
        public override ItemDef InternalPickup => RoR2Content.Items.LunarUtilityReplacement;

        public override string PickupText => "Replace your Utility Skill with 'Shadowfade'.";
        public override string DescText => "<style=cIsUtility>Replace your Utility Skill</style> with <style=cIsUtility>Shadowfade</style>.\n\nDash, <style=cIsHealing>healing</style> for <style=cIsHealing>5% of your maximum health</style>. Lasts <style=cIsUtility>1</style> <style=cStack>(+0.5 per stack)</style> seconds, then <style=cIsHealing>heals</style> for <style=cIsHealing>10% of your maximum health</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            ContentAddition.AddEntityState(typeof(StrideState), out _);

            var stridesSD = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/LunarSkillReplacements/LunarUtilityReplacement.asset").WaitForCompletion();
            stridesSD.activationState = new SerializableEntityStateType(typeof(StrideState));
            stridesSD.baseRechargeInterval = 6f;

            LanguageAPI.Add("SKILL_LUNAR_UTILITY_REPLACEMENT_DESCRIPTION_WRB", "Dash, <style=cIsHealing>healing</style> for <style=cIsHealing>5% of your maximum health</style>. Lasts <style=cIsUtility>1</style> <style=cStack>(+0.5 per stack)</style> seconds, then <style=cIsHealing>heals</style> for <style=cIsHealing>10% of your maximum health</style>.");
        }
    }

    public class StrideState : BaseState
    {
        public float duration = 1;
        public float pastAirControl;
        public static float newAirControl = 0;
        public Vector3 dashVector;
        public float speedCoefficient = 3.5f;
        public static float enterHealPercent = 0.05f;
        public static float endHealPercent = 0.1f;
        public static string enterSound = "Play_item_lunar_use_utilityReplacement_start";
        public static string exitSound = "Play_item_lunar_use_utilityReplacement_end";
        public static GameObject effect = Utils.Paths.GameObject.OmniExplosionCrowstorm.Load<GameObject>();
        public static GameObject overlay = Utils.Paths.GameObject.CrowstormCoreVFX.Load<GameObject>();
        public GameObject overlayInstance;
        public Inventory inventory;
        public CharacterModel characterModel;

        public override void OnEnter()
        {
            base.OnEnter();
            var modelTransform = GetModelTransform();
            characterModel = (modelTransform != null) ? modelTransform.GetComponent<CharacterModel>() : null;

            if (characterModel)
            {
                characterModel.invisibilityCount++;
            }

            Util.PlaySound(enterSound, gameObject);

            if (characterMotor)
            {
                characterMotor.Motor.ForceUnground();
                pastAirControl = characterMotor.airControl;
            }

            dashVector = GetDashVector();

            inventory = characterBody.inventory;
            if (inventory)
            {
                var stack = inventory.GetItemCount(RoR2Content.Items.LunarUtilityReplacement);
                duration = 1f + 0.5f * (stack - 1);
            }

            if (healthComponent)
                healthComponent.HealFraction(enterHealPercent, default);

            overlayInstance = Object.Instantiate(overlay);
            UpdateOverlayPosition();
            SpawnEffect(transform.position);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor)
            {
                characterMotor.airControl = newAirControl;
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion += dashVector * (moveSpeedStat * speedCoefficient * Time.fixedDeltaTime);
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void Update()
        {
            base.Update();
            UpdateOverlayPosition();
        }

        public override void OnExit()
        {
            base.OnExit();

            if (characterModel)
            {
                characterModel.invisibilityCount--;
            }

            Util.PlaySound(exitSound, gameObject);

            if (characterMotor)
                characterMotor.airControl = pastAirControl;

            if (healthComponent)
                healthComponent.HealFraction(endHealPercent, default);

            EntityState.Destroy(overlayInstance);
            SpawnEffect(transform.position);
        }

        public Vector3 GetDashVector()
        {
            return ((inputBank.moveVector == Vector3.zero) ? characterDirection.forward : inputBank.moveVector).normalized;
        }

        public void SpawnEffect(Vector3 origin)
        {
            EffectData effectData = new()
            {
                rotation = Util.QuaternionSafeLookRotation(dashVector),
                origin = origin
            };
            EffectManager.SpawnEffect(effect, effectData, true);
        }

        public void UpdateOverlayPosition()
        {
            if (characterBody)
            {
                if (overlayInstance)
                {
                    overlayInstance.transform.position = characterBody.corePosition;
                }
            }
        }
    }
}