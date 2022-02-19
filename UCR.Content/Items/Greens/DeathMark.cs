using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun
{
    public class DeathMark : ItemBase
    {
        // ////////////
        // 
        // Thanks to Skell
        //
        // ///////////////

        public static bool enable;
        public static float dmgperdebuff;
        public static float dmgperstack;
        public static int mindebuffs;

        public override string Name => ":: Items :: Greens :: Death Mark";
        public override string InternalPickupToken => "deathmark";
        public override bool NewPickup => true;
        public override string PickupText => enable ? " Enemies with " + mindebuffs + " or more debuffs are marked for death, taking bonus damage." : "Enemies with 4 or more debuffs are marked for death, taking bonus damage.";

        float actualStack = dmgperdebuff * dmgperstack;

        public override string DescText => enable ? "Enemies with <style=cIsDamage>" + mindebuffs + "</style> or more debuffs are <style=cIsDamage>marked for death</style>, increasing damage taken by <style=cIsDamage>" + d(dmgperstack) + "</style> <style=cStack>(+" + d(actualStack) + " per stack)</style> per debuff from all sources for <style=cIsUtility>7</style> <style=cStack>(+7 per stack)</style> seconds." : "Enemies with <style=cIsDamage>4</style> or more debuffs are <style=cIsDamage>marked for death</style>, increasing damage taken by <style=cIsDamage>50%</style> from all sources for <style=cIsUtility>7</style> <style=cStack>(+7 per stack)</style> seconds.";


        public override void Init()
        {
            enable = ConfigOption(false, "Enable Changes?", "Vanilla is false");
            dmgperdebuff = ConfigOption(0.1f, "Debuff Damage Increase", "Decimal. Per Debuff. Vanilla is 0");
            dmgperstack = ConfigOption(0.05f, "Stack Debuff Damage Increase", "Decimal. Per Stack. Vanilla is 0");
            mindebuffs = ConfigOption(2, "Minimum Debuffs", "Vanilla is 4");
            base.Init();
        }

        public override void Hooks()
        {
            if (enable)
            {
                IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDebuffsReq;
                IL.RoR2.HealthComponent.TakeDamage += Changes;
            }
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(6),
                x => x.MatchLdcR4(1.5f),
                x => x.MatchMul(),
                x => x.MatchStloc(6),
                x => x.MatchLdarg(1),
                x => x.MatchLdcI4(7),
                x => x.MatchStfld<DamageInfo>("damageColorIndex")
                );
            c.Index += 3;
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc_1);
            c.EmitDelegate<Func<HealthComponent, CharacterBody, float>>((self, attacker) =>
            {
                if (attacker.master.inventory)
                {
                    int DeathMarkCount = Util.GetItemCountForTeam(attacker.master.teamIndex, RoR2Content.Items.DeathMark.itemIndex, false, true);
                    int debuffCount = 0;
                    foreach (BuffIndex buffType in BuffCatalog.debuffBuffIndices)
                    {
                        if (self.body.HasBuff(buffType))
                        {
                            debuffCount++;
                        }
                    }
                    DotController dotController = DotController.FindDotController(self.gameObject);
                    if (dotController)
                    {
                        for (DotController.DotIndex dotIndex = DotController.DotIndex.Bleed; dotIndex < DotController.DotIndex.Count; dotIndex++)
                        {
                            if (dotController.HasDotActive(dotIndex))
                            {
                                debuffCount++;
                            }
                        }
                    }
                    float damageBonus = debuffCount * dmgperdebuff;
                    if (DeathMarkCount > 0)
                    {
                        return 1f + damageBonus + (dmgperstack * damageBonus * ((float)DeathMarkCount - 1f));
                    }
                    return 1f + damageBonus;
                }
                return 1.5f;
            });
        }

        public static void ChangeDebuffsReq(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdloc(16),
                x => x.MatchLdcI4(4),
                x => x.MatchBlt(out ILLabel IL_0BD7)
                );
            c.Index += 2;
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4, mindebuffs);
        }
    }
}
