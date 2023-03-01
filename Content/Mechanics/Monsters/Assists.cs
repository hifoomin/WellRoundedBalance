/*
namespace WellRoundedBalance.Mechanics.Monsters
{
    public class Assists : MechanicBase<Assists>
    {
        public override string Name => ":: Mechanics :::::::: Assists";

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnCharacterDeath += ProcAssistKills;
            On.RoR2.CharacterBody.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && !self.isPlayerControlled)
                {
                    self.gameObject.AddComponent<AssistController>();
                }
            };
        }

        private void ProcAssistKills(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport report)
        {
            if (report.victimBody && report.victimBody.GetComponent<AssistController>())
            {
                AssistController controller = report.victimBody.GetComponent<AssistController>();
                if (controller.attackers.Count >= 2)
                {
                    foreach (AssistController.Attacker attackerStr in controller.attackers)
                    {
                        GameObject attacker = attackerStr.attacker;
                        if (attacker)
                        {
                            report.attacker = attacker;
                            report.attackerBody = attacker.GetComponent<CharacterBody>();
                            report.attackerBodyIndex = attacker.GetComponent<CharacterBody>().bodyIndex;
                            report.attackerTeamIndex = attacker.GetComponent<CharacterBody>().teamComponent.teamIndex;
                            report.attackerMaster = attacker.GetComponent<CharacterBody>().master;
                            if (report.attackerMaster && report.attackerMaster.minionOwnership && report.attackerMaster.minionOwnership.ownerMaster)
                            {
                                report.attackerOwnerMaster = report.attackerMaster.minionOwnership.ownerMaster;
                                if (report.attackerMaster.minionOwnership.ownerMaster.GetBody())
                                {
                                    report.attackerOwnerBodyIndex = report.attackerMaster.minionOwnership.ownerMaster.GetBody().bodyIndex;
                                }
                            }
                        }

                        orig(self, report);
                    }
                }
                else
                {
                    orig(self, report);
                }
            }
            else
            {
                orig(self, report);
            }
        }

        public class AssistController : MonoBehaviour, IOnTakeDamageServerReceiver
        {
            public HealthComponent hc => GetComponent<HealthComponent>();
            public List<Attacker> attackers;

            public class Attacker
            {
                public GameObject attacker;
                public float timeValid;
            }

            private void Start()
            {
                List<IOnTakeDamageServerReceiver> receivers = hc.onTakeDamageReceivers.ToList();
                receivers.Add(this);
                hc.onTakeDamageReceivers = receivers.ToArray();
                attackers = new();
            }

            public void OnTakeDamageServer(DamageReport report)
            {
                if (report.attacker && attackers.Where(x => x.attacker == report.attacker).Count() == 0)
                {
                    attackers.Add(new Attacker
                    {
                        attacker = report.attacker,
                        timeValid = 5
                    });
                }
            }

            public void FixedUpdate()
            {
                attackers.ForEach(x => x.timeValid -= Time.fixedDeltaTime);
                attackers.RemoveAll(x => x.timeValid <= 0);
            }
        }
    }
}
*/

// FIX THIS GUH idk how you would tho