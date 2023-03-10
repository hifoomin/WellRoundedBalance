using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Voidtouched : EliteBase
    {
        public static BuffDef useless;

        public override string Name => ":: Elites :::::: Voidtouched";

        public override void Init()
        {
            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Voidtouched Deletion";
            useless.isHidden = true;

            ContentAddition.AddBuffDef(useless);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.AffixVoidBehavior.FixedUpdate += AffixVoidBehavior_FixedUpdate;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            On.RoR2.HealthComponent.Awake += HealthComponent_Awake;
        }

        private void HealthComponent_Awake(On.RoR2.HealthComponent.orig_Awake orig, HealthComponent self)
        {
            self.gameObject.AddComponent<VoidtouchedPermanentDamage>();
            orig(self);
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            var vbm = characterBody.GetComponent<VoidtouchedBallsManager>();
            if (characterBody.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                if (vbm == null)
                {
                    characterBody.gameObject.AddComponent<VoidtouchedBallsManager>();
                }
            }
            else if (vbm != null)
            {
                characterBody.gameObject.RemoveComponent<VoidtouchedBallsManager>();
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

    public class VoidtouchedBallsManager : MonoBehaviour
    {
        public CharacterBody body;
        public GameObject prefab = Projectiles.VoidBall.prefab;
        public int ballCount;

        private void Start()
        {
            body = GetComponent<CharacterBody>();

            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled)
            {
                ballCount = 4;
            }
            else
            {
                ballCount = 3;
            }

            if (body)
            {
                body.onSkillActivatedServer += Body_onSkillActivatedServer;
            }
        }

        private void Body_onSkillActivatedServer(GenericSkill genericSkill)
        {
            StartCoroutine(SummonBalls());
        }

        public Vector3 RaycastToFloor(Vector3 position)
        {
            Ray ray = new(position, Vector3.down);
            var maxDistance = 15f;
            LayerIndex world = LayerIndex.world;
            if (Physics.Raycast(ray, out RaycastHit raycastHit, maxDistance, world.mask, QueryTriggerInteraction.Ignore))
            {
                return raycastHit.point;
            }
            else
            {
                return Vector3.one;
            }
        }

        public IEnumerator SummonBalls()
        {
            if (Util.HasEffectiveAuthority(gameObject))
            {
                for (int i = 0; i < ballCount; i++)
                {
                    var fpi = new FireProjectileInfo
                    {
                        projectilePrefab = prefab,
                        damage = Run.instance ? 5f + Mathf.Sqrt(Run.instance.ambientLevel * 200f) / Mathf.Sqrt(Run.instance.participatingPlayerCount) : 0f,
                        rotation = Quaternion.identity,
                        owner = gameObject,
                        crit = body.RollCrit(),
                        position = RaycastToFloor(gameObject.transform.position + body.equipmentSlot.GetAimRay().GetPoint(i + 2))
                    };
                    ProjectileManager.instance.FireProjectile(fpi);
                }
            }
            yield return null;
        }
    }

    public class VoidtouchedPermanentDamage : MonoBehaviour, IOnTakeDamageServerReceiver
    {
        public HealthComponent hc;
        public CharacterBody body;

        public void Start()
        {
            hc = GetComponent<HealthComponent>();
            if (!hc)
            {
                Destroy(this);
                return;
            }
            body = hc.body;
        }

        public void OnTakeDamageServer(DamageReport damageReport)
        {
            if (body && damageReport.attacker && damageReport.attackerBody && damageReport.attackerBody.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                switch (body.teamComponent.teamIndex)
                {
                    case TeamIndex.Player:
                        {
                            float takenDamagePercent = damageReport.damageDealt / hc.fullCombinedHealth * 100f;
                            int permanentDamage = Mathf.FloorToInt((takenDamagePercent * 30f / 100f) * damageReport.damageInfo.procCoefficient);
                            for (int l = 0; l < permanentDamage; l++)
                            {
                                body.AddBuff(RoR2Content.Buffs.PermanentCurse);
                            }
                        }
                        break;
                }
            }
        }
    }
}