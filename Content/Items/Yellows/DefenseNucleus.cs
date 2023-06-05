using System.Collections;

namespace WellRoundedBalance.Items.Yellows
{
    internal class DefenseNucleus : ItemBase<DefenseNucleus>
    {
        public override string Name => ":: Items :::: Yellows :: Defense Nucleus";

        public override ItemDef InternalPickup => DLC1Content.Items.MinorConstructOnKill;

        public override string PickupText => "Reduce damage taken. Upon using your equipment, fire a devastating laser.";

        public override string DescText => "Gain <style=cIsHealing>" + armorGain + " armor</style>. Upon using your <style=cIsDamage>equipment</style>, unleash a devastating laser for <style=cIsDamage>" + d(baseDamagePerSecond) + "</style> <style=cStack>(+" + d(damagePerSecondPerStack) + " per stack)</style> damage per second for every <style=cIsUtility>second</style> of your <style=cIsUtility>equipment's base cooldown</style>.";

        [ConfigField("Armor Gain", "", 10f)]
        public static float armorGain;

        [ConfigField("Base Damage Per Second", "Decimal.", 0.7f)]
        public static float baseDamagePerSecond;

        [ConfigField("Damage Per Second Per Stack", "Decimal.", 0.7f)]
        public static float damagePerSecondPerStack;

        [ConfigField("Proc Coefficient Per Second", "", 0.25f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            EquipmentSlot.onServerEquipmentActivated += EquipmentSlot_onServerEquipmentActivated;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.DLC1Content/Items", "MinorConstructOnKill")))
            {
                c.Remove();
                c.Emit<Items.Useless>(OpCodes.Ldsfld, nameof(Items.Useless.uselessItem));
            }
            else
            {
                Logger.LogError("Failed to apply Defense Nucleus Deletion hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill);
                if (stack > 0)
                {
                    args.armorAdd += armorGain;
                }
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            var inventory = body.inventory;
            if (!inventory)
            {
                return;
            }
            body.AddItemBehavior<DefenseNucleusBehavior>(inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill));
        }

        private void EquipmentSlot_onServerEquipmentActivated(EquipmentSlot equipmentSlot, EquipmentIndex equipmentIndex)
        {
            var body = equipmentSlot.characterBody;
            if (body)
            {
                var equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);

                var defenseNucleusBehavior = body.GetComponent<DefenseNucleusBehavior>();
                if (equipmentDef && defenseNucleusBehavior && NetworkServer.active)
                {
                    defenseNucleusBehavior.Fire(body, equipmentDef.cooldown);
                }
            }
        }

        public class DefenseNucleusBehavior : CharacterBody.ItemBehavior
        {
            public GameObject laserPrefab = Utils.Paths.GameObject.LaserMajorConstruct.Load<GameObject>();
            public GameObject hitEffectPrefab = Utils.Paths.GameObject.Hitspark1.Load<GameObject>();
            public GameObject laserInstance;
            public Transform laserEndTrans;
            public bool shouldFireLaser = true;

            public float tickRate = 0.1f;
            public int ticks = 0;

            public float totalDamage = 0f;

            public void Start()
            {
                ticks = (int)(1 / tickRate * 5);
            }

            public void Fire(CharacterBody body, float equipmentCooldown)
            {
                totalDamage = body.damage * (baseDamagePerSecond + damagePerSecondPerStack * (stack - 1));

                if (shouldFireLaser)
                {
                    laserInstance = GameObject.Instantiate(laserPrefab, body.transform);
                    NetworkServer.Spawn(laserInstance);

                    StartCoroutine(FireLaser(equipmentCooldown));
                }

                shouldFireLaser = false;
            }

            public void UpdateLaser()
            {
                if (laserInstance && laserEndTrans)
                {
                    Vector3 pos = body.inputBank.GetAimRay().GetPoint(10000);
                    laserEndTrans.position = pos;
                }
            }

            public void DestroyLaser()
            {
                if (laserInstance != null)
                {
                    NetworkServer.Destroy(laserInstance);
                    Destroy(laserInstance);
                    laserInstance = null;
                    laserEndTrans = null;
                }
            }

            public IEnumerator FireLaser(float equipmentCooldown)
            {
                laserEndTrans = laserInstance.GetComponent<ChildLocator>().FindChild("LaserEnd");

                if (Util.HasEffectiveAuthority(gameObject))
                {
                    for (int i = 0; i < ticks; i++)
                    {
                        BulletAttack attack = new()
                        {
                            origin = body.corePosition,
                            damage = totalDamage * equipmentCooldown / ticks,
                            maxDistance = 10000,
                            aimVector = body.inputBank.aimDirection,
                            procChainMask = new(),
                            procCoefficient = procCoefficient / ticks * globalProc,
                            hitEffectPrefab = hitEffectPrefab,
                            radius = 6f,
                            smartCollision = true,
                            owner = body.gameObject,
                            weapon = body.gameObject,
                            isCrit = Util.CheckRoll(body.crit, body.master),
                            force = 700f,
                            falloffModel = BulletAttack.FalloffModel.None
                        };
                        attack.Fire();
                        UpdateLaser();
                        yield return new WaitForSeconds(tickRate);
                    }
                    DestroyLaser();
                }
                shouldFireLaser = true;
                yield return null;
            }
        }
    }
}