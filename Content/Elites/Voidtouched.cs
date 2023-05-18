using Unity.Collections.LowLevel.Unsafe;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Equipment.Orange;
using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Voidtouched : EliteBase<Voidtouched>
    {
        public static BuffDef useless;
        public static BuffDef hiddenCooldown;
        public static GameObject missile;

        public override string Name => ":: Elites :: Voidtouched";

        [ConfigField("Missile Count", "", 5)]
        public static int missileCount;

        [ConfigField("Missile Count Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 8)]
        public static int missileCountE3;

        [ConfigField("Missile Cooldown", "", 2f)]
        public static float missileCooldown;

        [ConfigField("Missile Damage", "Decimal.", 1f)]
        public static float missileDamage;

        [ConfigField("Permanent Damage Percent", "Eclipse 8 is 40", 40f)]
        public static float permanentDamagePercent;

        public override void Init()
        {
            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Voidtouched Deletion";
            useless.isHidden = true;

            hiddenCooldown = ScriptableObject.CreateInstance<BuffDef>();
            hiddenCooldown.name = "Voidtouched Spike Cooldown";
            hiddenCooldown.isHidden = true;

            ContentAddition.AddBuffDef(useless);
            ContentAddition.AddBuffDef(hiddenCooldown);

            missile = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MissileProjectile.Load<GameObject>(), "Voidtouched Missile");

            var projectileController = missile.GetComponent<ProjectileController>();

            var newGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MissileVoidBigGhost.Load<GameObject>(), "Voidtouched Missile Ghost", false);
            newGhost.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);

            projectileController.ghostPrefab = newGhost;

            var missileController = missile.GetComponent<MissileController>();
            missileController.maxVelocity = 20;
            missileController.acceleration = 1.5f;
            missileController.delayTimer = 0.5f;
            missileController.giveupTimer = 10f;
            missileController.deathTimer = 20f;
            missileController.turbulence = 0f;

            foreach (Component component in missile.GetComponents<Component>())
            {
                if (component.name == "AkEvent")
                {
                    Object.Destroy(component);
                }
            }

            PrefabAPI.RegisterNetworkPrefab(missile);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.AffixVoidBehavior.FixedUpdate += AffixVoidBehavior_FixedUpdate;
            On.RoR2.CharacterBody.OnSkillActivated += OnSkillActivated;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            var body = self.body;
            var attacker = damageInfo.attacker;
            if (body && attacker)
            {
                var attackerBody = attacker.GetComponent<CharacterBody>();
                if (attackerBody && attackerBody.HasBuff(DLC1Content.Buffs.EliteVoid))
                {
                    float takenDamagePercent = damageInfo.damage / self.fullCombinedHealth * 100f;
                    int permanentDamage = Mathf.FloorToInt((takenDamagePercent * permanentDamagePercent / 100f) * damageInfo.procCoefficient);
                    for (int l = 0; l < permanentDamage; l++)
                    {
                        body.AddBuff(RoR2Content.Buffs.PermanentCurse);
                    }
                }
            }
        }

        private void OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody body, GenericSkill slot)
        {
            orig(body, slot);
            if (!NetworkServer.active || body.HasBuff(hiddenCooldown) || !body.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                return;
            }

            var startPos = body.corePosition + Vector3.up * 5f;

            for (int i = 0; i < (Eclipse3.CheckEclipse() ? missileCount : missileCountE3); i++)
            {
                if (Util.HasEffectiveAuthority(body.gameObject))
                {
                    FireProjectileInfo info = new()
                    {
                        damage = body.damage * missileDamage,
                        damageTypeOverride = DamageType.Nullify,
                        crit = false,
                        position = startPos,
                        projectilePrefab = missile,
                        owner = body.gameObject,
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }
            }
            body.AddTimedBuff(hiddenCooldown, missileCooldown);
        }

        private void AffixVoidBehavior_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "BearVoidReady"),
                x => x.MatchCallOrCallvirt<CharacterBody>("AddBuff")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.voidtouchedSaferSpaces));
            }
            else
            {
                Logger.LogError("Failed to apply Voidtouched Elite Safer Spaces hook");
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "EliteVoid")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Voidtouched Elite Needletick hook");
            }
        }
    }
}