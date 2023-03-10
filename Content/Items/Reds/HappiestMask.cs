using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class HappiestMask : ItemBase
    {
        public static BuffDef happiestMaskReady;
        public static BuffDef happiestMaskCooldown;
        public override string Name => ":: Items ::: Reds :: Happiest Mask";
        public override string InternalPickupToken => "ghostOnKill";

        public override string PickupText => "Summon a ghost upon killing an enemy. Recharges over time.";

        public override string DescText => "Killing an enemy <style=cIsDamage>spawns a ghost</style> with <style=cIsDamage>" + d(baseDamage) + "</style> <style=cStack>(+" + d(damagePerStack) + " per stack)</style> damage that lasts <style=cIsDamage>" + lifetime + "s</style>. Recharges every <style=cIsDamage>" + buffCooldown + "s</style>.";

        [ConfigField("Buff Cooldown", 20f)]
        public static float buffCooldown;

        [ConfigField("Lifetime", 30)]
        public static int lifetime;

        [ConfigField("Base Damage", "Decimal.", 12)]
        public static int baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 4)]
        public static int damagePerStack;

        public override void Init()
        {
            happiestMaskReady = ScriptableObject.CreateInstance<BuffDef>();
            happiestMaskReady.isHidden = false;
            happiestMaskReady.canStack = false;
            happiestMaskReady.isDebuff = false;
            happiestMaskReady.buffColor = new Color32();
            happiestMaskReady.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("texBuffHappiestMaskReady.png");
            happiestMaskReady.buffColor = new Color32(227, 228, 227, 255);
            happiestMaskReady.name = "Happiest Mask Ready";

            happiestMaskCooldown = ScriptableObject.CreateInstance<BuffDef>();
            happiestMaskCooldown.isHidden = false;
            happiestMaskCooldown.canStack = true;
            happiestMaskCooldown.isDebuff = false;
            happiestMaskCooldown.isCooldown = true;
            happiestMaskCooldown.buffColor = new Color32();
            happiestMaskCooldown.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("texBuffHappiestMaskCooldown.png");
            happiestMaskCooldown.buffColor = new Color32(255, 255, 255, 255);
            happiestMaskCooldown.name = "Happiest Mask Cooldown";

            ContentAddition.AddBuffDef(happiestMaskReady);
            ContentAddition.AddBuffDef(happiestMaskCooldown);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (NetworkServer.active)
            {
                characterBody.AddItemBehavior<HappiestMaskController>(characterBody.inventory.GetItemCount(RoR2Content.Items.GhostOnKill));
            }
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            var attackerBody = damageReport.attackerBody;
            if (!attackerBody)
            {
                return;
            }

            var inventory = attackerBody.inventory;

            if (!inventory)
            {
                return;
            }

            var victimBody = damageReport.victimBody;

            if (!victimBody)
            {
                return;
            }

            var stack = inventory.GetItemCount(RoR2Content.Items.GhostOnKill);
            if (stack > 0)
            {
                if (!victimBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Masterless) && attackerBody.HasBuff(happiestMaskReady.buffIndex) && !attackerBody.HasBuff(happiestMaskCooldown.buffIndex))
                {
                    var attackerTeam = attackerBody.teamComponent ? attackerBody.teamComponent.teamIndex : TeamIndex.None;
                    var victimTeam = victimBody.teamComponent ? victimBody.teamComponent.teamIndex : TeamIndex.None;

                    if (attackerTeam == TeamIndex.Player || victimTeam != TeamIndex.Player || victimBody.isPlayerControlled)
                    {
                        var happiestMaskController = attackerBody.GetComponent<HappiestMaskController>();
                        if (happiestMaskController && happiestMaskController.CanSpawnGhost())
                        {
                            happiestMaskController.AddGhost(SpawnMaskGhost(victimBody, attackerBody, stack));
                            for (int i = 1; i <= buffCooldown; i++)
                            {
                                attackerBody.AddTimedBuff(happiestMaskCooldown.buffIndex, i);
                            }
                        }
                    }
                }
            }
        }

        public static CharacterBody SpawnMaskGhost(CharacterBody targetBody, CharacterBody ownerBody, int itemCount)
        {
            if (!NetworkServer.active)
            {
                return null;
            }
            if (!targetBody)
            {
                return null;
            }
            var bodyPrefab = BodyCatalog.FindBodyPrefab(targetBody);
            if (!bodyPrefab)
            {
                return null;
            }
            var characterMaster = MasterCatalog.allAiMasters.FirstOrDefault((CharacterMaster master) => master.bodyPrefab == bodyPrefab);
            if (!characterMaster)
            {
                return null;
            }
            MasterSummon masterSummon = new()
            {
                masterPrefab = characterMaster.gameObject,
                ignoreTeamMemberLimit = true,
                position = targetBody.footPosition,
                summonerBodyObject = ownerBody ? ownerBody.gameObject : null,
                inventoryToCopy = targetBody.inventory,
                useAmbientLevel = null
            };
            var characterDirection = targetBody.GetComponent<CharacterDirection>();
            masterSummon.rotation = (characterDirection ? Quaternion.Euler(0f, characterDirection.yaw, 0f) : targetBody.transform.rotation);

            var characterMaster2 = masterSummon.Perform();
            if (!characterMaster2)
            {
                return null;
            }
            else
            {
                var inventory = characterMaster2.inventory;
                if (inventory)
                {
                    if (inventory.GetItemCount(RoR2Content.Items.Ghost) <= 0) inventory.GiveItem(RoR2Content.Items.Ghost);
                    if (inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);

                    if (ownerBody && ownerBody.teamComponent && ownerBody.teamComponent.teamIndex == TeamIndex.Player)
                    {
                        inventory.GiveItem(RoR2Content.Items.BoostDamage.itemIndex, baseDamage + damagePerStack * itemCount);
                        inventory.GiveItem(RoR2Content.Items.HealthDecay.itemIndex, lifetime);
                    }
                }
            }
            var characterBody = characterMaster2.GetBody();
            if (characterBody)
            {
                foreach (EntityStateMachine entityStateMachine in characterBody.GetComponents<EntityStateMachine>())
                {
                    entityStateMachine.initialStateType = entityStateMachine.mainStateType;
                }
            }
            return characterBody;
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld(typeof(RoR2Content.Items), "GhostOnKill")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessItem));
            }
            else
            {
                Logger.LogError("Failed to apply Happiest Mask Deletion hook");
            }
        }
    }

    public class HappiestMaskController : CharacterBody.ItemBehavior
    {
        private List<CharacterBody> activeGhosts;

        private void Awake()
        {
            activeGhosts = new();
        }

        private void FixedUpdate()
        {
            bool hasCooldown = body.HasBuff(HappiestMask.happiestMaskCooldown.buffIndex);
            bool hasReady = body.HasBuff(HappiestMask.happiestMaskReady.buffIndex);
            if (!hasCooldown && !hasReady)
            {
                body.AddBuff(HappiestMask.happiestMaskReady.buffIndex);
            }
            if (hasReady && hasCooldown)
            {
                body.RemoveBuff(HappiestMask.happiestMaskReady.buffIndex);
            }

            UpdateGhosts();
        }

        private void UpdateGhosts()
        {
            List<CharacterBody> toRemove = new();
            foreach (CharacterBody characterBody in activeGhosts)
            {
                if (!(characterBody && characterBody.healthComponent && characterBody.healthComponent.alive))
                {
                    toRemove.Add(characterBody);
                }
            }

            foreach (CharacterBody characterBody in toRemove)
            {
                activeGhosts.Remove(characterBody);
            }
        }

        public bool CanSpawnGhost()
        {
            if (stack <= 0) return false;

            var maxGhosts = 500;
            return activeGhosts.Count < maxGhosts;
        }

        public void AddGhost(CharacterBody characterBody)
        {
            activeGhosts.Add(characterBody);
        }
    }
}