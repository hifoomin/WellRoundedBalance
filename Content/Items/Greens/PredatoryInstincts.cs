using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class PredatoryInstincts : ItemBase
    {
        public static BuffDef attackSpeedBuff;
        public override string Name => ":: Items :: Greens :: Predatory Instincts";
        public override string InternalPickupToken => "attackSpeedOnCrit";

        public override string PickupText => "'Backstabs' increase attack speed up to 3 times.";

        public override string DescText => "<style=cIsDamage>Backstabs</style> increase <style=cIsDamage>attack speed</style> by <style=cIsDamage>12%</style> up to <style=cIsDamage>4</style> <style=cStack>(+2 per stack)</style> times for <style=cIsDamage>3</style> seconds.";
        private static ProcType Backstab = (ProcType)38922;
        public override void Init()
        {
            var attackSpeedOnCritBuffIcon = Utils.Paths.Texture2D.texBuffAttackSpeedOnCritIcon.Load<Texture2D>();

            attackSpeedBuff = ScriptableObject.CreateInstance<BuffDef>();
            attackSpeedBuff.isHidden = false;
            attackSpeedBuff.isDebuff = false;
            attackSpeedBuff.canStack = true;
            attackSpeedBuff.buffColor = new Color32(232, 129, 61, 255);
            attackSpeedBuff.iconSprite = Sprite.Create(attackSpeedOnCritBuffIcon, new Rect(0f, 0f, (float)attackSpeedOnCritBuffIcon.width, (float)attackSpeedOnCritBuffIcon.height), new Vector2(0f, 0f));
            attackSpeedBuff.name = "Predatory Instincts Attack Speed";

            ContentAddition.AddBuffDef(attackSpeedBuff);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += CharacterBody_AddTimedBuff_BuffDef_float;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(11),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5f)))
            {
                c.Index += 4;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Predatory Instincts Deletion 3 hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (inventory)
            {
                var stack = inventory.GetItemCount(RoR2Content.Items.AttackSpeedOnCrit);
                var buffs = sender.GetBuffCount(attackSpeedBuff);
                if (stack > 0)
                {
                    args.baseAttackSpeedAdd += 0.12f * buffs;
                }
            }
        }


        private void CharacterBody_AddTimedBuff_BuffDef_float(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Items), "AttackSpeedOnCrit")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessItem));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Predatory Instincts Deletion 1 hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(1),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(2)))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, 0); // fuck you piece of shit garbage codfe
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Predatory Instincts Deletion 2 hook");
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info) {
            orig(self, info);

            if (!info.attacker || info.procChainMask.HasProc(Backstab)) {
                return;
            }

            CharacterBody attacker = info.attacker.GetComponent<CharacterBody>();

            if (!attacker || info.damageType.HasFlag(DamageType.DoT)) {
                return;
            }

            int stack =  attacker.inventory.GetItemCount(RoR2Content.Items.AttackSpeedOnCrit);

            Vector3 vector = (attacker.corePosition - info.position) * -1;

            if (BackstabManager.IsBackstab(vector, self.body) && stack > 0) {
                info.damageColorIndex = DamageColorIndex.Fragile;
                info.procChainMask.AddProc(Backstab);

                if (NetworkServer.active) {
                    attacker.AddTimedBuff(PredatoryInstincts.attackSpeedBuff, 3f, 4 + 2 * (stack - 1));
                }

                ChildLocator locator = attacker.modelLocator?.modelTransform?.GetComponent<ChildLocator>() ?? null;
                if (locator) {
                    Transform right = locator.FindChild("HandR");
                    Transform left = locator.FindChild("HandL");

                    if (!left || !right) {
                        return;
                    }

                    GameObject vfx = Utils.Paths.GameObject.WolfProcEffect.Load<GameObject>();

                    EffectManager.SimpleMuzzleFlash(vfx, info.attacker, "HandR", true);
                    EffectManager.SimpleMuzzleFlash(vfx, info.attacker, "HandL", true);
                }
            }
        }
    }
}