using System;
using EntityStates.DroneWeaponsChainGun;
using RoR2BepInExPack.Utilities;

namespace WellRoundedBalance.Items.Reds
{
    public class SpareDroneParts : ItemBase<SpareDroneParts>
    {
        public override string Name => ":: Items ::: Reds :: Spare Drone Parts";
        public override ItemDef InternalPickup => DLC1Content.Items.DroneWeapons;

        public override string PickupText => "Your drones fire faster, have less cooldowns, shoot missiles, and gain a bonus chaingun.";

        public override string DescText => GetDesc();

        [ConfigField("Chaingun Bounces", 0)]
        public static int chaingunBounces;

        [ConfigField("Chaingun Damage", "Decimal.", 0.3f)]
        public static float chaingunDamage;

        [ConfigField("Attack Speed and Cooldown Reduction Gain", "Decimal.", 0.3f)]
        public static float attackSpeedCdr;
        [ConfigField("Targeting Rework", true)]
        public static bool enableTargetingRework;
        public static BuffDef DroneTargetingBuff;
        public static FixedConditionalWeakTable<GameObject, SDPTargetLocker> TargetTable = new();

        public override void Init()
        {
            base.Init();

            if (enableTargetingRework) {
                var icon = Utils.Paths.Texture2D.texBuffFullCritIcon.Load<Texture2D>();

                DroneTargetingBuff = ScriptableObject.CreateInstance<BuffDef>();
                DroneTargetingBuff.isHidden = false;
                DroneTargetingBuff.isDebuff = true;
                DroneTargetingBuff.canStack = false;
                DroneTargetingBuff.buffColor = Color.red;
                DroneTargetingBuff.iconSprite = Sprite.Create(icon, new Rect(0f, 0f, icon.width, icon.height), new Vector2(0f, 0f));
                DroneTargetingBuff.name = "SDP Target Mark";

                ContentAddition.AddBuffDef(DroneTargetingBuff);
            }
        }

        public string GetDesc() {
            if (enableTargetingRework) {
                return "Gain <style=cIsDamage>Col. Droneman.</style> Drones gain <style=cIsDamage>+" + d(attackSpeedCdr) + "</style> <style=cStack>(+" + d(attackSpeedCdr) + " per stack)</style> attack speed and cooldown reduction, and a <style=cIsDamage>10%</style> chance to fire a <style=cIsDamage>missile</style> on hit, dealing <style=cIsDamage>300%</style> TOTAL damage. The most recently attacked enemy is <style=cDeath>marked</style>, and drones will prioritize the marked enemy and utilize an <style=cIsDamage>automatic chain gun</style> that deals <style=cIsDamage>6x" + d(chaingunDamage) + " damage</style>" + (chaingunBounces > 0 ? " and <style=cIsDamage>bounces</style> up to " + chaingunBounces + " times against them." : " against them.");
            }
            else {
                return "Gain <style=cIsDamage>Col. Droneman.</style> Drones gain <style=cIsDamage>+" + d(attackSpeedCdr) + "</style> <style=cStack>(+" + d(attackSpeedCdr) + " per stack)</style> attack speed and cooldown reduction, a <style=cIsDamage>10%</style> chance to fire a <style=cIsDamage>missile</style> on hit, dealing <style=cIsDamage>300%</style> TOTAL damage and an <style=cIsDamage>automatic chain gun</style> that deals <style=cIsDamage>6x" + d(chaingunDamage) + " damage</style>" + (chaingunBounces > 0 ? " and <style=cIsDamage>bounces</style> up to " + chaingunBounces + " times." : ".");
            }
        }

        public override void Hooks()
        {
            On.EntityStates.DroneWeaponsChainGun.FireChainGun.OnEnter += ChaingunChanges;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

            if (enableTargetingRework) {
                On.EntityStates.DroneWeaponsChainGun.AimChainGun.OnEnter += DWCG_OnEnter;
                On.EntityStates.DroneWeaponsChainGun.AimChainGun.FixedUpdate += DWCG_FixedUpdate;
                RoR2.CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
                IL.RoR2.CharacterModel.UpdateOverlayStates += UpdateOverlay;
                IL.RoR2.CharacterModel.UpdateOverlays += UpdateOverlay;
            }
        }

        private void UpdateOverlay(ILContext il)
        {
            ILCursor c = new(il);

            bool found = c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.FullCrit)));

            if (!found) {
                Logger.LogError("Failed to apply SDP overlay IL Hook");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg, 0);
            c.EmitDelegate<Func<bool, CharacterModel, bool>>((prev, self) => {
                return prev || self.body.HasBuff(DroneTargetingBuff);
            });
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            if (NetworkServer.active) {
                body.AddItemBehavior<SDPTargetingBehaviour>(body.inventory.GetItemCount(DLC1Content.Items.DroneWeapons));
            }
        }

        private void DWCG_OnEnter(On.EntityStates.DroneWeaponsChainGun.AimChainGun.orig_OnEnter orig, EntityStates.DroneWeaponsChainGun.AimChainGun self)
        {
            orig(self);

            if (!TargetTable.ContainsKey(self.gameObject) && self.body && self.body.master && self.body.master.GetComponent<SDPTargetLocker>()) {
                TargetTable.Add(self.gameObject, self.body.master.GetComponent<SDPTargetLocker>());
            }
        }

        private void DWCG_FixedUpdate(On.EntityStates.DroneWeaponsChainGun.AimChainGun.orig_FixedUpdate orig, EntityStates.DroneWeaponsChainGun.AimChainGun self)
        {
            self.fixedAge += self.GetDeltaTime();
            if (!self.isAuthority || self.fixedAge < self.minDuration) {
                return;
            } 

            self.searchRefreshTimer -= self.GetDeltaTime();
            if (self.searchRefreshTimer <= 0f) {
                if (!TargetTable.ContainsKey(self.gameObject) && self.body && self.body.master && self.body.master.GetComponent<SDPTargetLocker>()) {
                    TargetTable.Add(self.gameObject, self.body.master.GetComponent<SDPTargetLocker>());
                }

                self.searchRefreshTimer = self.searchRefreshSeconds;

                if (TargetTable.ContainsKey(self.gameObject)) {
                    SDPTargetLocker locker = TargetTable[self.gameObject];

                    if (locker && locker.owner && locker.owner.primaryMarkedTarget) {
                        self.outer.SetNextState(new FireChainGun(locker.owner.primaryMarkedTarget.mainHurtBox));
                    }
                }
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DroneWeaponsBoostBehavior), nameof(DroneWeaponsBoostBehavior.attackSpeedPerStack))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_R4, attackSpeedCdr);
            }
            else
            {
                Logger.LogError("Failed to apply Spare Drone Parts Attack Speed hook");
            }

            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DroneWeaponsBoostBehavior), nameof(DroneWeaponsBoostBehavior.cooldownReductionPerStack))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_R4, Mathf.Clamp01(1f - attackSpeedCdr));
            }
            else
            {
                Logger.LogError("Failed to apply Spare Drone Parts Cooldown Reduction hook");
            }
        }

        public static void ChaingunChanges(On.EntityStates.DroneWeaponsChainGun.FireChainGun.orig_OnEnter orig, EntityStates.DroneWeaponsChainGun.FireChainGun self)
        {
            self.additionalBounces = chaingunBounces;
            self.damageCoefficient = chaingunDamage;
            orig(self);
        }

        public class SDPTargetingBehaviour : CharacterBody.ItemBehavior {
            public List<CharacterBody> markedTargets = new();
            public CharacterBody primaryMarkedTarget;
            public static List<SDPTargetingBehaviour> instancesList = new();
            private float stopwatch = 0f;
            private void OnServerDamageDealt(DamageReport report)
            {
                if (!NetworkServer.active) return;

                if (report.attackerBody && report.attackerBody == body && report.victimBody) {
                    if (report.victimBody == primaryMarkedTarget) {
                        if (report.victimBody != null) {
                            report.victimBody.AddTimedBuff(SpareDroneParts.DroneTargetingBuff, 5f);
                        }

                        return;
                    }

                    if (report.damageInfo.damageType.IsDamageSourceSkillBased) {
                        foreach (CharacterBody body in markedTargets) {
                            if (body && (body == primaryMarkedTarget || !IsAnyPrimaryTarget(body))) {
                                body.SetBuffCount(SpareDroneParts.DroneTargetingBuff.buffIndex, 0);
                            }
                        }
                        markedTargets.Clear();

                        markedTargets.Add(report.victimBody);
                        primaryMarkedTarget = report.victimBody;
                        primaryMarkedTarget.AddTimedBuff(SpareDroneParts.DroneTargetingBuff, 5f);

                        UpdateAllDrones();
                    }
                }
            }

            public void FixedUpdate() {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= 1f && primaryMarkedTarget != null) {
                    stopwatch = 0f;

                    if (!primaryMarkedTarget.HasBuff(SpareDroneParts.DroneTargetingBuff)) {
                        primaryMarkedTarget = null;
                        return;
                    }
                }
            }

            public void UpdateAllDrones() {
                if (!body || !body.master) return;
                MinionOwnership.MinionGroup group = MinionOwnership.MinionGroup.FindGroup(body.master.netId);

                foreach (MinionOwnership member in group.members) {
                    if (!member) continue;
                    CharacterMaster master = member.GetComponent<CharacterMaster>();

                    if (member.ownerMaster == body.master && master.bodyInstanceObject && master.GetBody().bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical)) {
                        if (!master.GetComponent<SDPTargetLocker>()) {
                            SDPTargetLocker locker = master.gameObject.AddComponent<SDPTargetLocker>();
                            locker.owner = this;
                            locker.ai = master.GetComponent<BaseAI>();
                        }
                    }
                }
            }

            public void OnEnable() {
                GlobalEventManager.onServerDamageDealt += OnServerDamageDealt;
                instancesList.Add(this);
            }

            public void OnDisable() {
                GlobalEventManager.onServerDamageDealt -= OnServerDamageDealt;
                instancesList.Remove(this);
            }

            public static bool IsAnyPrimaryTarget(CharacterBody body) {
                foreach (SDPTargetingBehaviour sdp in instancesList) {
                    if (body == sdp.primaryMarkedTarget) {
                        return true;
                    }
                }

                return false;
            }
        }

        public class SDPTargetLocker : MonoBehaviour {
            public SDPTargetingBehaviour owner;
            public BaseAI ai;
            private float stopwatch = 0f;
            public void FixedUpdate() {
                if (!owner || !ai) return;

                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= 0.25f) {
                    stopwatch = 0f;

                    if (owner.primaryMarkedTarget) {
                        ai.currentEnemy.gameObject = owner.primaryMarkedTarget.gameObject;
                    }
                }
            }
        }
    }
}