using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class Crowbar : ItemBase<Crowbar>
    {
        public override string Name => ":: Items : Whites :: Crowbar";
        public override ItemDef InternalPickup => RoR2Content.Items.Crowbar;

        public override string PickupText => "Deal bonus damage to enemies " + (firstHit ? "on the first hit" : " above " + d(healthThreshold) + " health") + ".";

        public override string DescText =>
            StackDesc(damageIncrease, damageIncreaseStack, init => $"Deal <style=cIsDamage>{d(init)}</style>{{Stack}} damage to enemies ", d) +
            (firstHit ? "on the first hit" : StackDesc(healthThreshold, healthThresholdStack, init => $"above <style=cIsDamage>{d(init)}{{Stack}} health</style>", d)) + ".";

        [ConfigField("Damage Increase", "Decimal.", 0.4f)]
        public static float damageIncrease;

        [ConfigField("Damage Increase per Stack", "Decimal.", 0.4f)]
        public static float damageIncreaseStack;

        [ConfigField("Damage Increase is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float damageIncreaseIsHyperbolic;

        [ConfigField("First Hit", "If enabled, Health Threshold configs will be ignored.", true)]
        public static bool firstHit;

        [ConfigField("Health Threshold", "Decimal.", 0.85f)]
        public static float healthThreshold;

        [ConfigField("Health Threshold per Stack", "Decimal.", 0f)]
        public static float healthThresholdStack;

        [ConfigField("Health Threshold is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float healthThresholdIsHyperbolic;

        public static Dictionary<CharacterBody, List<CharacterBody>> db = new();

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Stage.onStageStartGlobal += _ => db.Clear();
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }

        public void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);
            int info = -1; int dmg = -1;
            c.TryGotoNext(x => x.MatchLdarg(out info), x => x.MatchLdfld<DamageInfo>(nameof(DamageInfo.damage)), x => x.MatchStloc(out dmg));
            int stack = GetItemLoc(c, nameof(RoR2Content.Items.Crowbar));
            int m = -1; c.TryGotoPrev(x => x.MatchLdloc(out m));
            if (dmg == -1 || stack == -1) return;
            if (c.TryGotoPrev(x => x.MatchCallOrCallvirt<HealthComponent>("get_" + nameof(HealthComponent.fullCombinedHealth))) && c.TryGotoNext(MoveType.After, x => x.MatchMul()))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, m);
                c.EmitDelegate<Func<HealthComponent, CharacterMaster, float>>((self, master) =>
                {
                    if (firstHit)
                    {
                        CharacterBody from = master.GetBody();
                        CharacterBody to = self.body;
                        if (from && to)
                        {
                            if (!db.ContainsKey(from)) db.Add(from, new());
                            if (db[from].Contains(to)) return float.MaxValue;
                            db[from].Add(to);
                            return 0;
                        }
                    }
                    return self.fullCombinedHealth * StackAmount(healthThreshold, healthThresholdStack, master.inventory.GetItemCount(InternalPickup), healthThresholdIsHyperbolic);
                });
            }
            else Logger.LogError("Failed to apply Crowbar Threshold hook");
            if (c.TryGotoNext(x => x.MatchStloc(dmg)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, dmg);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<float, int, float>>((orig, stack) => orig * (1f + StackAmount(damageIncrease, damageIncreaseStack, stack, damageIncreaseIsHyperbolic)));
            }
            else Logger.LogError("Failed to apply Crowbar Damage hook");
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            if (damageReport.victimBody)
            {
                db.Remove(damageReport.victimBody);
                foreach (var k in db.Keys) db[k].Remove(damageReport.victimBody);
            }
        }
    }
}