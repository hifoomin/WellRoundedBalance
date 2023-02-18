/*using System;

namespace WellRoundedBalance.Elites {
    internal class Mending : EliteBase
    {
        public override string Name => "Elites :::: Mending";
        public GameObject AffixEarthAttachment;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.AffixEarthBehavior.FixedUpdate += Disable;
            On.RoR2.AffixEarthBehavior.Start += Overwrite;
        }

        private static void Disable(On.RoR2.AffixEarthBehavior.orig_FixedUpdate orig, AffixEarthBehavior self) {
            return;
        }

        private static void Overwrite(On.RoR2.AffixEarthBehavior.orig_Start orig, AffixEarthBehavior self) {
            self.body.gameObject.AddComponent<MendingController>();
            return;
        }

        private class MendingController : MonoBehaviour {
            private TetherVfxOrigin vfxOrigin;
            private float healRate = 1f;
            public HealthComponent target;
            private float stopwatch = 0f;
            private float delay = 1f;
            private TeamIndex team;
            private CharacterBody body;

            private void Start() {
                vfxOrigin = base.gameObject.AddComponent<TetherVfxOrigin>();
                vfxOrigin.tetherPrefab = Utils.Paths.GameObject.AffixEarthTetherVFX.Load<GameObject>();
                team = GetComponent<TeamComponent>().teamIndex;
                body = GetComponent<CharacterBody>();
            }

            private void FixedUpdate() {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= delay) {
                    stopwatch = 0f;
                    target = FetchTarget();
                    if (target && NetworkServer.active) {
                        target.Heal(target.health * (healRate * 0.01f), new(), true);
                    }

                    if (target) {
                        vfxOrigin.SetTetheredTransforms(new() { target.transform });
                    }

                    if (!body.HasBuff(DLC1Content.Buffs.EliteEarth)) {
                        Destroy(vfxOrigin);
                        Destroy(this);
                    }
                }
            }

            private HealthComponent FetchTarget() {
                SphereSearch search = new();
                search.origin = base.transform.position;
                search.radius = 25f;
                search.RefreshCandidates();
                search.FilterCandidatesByDistinctColliderEntities();
                return search.GetHurtBoxes().FirstOrDefault(x => CheckIsValid(x.healthComponent))?.healthComponent ?? null;
            }

            private bool CheckIsValid(HealthComponent com) {
                Debug.Log(com);
                if (com.body == body) {
                    return false;
                }
                if (com.body.teamComponent.teamIndex != team) {
                    return false;
                }
                if (com.body.HasBuff(DLC1Content.Buffs.EliteEarth)) {
                    return false;
                }
                MendingController[] controllers = GameObject.FindObjectsOfType<MendingController>();
                foreach (MendingController controller in controllers) {
                    if (controller != this && controller.target == com) {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}*/