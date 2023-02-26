using System;
using UnityEngine;
using Mono.Cecil;
using MonoMod.Cil;
using System.Linq;

namespace WellRoundedBalance.Items.Reds
{
    public class WakeOfVultures : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Wake Of Vultures";
        public override string InternalPickupToken => GetToken(Utils.Paths.ItemDef.HeadHunter);

        public override string PickupText => "Gain the powers of slain elites. Become resistant to elites.";

        public override string DescText => "Killing an elite <style=cIsUtility>grants you their power</style>. You can have <style=cIsUtility>1</style> <style=cStack>(+1 per stack)</style> affix out at once. Take <style=cIsDamage>20%</style> <style=cStack>(+20% per stack)</style> reduced damage from elites of the same type as you.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += DisableVanilla;
            GlobalEventManager.onCharacterDeathGlobal += Killed;
            On.RoR2.HealthComponent.TakeDamage += ReduceDamage;
        }

        private void ReduceDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damage)
        {
            if (NetworkServer.active)
            {
                if (damage.attacker && damage.attacker.GetComponent<CharacterBody>())
                {
                    CharacterBody attacker = damage.attacker.GetComponent<CharacterBody>();
                    if (attacker.isElite && self.body.inventory && self.body.inventory.GetItemCount(RoR2Content.Items.HeadHunter) > 0)
                    {
                        List<BuffIndex> currentEliteBuffs = new();
                        foreach (BuffIndex buff in attacker.activeBuffsList)
                        {
                            if (BuffCatalog.eliteBuffIndices.Contains(buff))
                            {
                                currentEliteBuffs.Add(buff);
                            }
                        }

                        List<BuffIndex> currentEliteBuffsVictim = new();
                        foreach (BuffIndex buff in self.body.activeBuffsList)
                        {
                            if (BuffCatalog.eliteBuffIndices.Contains(buff))
                            {
                                currentEliteBuffsVictim.Add(buff);
                            }
                        }

                        bool hasAtLeastOne = false;

                        foreach (BuffIndex index in currentEliteBuffsVictim)
                        {
                            if (currentEliteBuffs.Contains(index))
                            {
                                hasAtLeastOne = true;
                                break;
                            }
                        }

                        float mult = Mathf.Pow((1 - 0.2f), self.body.inventory.GetItemCount(RoR2Content.Items.HeadHunter));
                        // Debug.Log(mult);
                        if (hasAtLeastOne)
                        {
                            damage.damage *= mult;
                        }
                    }
                }
            }
            orig(self, damage);
        }

        private void DisableVanilla(ILContext context)
        {
            ILCursor c = new(context);
            bool found = c.TryGotoNext(MoveType.After,
                x => x.MatchLdloc(69),
                x => x.MatchLdcI4(0)
            );

            if (found)
            {
                c.Prev.Operand = int.MaxValue;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Wake of Vultures Deletion hook");
            }
        }

        private void Killed(DamageReport report)
        {
            if (NetworkServer.active)
            {
                if (report.victimIsElite && report.attackerBody)
                {
                    // Debug.Log("killed elite");
                    int stack = report.attackerBody.inventory.GetItemCount(RoR2Content.Items.HeadHunter);
                    if (stack > 0)
                    {
                        List<BuffIndex> currentEliteBuffs = new();
                        foreach (BuffIndex buff in report.attackerBody.activeBuffsList)
                        {
                            if (BuffCatalog.eliteBuffIndices.Contains(buff))
                            {
                                currentEliteBuffs.Add(buff);
                            }
                        }

                        BuffIndex eliteIndex = 0;
                        foreach (BuffIndex buff in report.victimBody.activeBuffsList)
                        {
                            if (BuffCatalog.eliteBuffIndices.Contains(buff) && !currentEliteBuffs.Contains(buff))
                            {
                                // Debug.Log("giving elite buff");
                                eliteIndex = buff;
                                break;
                            }
                        }

                        if (eliteIndex != 0)
                        {
                            report.attackerBody.AddBuff(eliteIndex);
                        }

                        if (currentEliteBuffs.Count > stack)
                        {
                            // Debug.Log("has too many elite buffs");
                            for (int i = 0; i < currentEliteBuffs.Count - stack; i++)
                            {
                                report.attackerBody.RemoveBuff(currentEliteBuffs[i]);
                                // Debug.Log("removing buff");
                            }
                        }
                    }
                }
            }
        }
    }
}