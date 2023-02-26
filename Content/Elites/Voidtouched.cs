using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Audio;
using System.Collections;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Voidtouched : EliteBase
    {
        public static BuffDef useless;
        public static BuffDef voidCurse;

        public override string Name => ":: Elites :::::: Voidtouched";

        public override void Init()
        {
            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Voidtouched Deletion";
            useless.isHidden = true;

            var curse = Utils.Paths.Texture2D.texBuffPermanentCurse.Load<Texture2D>();
            voidCurse = ScriptableObject.CreateInstance<BuffDef>();
            voidCurse.isHidden = false;
            voidCurse.canStack = true;
            voidCurse.iconSprite = Sprite.Create(curse, new Rect(0, 0, (float)curse.width, (float)curse.height), new Vector2(0f, 0f));
            voidCurse.buffColor = new Color32(255, 75, 74, 255);
            voidCurse.isDebuff = true;

            voidCurse.name = "Void Curse";

            ContentAddition.AddBuffDef(useless);
            ContentAddition.AddBuffDef(voidCurse);

            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.AffixVoidBehavior.FixedUpdate += AffixVoidBehavior_FixedUpdate;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy1;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
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

        private void GlobalEventManager_OnHitEnemy1(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim)
        {
            orig(self, info, victim);

            if (NetworkServer.active && info.attacker)
            {
                CharacterBody attackerBody = info.attacker.GetComponent<CharacterBody>();
                CharacterBody victimBody = info.attacker.GetComponent<CharacterBody>();

                if (attackerBody.HasBuff(DLC1Content.Buffs.EliteVoid))
                {
                    float takenDamagePercent = (info.damage / victimBody.healthComponent.fullCombinedHealth * 100f) * info.procCoefficient;
                    int permanentDamage = Mathf.FloorToInt(takenDamagePercent * 50 / 100f);
                    for (int l = 0; l < permanentDamage; l++)
                    {
                        victimBody.AddBuff(RoR2Content.Buffs.PermanentCurse);
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
                Main.WRBLogger.LogError("Failed to apply Voidtouched Elite Safer Spaces hook");
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
                Main.WRBLogger.LogError("Failed to apply Voidtouched Elite Needletick hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                var curses = sender.GetBuffCount(voidCurse);
                if (curses > 0)
                {
                    args.baseCurseAdd += curses * 0.01f;
                }
            }
        }
    }

    public class VoidtouchedBallsManager : MonoBehaviour
    {
        public CharacterBody body;
        public GameObject prefab = Projectiles.VoidBall.prefab;
        private float timer;
        public float interval = 5f;
        public int ballCount;

        private void Start()
        {
            body = GetComponent<CharacterBody>();
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled)
            {
                ballCount = 5;
            }
            else
            {
                ballCount = 4;
            }
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= interval)
            {
                var randomDelay = Run.instance.nextStageRng.RangeFloat(0.12f, 0.4f);
                StartCoroutine(SummonBalls(ballCount, randomDelay));
                timer = 0;
            }
        }

        private IEnumerator SummonBalls(int count, float delay)
        {
            var playerList = CharacterBody.readOnlyInstancesList.Where(x => x.isPlayerControlled).ToArray();
            for (int i = 0; i < playerList.Length; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (Util.HasEffectiveAuthority(gameObject) && Extensions.CheckLoS(playerList[i].transform.position, gameObject.transform.position, 120f))
                    {
                        var fpi = new FireProjectileInfo
                        {
                            projectilePrefab = prefab,
                            damage = Run.instance ? 5f + Mathf.Sqrt(Run.instance.ambientLevel * 200f) / Mathf.Sqrt(Run.instance.participatingPlayerCount) : 0f,
                            rotation = Quaternion.identity,
                            owner = gameObject,
                            crit = body.RollCrit(),
                            //position = Util.ApplySpread(playerList[i].footPosition, 7f * j, 10f * j, 1f, 0.08f)
                            position = new Vector3(playerList[i].footPosition.x + Run.instance.treasureRng.RangeFloat(-15f * j, 15f * j), playerList[i].footPosition.y, playerList[i].footPosition.z + Run.instance.runRNG.RangeFloat(-15f * j, 15f * j))
                        };
                        ProjectileManager.instance.FireProjectile(fpi);
                    }

                    yield return new WaitForSeconds(delay);
                }
            }
        }
    }
}