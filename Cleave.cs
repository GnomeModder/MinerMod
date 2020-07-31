using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using System.Collections.Generic;

namespace Gnome.EntityStatez
{
    public class CleaveState : BaseSkillState
    {
        public float baseDuration = 0.01f;
        private float duration;

        public GameObject bombPrefab = Assets.minerAssetBundle.LoadAsset<GameObject>("C4");
        MineTracker mineTracker;
        public override void OnEnter()
        {
            base.OnEnter();
            mineTracker = characterBody.GetComponent<MineTracker>();
            this.duration = this.baseDuration;
            if (base.isAuthority)
            {
                tossMine();
                mineTracker.timer = Time.fixedTime;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private void tossMine()
        {
            Ray aimRay = base.GetAimRay();

            GameObject bomb = UnityEngine.Object.Instantiate<GameObject>(bombPrefab, aimRay.origin + 2 * aimRay.direction, Quaternion.LookRotation(-1 * aimRay.direction));
            Rigidbody rig = bomb.GetComponent<Rigidbody>();
            rig.velocity = aimRay.direction * 30;

            mineTracker.mineList.Add(bomb);
        }
    }
}