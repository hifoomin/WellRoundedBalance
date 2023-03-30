using R2API.Utils;

namespace WellRoundedBalance.Mechanics.Bosses
{
    public class OnKill : MechanicBase<OnKill>
    {
        public override string Name => ":: Mechanics ::::: Boss On Kill";
        private List<BodyIndex> acceptableBodies;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.Start += MarkOnSpawn;
            On.RoR2.BodyCatalog.Init += PopulateAcceptableBodies;
        }

        private void MarkOnSpawn(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (!NetworkServer.active)
            {
                return;
            }

            if (acceptableBodies.Contains(self.bodyIndex))
            {
                self.gameObject.AddComponent<OnKillThresholdManager>();
            }
        }

        private void PopulateAcceptableBodies(On.RoR2.BodyCatalog.orig_Init orig)
        {
            orig();

            BodyIndex mithrix = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.BrotherBody.Load<GameObject>());
            BodyIndex mithrix2 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.BrotherHauntBody.Load<GameObject>());
            BodyIndex voidling1 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.VoidRaidCrabBody.Load<GameObject>());
            BodyIndex voidling2 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.MiniVoidRaidCrabBodyBase.Load<GameObject>());
            BodyIndex voidling3 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.MiniVoidRaidCrabBodyPhase1.Load<GameObject>());
            BodyIndex voidling4 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.MiniVoidRaidCrabBodyPhase2.Load<GameObject>());
            BodyIndex voidling5 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.MiniVoidRaidCrabBodyPhase3.Load<GameObject>());
            BodyIndex twisted1 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.ScavLunar1Body.Load<GameObject>());
            BodyIndex twisted2 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.ScavLunar2Body.Load<GameObject>());
            BodyIndex twisted3 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.ScavLunar3Body.Load<GameObject>());
            BodyIndex twisted4 = BodyCatalog.FindBodyIndex(Utils.Paths.GameObject.ScavLunar4Body.Load<GameObject>());

            acceptableBodies = new() {
                mithrix,
                mithrix2,
                voidling1,
                voidling2,
                voidling3,
                voidling4,
                voidling5,
                twisted1,
                twisted2,
                twisted3,
                twisted4
            };
        }

        private class OnKillThresholdManager : MonoBehaviour, IOnTakeDamageServerReceiver
        {
            private HealthComponent hc => base.GetComponent<HealthComponent>();
            private CharacterBody cb => base.GetComponent<CharacterBody>();

            private struct Threshold
            {
                public float fraction;
            }

            private List<Threshold> activeThresholds;

            private void Start()
            {
                activeThresholds = new();
                activeThresholds.Add(new Threshold { fraction = 80 });
                activeThresholds.Add(new Threshold { fraction = 65 });
                activeThresholds.Add(new Threshold { fraction = 50 });
                activeThresholds.Add(new Threshold { fraction = 35 });
                activeThresholds.Add(new Threshold { fraction = 20 });
                activeThresholds.Add(new Threshold { fraction = 5 });
                // reset this since it only checks on hc awake
                hc.onTakeDamageReceivers = base.GetComponents<IOnTakeDamageServerReceiver>();
            }

            public void OnTakeDamageServer(DamageReport report)
            {
                if (report.victimBody == cb && report.attackerBody)
                {
                    CharacterBody attacker = report.attackerBody;
                    List<Threshold> completed = new();

                    foreach (Threshold threshold in activeThresholds)
                    {
                        float value = threshold.fraction * 0.01f;
                        if (hc.health < (hc.fullCombinedHealth * value))
                        {
                            completed.Add(threshold);
                        }
                    }

                    foreach (Threshold threshold in completed)
                    {
                        activeThresholds.Remove(threshold);
                        TriggerKill(report);
                    }
                }
            }

            private void TriggerKill(DamageReport report)
            {
                if (!NetworkServer.active)
                {
                    return;
                }

                if (report.damageInfo.procChainMask.HasProc((ProcType)12096721))
                {
                    return;
                }

                DamageInfo info = new()
                {
                    attacker = report.attacker,
                    crit = report.damageInfo.crit,
                    damage = report.damageDealt,
                    damageType = report.damageInfo.damageType,
                    position = cb.corePosition,
                    damageColorIndex = report.damageInfo.damageColorIndex,
                    procCoefficient = report.damageInfo.procCoefficient
                };

                GlobalEventManager.instance?.OnCharacterDeath(new(info, hc, info.damage, hc.combinedHealth));
            }
        }
    }
}