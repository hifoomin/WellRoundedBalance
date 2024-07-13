using WellRoundedBalance.Buffs;
using WellRoundedBalance.Gamemodes.Eclipse;
using System.Collections;

namespace WellRoundedBalance.Elites.Special
{
    internal class Voidtouched : EliteBase<Voidtouched>
    {
        public static BuffDef useless;
        public static BuffDef hiddenCooldown;
        public static GameObject missile;

        public override string Name => ":: Elites :: Voidtouched";

        [ConfigField("Missile Count", "", 5)]
        public static int missileCount;

        [ConfigField("Missile Count Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 7)]
        public static int missileCountE3;

        [ConfigField("Missile Cooldown", "", 5f)]
        public static float missileCooldown;

        [ConfigField("Missile Damage", "Decimal.", 0.75f)]
        public static float missileDamage;

        [ConfigField("Permanent Damage Percent", "Eclipse 8 is 40", 40f)]
        public static float permanentDamagePercent;

        public override void Init()
        {
            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Voidtouched Deletion";
            useless.isHidden = true;

            hiddenCooldown = ScriptableObject.CreateInstance<BuffDef>();
            hiddenCooldown.name = "Voidtouched Missile Cooldown";
            hiddenCooldown.isHidden = true;

            ContentAddition.AddBuffDef(useless);
            ContentAddition.AddBuffDef(hiddenCooldown);

            missile = Utils.Paths.GameObject.MissileProjectile.Load<GameObject>().InstantiateClone("Voidtouched Missile");

            var projectileController = missile.GetComponent<ProjectileController>();

            var newGhost = Utils.Paths.GameObject.MissileVoidBigGhost.Load<GameObject>().InstantiateClone("Voidtouched Missile Ghost", false);
            newGhost.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            projectileController.ghostPrefab = newGhost;

            var missileController = missile.GetComponent<MissileController>();
            missileController.maxVelocity = 30;
            missileController.acceleration = 3f;
            missileController.delayTimer = 0.5f;
            missileController.giveupTimer = 10f;
            missileController.deathTimer = 11f;
            missileController.turbulence = 5.5f;
            missileController.maxSeekDistance = 75f;

            foreach (Component component in missile.GetComponents<Component>())
            {
                if (component.name == "AkEvent")
                {
                    Object.Destroy(component);
                }
            }

            missile.RegisterNetworkPrefab();

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.AffixVoidBehavior.FixedUpdate += AffixVoidBehavior_FixedUpdate;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            var sfp = characterBody.GetComponent<VoidtouchedController>();
            if (characterBody.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                if (sfp == null)
                {
                    characterBody.gameObject.AddComponent<VoidtouchedController>();
                }
            }
            else if (sfp != null)
            {
                characterBody.gameObject.RemoveComponent<VoidtouchedController>();
            }
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
                    int permanentDamage = Mathf.FloorToInt(takenDamagePercent * permanentDamagePercent / 100f * damageInfo.procCoefficient);
                    for (int l = 0; l < permanentDamage; l++)
                    {
                        body.AddBuff(RoR2Content.Buffs.PermanentCurse);
                    }
                }
            }
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

    public class VoidtouchedController : MonoBehaviour
    {
        public CharacterBody body;
        public GameObject prefab;

        public void Start()
        {
            body = GetComponent<CharacterBody>();
            body.onSkillActivatedAuthority += Body_onSkillActivatedAuthority;
        }

        public void Body_onSkillActivatedAuthority(GenericSkill skill)
        {
            if (!NetworkServer.active || body.HasBuff(Voidtouched.hiddenCooldown) || !body.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                return;
            }
            StartCoroutine(FireMissiles());
        }

        public IEnumerator FireMissiles()
        {
            body.AddTimedBuff(Voidtouched.hiddenCooldown, Voidtouched.missileCooldown);

            var startPos = body.corePosition + Vector3.up * 5f;

            for (int i = 0; i < (Eclipse3.CheckEclipse() ? Voidtouched.missileCount : Voidtouched.missileCountE3); i++)
            {
                var j = i % 2 == 0 ? i : -i;
                var randomPos = new Vector3(j, j, j);
                if (Util.HasEffectiveAuthority(body.gameObject))
                {
                    FireProjectileInfo info = new()
                    {
                        damage = body.damage * Voidtouched.missileDamage,
                        crit = false,
                        position = startPos + randomPos,
                        projectilePrefab = Voidtouched.missile,
                        owner = body.gameObject,
                    };
                    ProjectileManager.instance.FireProjectile(info);
                    yield return new WaitForSeconds(0.2f);
                }
            }

            yield return null;
        }
    }
}