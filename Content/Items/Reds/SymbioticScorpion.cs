using MonoMod.Cil;
using System.Data;
using UnityEngine.UIElements;
using static RoR2.DotController;

namespace WellRoundedBalance.Items.Reds
{
    public class SymbioticScorpion : ItemBase
    {
        public static BuffDef venom;
        public static BuffDef armorReduction;
        public static BuffDef armorGain;
        public override string Name => ":: Items ::: Reds :: Symbiotic Scorpion";
        public override string InternalPickupToken => "permanentDebuffOnHit";

        public override string PickupText => "Inflict venom on hit.";

        public override string DescText => "Inflict <style=cIsDamage>venom</style> on hit, dealing <style=cIsDamage>750%</style> <style=cStack>(+100% per stack)</style> base damage over 5s and stealing <style=cIsHealing>20</style> <style=cStack>(+10 per stack)</style> <style=cIsHealing>armor</style> for 5s.";

        public override void Init()
        {
            var scorpion = Utils.Paths.Texture2D.texBuffPermanentDebuffIcon.Load<Texture2D>();
            var armor = Utils.Paths.Texture2D.texBuffBodyArmorIcon.Load<Texture2D>();

            venom = ScriptableObject.CreateInstance<BuffDef>();
            venom.isHidden = false;
            venom.canStack = false;
            venom.isDebuff = true;
            venom.iconSprite = Sprite.Create(scorpion, new Rect(0f, 0f, (float)scorpion.width, (float)scorpion.height), new Vector2(0f, 0f));
            venom.buffColor = new Color32(187, 255, 0, 255);
            venom.name = "Symbiotic Scorpion Venom";

            armorReduction = ScriptableObject.CreateInstance<BuffDef>();
            armorReduction.isHidden = true;
            armorReduction.isDebuff = true;
            armorReduction.canStack = false;

            armorGain = ScriptableObject.CreateInstance<BuffDef>();
            armorGain.isHidden = false;
            armorGain.canStack = false;
            armorGain.isDebuff = false;
            armorGain.iconSprite = Sprite.Create(armor, new Rect(0f, 0f, (float)armor.width, (float)armor.height), new Vector2(0f, 0f));
            armorGain.buffColor = new Color32(216, 116, 132, 255);
            armorGain.name = "Symbiotic Scorpion Armor";

            ContentAddition.AddBuffDef(venom);
            ContentAddition.AddBuffDef(armorReduction);
            ContentAddition.AddBuffDef(armorGain);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeChance;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            Changes();
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var stack = sender.inventory ? sender.inventory.GetItemCount(DLC1Content.Items.PermanentDebuffOnHit) : 0;
            if (sender.HasBuff(armorReduction))
            {
                if (sender.armor - (20 + 10 * (stack - 1)) >= 0)
                {
                    args.armorAdd -= 20 + 10 * (stack - 1);
                }
            }
            args.armorAdd += sender.GetBuffCount(armorGain) * (20 + 10 * (stack - 1));
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            var attacker = damageReport.attacker;
            var victim = damageReport.victim;
            if (attacker && victim)
            {
                var attackerBody = damageReport.attackerBody;
                var victimBody = damageReport.victimBody;
                if (attackerBody && victimBody)
                {
                    var based = victimBody.gameObject.GetComponent<FuckDoTAPIFuckDelegatesFuckComplicatedShit>();
                    if (based)
                    {
                        victimBody.gameObject.GetComponent<FuckDoTAPIFuckDelegatesFuckComplicatedShit>().attackerBody = attackerBody;
                    }
                    var inventory = attackerBody.inventory;
                    if (inventory)
                    {
                        var stack = inventory.GetItemCount(DLC1Content.Items.PermanentDebuffOnHit);
                        if (stack > 0 && damageReport.damageInfo.procCoefficient > 0)
                        {
                            victimBody.AddTimedBuff(armorReduction, 5f);
                            attackerBody.AddTimedBuff(armorGain, 5f);
                            if (!based)
                            {
                                victimBody.gameObject.AddComponent<FuckDoTAPIFuckDelegatesFuckComplicatedShit>();
                            }
                        }
                    }
                }
            }
        }

        private void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld("RoR2.DLC1Content/Items", "PermanentDebuffOnHit"),
                    x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                    x => x.MatchStloc(out _),
                    x => x.MatchLdcI4(0),
                    x => x.MatchStloc(out _),
                    x => x.MatchLdcI4(0),
                    x => x.MatchStloc(out _),
                    x => x.MatchBr(out _),
                    x => x.MatchLdcR4(100f)))
            {
                c.Index += 8;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Symbiotic Scorpion Chance hook");
            }
        }

        private void Changes()
        {
            var scorpion = Utils.Paths.ItemDef.PermanentDebuffOnHit.Load<ItemDef>();
            scorpion.tags = new ItemTag[] { ItemTag.Damage, ItemTag.Damage, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist };
        }
    }

    public class FuckDoTAPIFuckDelegatesFuckComplicatedShit : MonoBehaviour
    {
        public float interval = 0.2f;
        public float timer;
        public float damageCoefficient = 0.3f;
        public float duration = 5f;
        public HealthComponent victimHealthComponent;
        public CharacterBody attackerBody;
        public DamageInfo info;
        // ticks 5 times a second for 30% damage per tick for 5 seconds in total

        private void Start()
        {
            attackerBody = GetComponent<CharacterBody>();
            victimHealthComponent = attackerBody.GetComponent<HealthComponent>();
            var inventory = attackerBody.inventory;
            if (inventory)
            {
                damageCoefficient = 0.3f + 0.04f * (inventory.GetItemCount(DLC1Content.Items.PermanentDebuffOnHit) - 1);
            }
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            duration -= Time.fixedDeltaTime;
            if (timer >= interval && duration >= 0f)
            {
                if (victimHealthComponent)
                {
                    info = new()
                    {
                        attacker = gameObject,
                        crit = false,
                        damage = damageCoefficient * attackerBody.damage,
                        damageColorIndex = DamageColorIndex.Poison,
                        force = Vector3.zero,
                        procCoefficient = 0f,
                        damageType = DamageType.Generic,
                        position = attackerBody.corePosition,
                        dotIndex = DotIndex.None,
                        inflictor = gameObject
                    };
                    victimHealthComponent.TakeDamage(info);
                }
                timer = 0f;
            }

            if (duration < 0f)
            {
                Destroy(this);
            }
        }
    }
}