using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.VoidWhites
{
    public class LostSeersLenses : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Lost Seers Lenses";
        public override ItemDef InternalPickup => DLC1Content.Items.CritGlassesVoid;

        public override string PickupText => StackDesc(chance, chanceStack, init => $"Gain a {d(init)} chance to instantly kill a non-boss enemy. ", d) + "<style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";
        public override string DescText => StackDesc(chance, chanceStack, init => $"Your attacks have a <style=cIsDamage>{d(init)}</style>{{Stack}} chance to <style=cIsDamage>instantly kill</style> a <style=cIsDamage>non-Boss enemy</style>. ", d) + "<style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";

        [ConfigField("Chance", "Decimal.", 0.01f)]
        public static float chance;

        [ConfigField("Chance per Stack", "Decimal.", 0.01f)]
        public static float chanceStack;

        [ConfigField("Chance is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 1f)]
        public static float chanceIsHyperbolic;

        [ConfigField("Champion Chance Multiplier", "Decimal.", 0.5f)]
        public static float championMultiplier;

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage2;
        }

        private void HealthComponent_TakeDamage2(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker)
            {
                var characterBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (characterBody && characterBody.inventory)
                {
                    var delete = false;
                    var chance = StackAmount(1f, 1f, characterBody.inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid), 1f);
                    if (!self.body.isBoss && characterBody.inventory && Util.CheckRoll(chance, characterBody.master))
                    {
                        delete = true;
                        var vroggleVFX = HealthComponent.AssetReferences.critGlassesVoidExecuteEffectPrefab; // where did the r come from
                        EffectManager.SpawnEffect(vroggleVFX, new EffectData
                        {
                            origin = self.body.corePosition,
                            scale = (self.body ? self.body.radius : 1f)
                        }, true);
                        damageInfo.damageType |= DamageType.VoidDeath;
                    }
                    if (delete)
                    {
                        if (self.health > 0f) self.Networkhealth = 0f;
                        if (self.shield > 0f) self.Networkshield = 0f;
                        if (self.barrier > 0f) self.Networkbarrier = 0f;
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);
            int inv = -1;
            int info = -1;
            if (c.TryGotoNext(x => x.MatchLdloc(out inv), x => x.MatchCallOrCallvirt<CharacterBody>("get_" + nameof(CharacterBody.inventory)), x => x.MatchLdsfld(typeof(DLC1Content.Items), nameof(DLC1Content.Items.CritGlassesVoid))) 
                && c.TryGotoNext(x => x.MatchLdarg(out info), x => x.MatchLdfld<DamageInfo>(nameof(DamageInfo.procCoefficient)))
                && c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(Util), nameof(Util.CheckRoll))) && c.TryGotoPrev(x => x.MatchLdloc(inv)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg, info);
                c.Emit(OpCodes.Ldloc, inv);
                c.EmitDelegate<Func<HealthComponent, DamageInfo, CharacterBody, float>>((self, info, body) => StackAmount(chance, chanceStack, body.inventory.GetItemCount(InternalPickup), chanceIsHyperbolic) * 100 * (self.body.isChampion ? championMultiplier : 1) * info.procCoefficient);
            }
            else Logger.LogError("Failed to apply Lost Seer's Lenses Chance hook");
        }
    }
}