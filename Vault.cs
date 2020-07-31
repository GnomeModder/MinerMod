using EntityStates;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using KinematicCharacterController;

namespace Gnome.EntityStatez
{
    public class VaultState : BaseSkillState
    {
        public float baseDuration = 0.3f;
        private float duration;
        //public GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        public GameObject slashPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXSlash");

        BlastAttack blastAttack;
        List<CharacterBody> victimBodyList = new List<CharacterBody>();
        Ray aimRay;
        public override void OnEnter()
        {
            base.OnEnter();
            aimRay = base.GetAimRay();
            this.duration = this.baseDuration;
            if (base.isAuthority)
            {
                base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

                blastAttack = new BlastAttack();
                blastAttack.radius = 25f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = aimRay.origin;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = 0.1f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 3f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = aimRay.origin;
                effectData.scale = 15;

                EffectManager.SpawnEffect(slashPrefab, effectData, false);

                Util.PlaySound("Backblast", base.gameObject);

                getHitList(blastAttack);
                victimBodyList.ForEach(Suck);

                base.characterMotor.velocity = -60 * aimRay.direction;
            }
        }
        public override void OnExit()
        {
            base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
            base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, StaticValues.lingeringInvincibilityDuration * 0.5f);

            blastAttack.radius = 5f;
            blastAttack.Fire();

            victimBodyList.Clear();
            getHitList(blastAttack);
            victimBodyList.ForEach(Stop);

            base.characterMotor.velocity.x *= 0.1f;
            base.characterMotor.velocity.y *= 0.1f;
            base.characterMotor.velocity.z *= 0.1f;

            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((base.fixedAge >= this.duration && base.isAuthority))
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void getHitList(BlastAttack ba)
        {
            Collider[] array = Physics.OverlapSphere(ba.position, ba.radius, LayerIndex.defaultLayer.mask);
            int num = 0;
            int num2 = 0;
            while (num < array.Length && num2 < 12)
            {
                HealthComponent component = array[num].GetComponent<HealthComponent>();
                if (component)
                {
                    TeamComponent component2 = component.GetComponent<TeamComponent>();
                    if (component2.teamIndex != TeamIndex.Player)
                    {
                        this.AddToList(component.gameObject);
                        num2++;
                    }
                }
                num++;
            }
        }

        private void AddToList(GameObject affectedObject)
        {
            CharacterBody component = affectedObject.GetComponent<CharacterBody>();
            if (!this.victimBodyList.Contains(component))
            {
                this.victimBodyList.Add(component);
            }
        }

        void Suck(CharacterBody charb)
        {
            if (charb.characterMotor)
            {
                charb.characterMotor.velocity += (aimRay.origin - charb.corePosition) * 3;
            }
            else
            {
                Rigidbody component2 = charb.GetComponent<Rigidbody>();
                if (component2)
                {
                    component2.velocity += (aimRay.origin - charb.corePosition) * 3;
                }
            }
        }

        void Stop(CharacterBody charb)
        {
            if (charb.characterMotor)
            {
                charb.characterMotor.velocity *= 0.1f;
            }
            else
            {
                Rigidbody component2 = charb.GetComponent<Rigidbody>();
                if (component2)
                {
                    component2.velocity *= 0.1f;
                }
            }

        }
    }
}