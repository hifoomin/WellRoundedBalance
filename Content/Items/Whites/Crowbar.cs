using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using static R2API.DamageAPI;

namespace WellRoundedBalance.Items.Whites
{
    public class Crowbar : ItemBase<Crowbar>
    {
        public override string Name => ":: Items : Whites :: Crowbar";
        public override ItemDef InternalPickup => RoR2Content.Items.Crowbar;

        public override string PickupText => "Deal bonus damage to enemies " + (firstHit ? "on the first hit" : " above " + d(healthThreshold) + " health") + ".";

        public override string DescText =>
            StackDesc(damageIncrease, damageIncreaseStack, init => $"Deal <style=cIsDamage>{d(init)}</style>{{Stack}} damage to enemies ", d) +
            (firstHit ? "on your first hit" : StackDesc(healthThreshold, healthThresholdStack, init => $"above <style=cIsDamage>{d(init)}{{Stack}} health</style>", d)) + ".";

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
            // IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage1;
        }

        private void HealthComponent_TakeDamage1(ILContext il)
        {
            // untested but uhh yeah I just did it for JavAngle lol
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "Crowbar"),
                        x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                        x => x.MatchStloc(out _), // out _ means you don't care what it matches, and since HealthComponent.TakeDamage only has one crowbar thing, it's good enough, but I like to match a bit more to see what I'm doing
                        x => x.MatchLdloc(out _),
                        x => x.MatchLdcI4(out _)))
            {
                c.Index += 6; // goes after the the int with value of 0 (the one that checks for crowbar amount > 0), we need after because of EmitDelegate
                c.Emit(OpCodes.Ldarg_1); // inserts the second method argument (which is damageInfo, 0 is HealthComponent itself)
                c.EmitDelegate<Func<int, DamageInfo, int>>((orig, self) =>
                {
                    // we get the DamageInfo from the Ldarg_1, otherwise it would've just been int, int for the Func
                    // orig here is the LdcI4, and self is the damageInfo
                    // do custom logic here, like
                    // if (HasModdedDamageType(myModdedDamageType, self)) return int.MaxValue; // we're setting the crowbar damage increase to only work if you have 2147483647 of them AND ONLY if the modded damage type is yours.
                    // else return orig;
                    return orig;
                });
            }
            else
            {
                // log that the hook failed
            }
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