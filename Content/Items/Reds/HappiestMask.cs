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

        public override string PickupText => "Chance on killing an enemy to summon a ghost.";

        public override string DescText => "Killing enemies has a <style=cIsDamage>30%</style> chance to <style=cIsDamage>spawn a ghost</style> of the killed enemy with <style=cIsDamage>1500%</style> damage. Lasts <style=cIsDamage>30s</style> <style=cStack>(+30s per stack)</style>.";

        public override void Init()
        {
            happiestMaskReady = ScriptableObject.CreateInstance<BuffDef>();
            happiestMaskReady.isHidden = false;
            happiestMaskReady.canStack = false;
            happiestMaskReady.isDebuff = false;
            happiestMaskReady.buffColor = new Color32();
            happiestMaskReady.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("texBuffHappiestMaskReady.png");
            happiestMaskReady.buffColor = new Color32(190, 171, 165, 255);
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
                            for (int i = 1; i <= 20; i++)
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
                        inventory.GiveItem(RoR2Content.Items.BoostDamage.itemIndex, 12 + 4 * itemCount);
                        inventory.GiveItem(RoR2Content.Items.HealthDecay.itemIndex, 30);
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
                Main.WRBLogger.LogError("Failed to apply Happiest Mask Deletion hook");
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

            var maxGhosts = 3;
            return activeGhosts.Count < maxGhosts;
        }

        public void AddGhost(CharacterBody characterBody)
        {
            activeGhosts.Add(characterBody);
        }
    }
}