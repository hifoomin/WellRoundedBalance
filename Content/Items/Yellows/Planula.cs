using MonoMod.Cil;
using EntityStates.GrandParent;
using UnityEngine.Rendering.PostProcessing;

namespace WellRoundedBalance.Items.Yellows
{
    public class Planula : ItemBase
    {
        public static GameObject sunPrefabLessPP;
        public override string Name => ":: Items :::: Yellows :: Planula";
        public override ItemDef InternalPickup => RoR2Content.Items.ParentEgg;

        public override string PickupText => "Summon the unmatched power of the sun after standing still for 1 second.";

        public override string DescText => "After standing still for <style=cIsDamage>1</style> second, summon <style=cIsDamage>the unmatched power of the sun</style> that <style=cIsDamage>ignites</style> enemies every <style=cIsDamage>0.5s</style> for <style=cIsDamage>5s</style> <style=cStack>(+3s per stack)</style>.";

        public override void Init()
        {
            sunPrefabLessPP = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.GrandParentSun.Load<GameObject>(), "THEFUCKINGSUN");
            var postProcessingObject = sunPrefabLessPP.transform.GetChild(0).GetChild(0);
            var postProcessVolume = postProcessingObject.GetComponent<PostProcessVolume>();
            postProcessVolume.weight = 1f;

            var profile = postProcessVolume.sharedProfile;
            var bloom = profile.GetSetting<Bloom>();
            bloom.SetAllOverridesTo(true);
            bloom.intensity.value = 0.3f;

            var colorGrading = profile.GetSetting<ColorGrading>();
            colorGrading.SetAllOverridesTo(true);
            colorGrading.saturation.value = -4f;
            colorGrading.postExposure.value = 0.1f;
            colorGrading.contrast.value = -15f;
            colorGrading.mixerRedOutBlueIn.overrideState = false;
            colorGrading.mixerGreenOutRedIn.overrideState = false;

            PrefabAPI.RegisterNetworkPrefab(sunPrefabLessPP);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                body.AddItemBehavior<PlanulaController>(body.inventory.GetItemCount(RoR2Content.Items.ParentEgg));
            }
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(15f),
                x => x.MatchMul()))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Logger.LogError("Failed to apply Planula Healing hook");
            }
        }
    }

    public class PlanulaController : CharacterBody.ItemBehavior
    {
        private float timer = 0;
        private float burnDistanceBase = 10000;
        private float burnInterval = 0.5f;
        private float burnDuration = 5f;

        private GameObject sunInstance;

        public Vector3? sunSpawnPosition;

        private static float sunPrefabDiameter = 50f;
        private float sunPlacementMinDistance = 5f;
        private static float sunPlacementIdealAltitudeBonus = 50f;

        private void Start()
        {
            sunPlacementMinDistance += body.radius;
            sunPlacementIdealAltitudeBonus += body.radius;
            burnDuration = 5f + 3f * (stack - 1);
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            var what = stack > 0 && body.notMovingStopwatch >= 1f;

            if (what)
            {
                if (sunInstance)
                {
                    timer += Time.fixedDeltaTime;
                    while (timer > burnInterval)
                    {
                        timer -= burnInterval;
                    }
                }
                else
                {
                    sunSpawnPosition = FindSunSpawnPosition(body.corePosition);

                    if (sunSpawnPosition != null)
                    {
                        sunInstance = CreateSun(sunSpawnPosition.Value);
                    }
                    var teamFilter = sunInstance.GetComponent<TeamFilter>();
                    teamFilter.teamIndex = body.teamComponent.teamIndex;

                    var grandParentSunController = sunInstance.GetComponent<GrandParentSunController>();
                    grandParentSunController.burnDuration = burnInterval * stack;
                    grandParentSunController.nearBuffDuration = burnDuration;
                    grandParentSunController.maxDistance = burnDistanceBase;
                    grandParentSunController.minimumStacksBeforeApplyingBurns = 0;
                    grandParentSunController.cycleInterval = burnInterval;

                    var areaIndicator = sunInstance.transform.Find("AreaIndicator");
                    if (areaIndicator)
                    {
                        areaIndicator.localScale = Vector3.one * burnDistanceBase / 5;
                    }

                    timer = 0;
                }
            }
            else
            {
                DestroySun();
            }

            if (sunInstance != what)
            {
                if (what)
                {
                }
                else
                {
                    DestroySun();
                }
            }
        }

        private void OnDisable()
        {
            DestroySun();
        }

        private void DestroySun()
        {
            if (sunInstance)
            {
                Destroy(sunInstance);
                sunInstance = null;
            }
        }

        private GameObject CreateSun(Vector3 sunSpawnPosition)
        {
            var sun = Instantiate(Planula.sunPrefabLessPP, sunSpawnPosition, Quaternion.identity);
            sun.GetComponent<GenericOwnership>().ownerObject = gameObject;
            NetworkServer.Spawn(sun);
            return sun;
        }

        public static Vector3? FindSunSpawnPosition(Vector3 searchOrigin)
        {
            Vector3? vector = searchOrigin;
            if (vector != null)
            {
                Vector3 value = vector.Value;
                var maxPos = sunPlacementIdealAltitudeBonus;
                var halfDiameter = sunPrefabDiameter * 0.5f;
                if (Physics.Raycast(value, Vector3.up, out RaycastHit raycastHit, ChannelSun.sunPlacementIdealAltitudeBonus + halfDiameter, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    maxPos = Mathf.Clamp(raycastHit.distance - halfDiameter, 0f, maxPos);
                }
                value.y += maxPos;
                return new Vector3?(value);
            }
            return null;
        }
    }
}