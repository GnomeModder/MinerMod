using EntityStates;
using RoR2;
using UnityEngine;
using System.Collections.Generic;

namespace Gnome.EntityStatez
{
    public class DrillBreakState : BaseSkillState
    {
        private float duration;
        public GameObject slashPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXSlash");
        public GameObject largePrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLarge");

        BlastAttack blastAttack;
        BlastAttack checkAttack;
        //List<CharacterBody> victimBodyList = new List<CharacterBody>();
        //int consume = 0;
        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = base.GetAimRay();
            this.duration = 0.5f;

            blastAttack = new BlastAttack
            {
                radius = 13f,
                procCoefficient = 1,
                position = aimRay.origin,
                attacker = base.gameObject,
                crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                baseDamage = base.characterBody.baseDamage * 4,
                falloffModel = BlastAttack.FalloffModel.SweetSpot,
                baseForce = 3f,
                damageType = DamageType.Generic,
                attackerFiltering = AttackerFiltering.NeverHit,
            };
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);

            checkAttack = new BlastAttack
            {
                radius = 2.5f,
                procCoefficient = 1,
                position = aimRay.origin,
                attacker = base.gameObject,
                crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                baseDamage = 0.1f,
                falloffModel = BlastAttack.FalloffModel.None,
                baseForce = 3f,
                damageType = DamageType.Generic,
                attackerFiltering = AttackerFiltering.NeverHit,
            };
            checkAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);

            if (base.isAuthority)
            {
                base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

                Util.PlaySound("DrillCharge", base.gameObject);

                base.characterMotor.velocity += 75 * aimRay.direction;
            }
        }
        public override void OnExit()
        {
            base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
            base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, StaticValues.lingeringInvincibilityDuration * 0.75f);

            base.characterMotor.velocity.x *= 0.5f;
            base.characterMotor.velocity.y *= 0.5f;
            base.characterMotor.velocity.z *= 0.5f;

            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Ray aimRay = base.GetAimRay();
            checkAttack.position = aimRay.origin;
            BlastAttack.Result result = checkAttack.Fire();

            if (result.hitCount > 0 && base.isAuthority)
            {
                blastAttack.position = aimRay.origin;
                blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.scale = 6;
                effectData.origin = aimRay.origin;
                EffectManager.SpawnEffect(largePrefab, effectData, false);

                Util.PlaySound("CrackHammer", base.gameObject);

                base.characterMotor.velocity.x *= 0.5f;
                base.characterMotor.velocity.y *= 0.5f;
                base.characterMotor.velocity.z *= 0.5f;

                this.outer.SetNextStateToMain();
                return;
            }
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        //private void getHitList(BlastAttack ba)
        //{
        //    Collider[] array = Physics.OverlapSphere(ba.position, ba.radius, LayerIndex.defaultLayer.mask);
        //    int num = 0;
        //    int num2 = 0;
        //    while (num < array.Length && num2 < 12)
        //    {
        //        HealthComponent component = array[num].GetComponent<HealthComponent>();
        //        if (component)
        //        {
        //            TeamComponent component2 = component.GetComponent<TeamComponent>();
        //            if (component2.teamIndex != TeamIndex.Player)
        //            {
        //                this.AddToList(component.gameObject);
        //                num2++;
        //            }
        //        }
        //        num++;
        //    }
        //}

        //private void AddToList(GameObject affectedObject)
        //{
        //    CharacterBody component = affectedObject.GetComponent<CharacterBody>();
        //    if (!this.victimBodyList.Contains(component))
        //    {
        //        this.victimBodyList.Add(component);
        //    }
        //}

        //void Pulverize(CharacterBody charb)
        //{
        //    charb.AddTimedBuff(BuffIndex.Pulverized, consume * 2);
        //}

        //private int consumeBuffs()
        //{
        //    int buffCount = characterBody.GetBuffCount(MinerPlugin.goldRushIndex);
        //    int output = Mathf.Max(0, buffCount);
        //    output = Mathf.Min(output, 3);
        //    characterBody.ClearTimedBuffs(MinerPlugin.goldRushIndex);
        //    for (int i = 0; i < buffCount - output; i++) { characterBody.AddTimedBuff(MinerPlugin.goldRushIndex, StaticValues.goldRushDuration); }
        //    return output;
        //}
    }
}