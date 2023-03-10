using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidWhites
{
    public class LostSeersLenses : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Lost Seers Lenses";
        public override string InternalPickupToken => "critGlassesVoid";

        public override string PickupText => "Gain a 1% chance to instantly kill a non-boss enemy. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";
        public override string DescText => "Your attacks have a <style=cIsDamage>1%</style> <style=cStack>(1% per stack)</style> chance to <style=cIsDamage>instantly kill</style> a <style=cIsDamage>non-Boss enemy</style>. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";

        [ConfigField("Instant Kill Chance", 0.45f)]
        public static float instantKillChance;

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

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.5f),
                    x => x.MatchMul(),
                    x => x.MatchLdarg(1)))
            {
                c.Index += 1;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Lost Seer's Lenses Chance hook");
            }
        }
    }
}