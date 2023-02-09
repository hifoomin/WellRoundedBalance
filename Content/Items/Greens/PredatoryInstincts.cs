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
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
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

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (NetworkServer.active)
            {
                var stack = characterBody.inventory.GetItemCount(RoR2Content.Items.AttackSpeedOnCrit);
                characterBody.AddItemBehavior<PredatoryInstinctsController>(stack);
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
    }

    public class PredatoryInstinctsController : CharacterBody.ItemBehavior
    {
        private void Start()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var characterBody = attacker.GetComponent<CharacterBody>();
                if (stack > 0 && characterBody)
                {
                    var vector = characterBody.corePosition - damageInfo.position;
                    ProcType procType = (ProcType)2358697;
                    if ((damageInfo.damageType & DamageType.DoT) != DamageType.DoT && (damageInfo.procChainMask.HasProc(procType) || BackstabManager.IsBackstab(-vector, self.body)))
                    {
                        damageInfo.damageColorIndex = DamageColorIndex.Fragile;
                        damageInfo.procChainMask.AddProc(procType);
                        characterBody.AddTimedBuff(PredatoryInstincts.attackSpeedBuff, 3f, 4 + 2 * (stack - 1));

                        var childLocator = characterBody.modelLocator.modelTransform.GetComponent<ChildLocator>();
                        if (childLocator)
                        {
                            var handL = childLocator.FindChild("HandL");
                            var handR = childLocator.FindChild("HandR");
                            GameObject gameObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/WolfProcEffect");
                            if (handL)
                            {
                                EffectManager.SimpleMuzzleFlash(gameObject, base.gameObject, "HandL", true);
                            }
                            if (handR)
                            {
                                EffectManager.SimpleMuzzleFlash(gameObject, base.gameObject, "HandR", true);
                            }
                        }
                        Util.PlaySound("Play_item_proc_crit_attack_speed2", attacker.gameObject); // Play_item_proc_crit_attack_speed1 Play_item_proc_crit_attack_speed2 Play_item_proc_crit_attack_speed3
                    }
                }
            }

            orig(self, damageInfo);
        }

        public static bool IsBackstab(Vector3 attackerCorePositionToHitPosition, CharacterBody victimBody)
        {
            if (!victimBody.canReceiveBackstab)
            {
                return false;
            }
            Vector3? bodyForward = GetBodyForward(victimBody);
            return bodyForward != null && Vector3.Dot(attackerCorePositionToHitPosition, bodyForward.Value) > 0f;
        }

        private static Vector3? GetBodyForward(CharacterBody characterBody)
        {
            Vector3? vector = null;
            if (characterBody.characterDirection)
            {
                vector = new Vector3?(characterBody.characterDirection.forward);
            }
            else
            {
                vector = new Vector3?(characterBody.transform.forward);
            }
            return vector;
        }

        private static readonly float showBackstabThreshold = Mathf.Cos(0.7853982f);
    }
}