using EntityStates;
using RoR2;
using UnityEngine;

namespace Gnome.EntityStatez
{
    public class BackBlastState : BaseSkillState
    {
        public float baseDuration = 0.25f;
        private float duration;
        public GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        public GameObject slashPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXSlash");

        Animator anim;
        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = base.GetAimRay();
            this.duration = this.baseDuration;

            anim = characterDirection.modelAnimator;
            if (base.isAuthority)
            {
                base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

                Vector3 theSpot = aimRay.origin + 2 * aimRay.direction;

                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 14f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = theSpot;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = base.characterBody.damage * 5f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 3f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = theSpot;
                effectData.scale = 15;

                EffectManager.SpawnEffect(slashPrefab, effectData, false);

                Util.PlaySound("Backblast", base.gameObject);

                base.characterMotor.velocity = -100 * aimRay.direction;

                base.StartAimMode(0.6f, true);

                float angle = Vector3.Angle(new Vector3(0, -1, 0), aimRay.direction);
                if (angle < 60) { anim.SetInteger("aimAngle", 1); }
                if (angle > 120) { anim.SetInteger("aimAngle", -1); }
                anim.SetTrigger("Backblast");
            }
        }
        public override void OnExit()
        {
            base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
            base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, StaticValues.lingeringInvincibilityDuration * .5f);

            base.characterMotor.velocity.x *= 0.1f;
            base.characterMotor.velocity.y *= 0.1f;
            base.characterMotor.velocity.z *= 0.1f;

            anim.SetInteger("aimAngle", 0);

            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((base.fixedAge >= this.duration && base.isAuthority) || (!base.IsKeyDownAuthority()))
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}