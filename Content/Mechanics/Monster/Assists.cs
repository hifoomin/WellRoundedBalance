namespace WellRoundedBalance.Mechanic.Monster
{
    public class Assists : MechanicBase<Assists>
    {
        public override string Name => ":: Mechanics ::: Assists";

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnCharacterDeath += ProcAssistKills;
            On.RoR2.CharacterBody.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
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
                    foreach (GameObject attacker in controller.attackers)
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
            public List<GameObject> attackers;

            private void Start()
            {
                List<IOnTakeDamageServerReceiver> receivers = hc.onTakeDamageReceivers.ToList();
                receivers.Add(this);
                hc.onTakeDamageReceivers = receivers.ToArray();
                attackers = new();
            }

            public void OnTakeDamageServer(DamageReport report)
            {
                if (report.attacker && !attackers.Contains(report.attacker))
                {
                    attackers.Add(report.attacker);
                }
            }
        }
    }
}