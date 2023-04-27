using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class VoidsentFlame : ItemBase<VoidsentFlame>
    {
        public override string Name => ":: Items :::::: Voids :: Voidsent Flame";
        public override ItemDef InternalPickup => DLC1Content.Items.ExplodeOnDeathVoid;

        public override string PickupText => (firstHit ? "First hit on" : "Full Health") + " enemies also detonate on hit. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";

        public override string DescText => (firstHit ? "On first hit" : "Upon hitting an enemy at <style=cIsDamage>100% health</style>") + "," +
            StackDesc(baseRange, rangePerStack, init => $" <style=cIsDamage>detonate</style> them in a <style=cIsDamage>{m(init)}</style>{{Stack}} radius burst", m) +
            StackDesc(baseDamage, damagePerStack, init => $"for <style=cIsDamage>{d(init)}</style>{{Stack}} base damage", d) + ". <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";

        [ConfigField("Base Damage", "Decimal.", 1.4f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 0.7f)]
        public static float damagePerStack;

        [ConfigField("First Hit", "If enabled, Health Threshold configs will be ignored.", true)]
        public static bool firstHit;

        [ConfigField("Base Range", 12f)]
        public static float baseRange;

        [ConfigField("Range Per Stack", 0f)]
        public static float rangePerStack;

        [ConfigField("Proc Coefficient", 0f)]
        public static float procCoefficient;

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
            Changes();
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);
            int stack = GetItemLoc(c, nameof(DLC1Content.Items.ExplodeOnDeathVoid));
            int m = -1; c.TryGotoPrev(x => x.MatchLdloc(out m));
            if (c.TryGotoPrev(MoveType.After, x => x.MatchCallOrCallvirt<HealthComponent>("get_" + nameof(HealthComponent.fullCombinedHealth))))
            {
                c.Emit(OpCodes.Ldloc, m); // master
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterMaster, HealthComponent, float>>((orig, master, self) =>
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
                    return orig;
                });
            }
            else Logger.LogError("Failed to apply Voidsent Flame Threshold hook");
            if (c.TryGotoNext(x => x.MatchStfld<DelayBlast>(nameof(DelayBlast.radius))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(baseRange, rangePerStack, stack));
            }
            else Logger.LogError("Failed to apply Voidsent Flame Radius hook");
            int bd = -1, dc = -1;
            if (c.TryGotoPrev(x => x.MatchStfld<DelayBlast>(nameof(DelayBlast.baseDamage))) && c.TryGotoPrev(x => x.MatchLdloc(out bd))
                && c.TryGotoPrev(x => x.MatchStloc(bd)) && c.TryGotoPrev(x => x.MatchLdloc(out dc)) && c.TryGotoPrev(x => x.MatchStloc(dc)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(baseDamage, damagePerStack, stack));
            }
            else Logger.LogError("Failed to apply Voidsent Flame Damage hook");
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

        private void Changes()
        {
            var hopooGames = Utils.Paths.GameObject.ExplodeOnDeathVoidExplosion.Load<GameObject>();
            var delayBlast = hopooGames.GetComponent<DelayBlast>();
            delayBlast.procCoefficient = procCoefficient * globalProc;
        }
    }
}