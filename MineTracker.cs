using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using UnityEngine.Experimental.PlayerLoop;

public class MineTracker : MonoBehaviour
{
    public List<GameObject> mineList = new List<GameObject>();
    public float timer = 0;

    //public CharacterBody characterBody;
    //public CharacterMotor characterMotor;
    //public Ray aimRay;
    //public BlastAttack blastAttack;

    //void Update()
    //{
    //    mineList.RemoveAll(item => item == null);

    //    if (mineList.Count != 0 && (Time.fixedTime - timer) >= 2)
    //    {
    //        mineList.ForEach(detonate);

    //        float magnitude = Mathf.Min(80, characterMotor.velocity.magnitude);
    //        characterMotor.velocity = magnitude * characterMotor.velocity.normalized;

    //        mineList.Clear();
    //    }
    //}


    //private void tossMine()
    //{
    //    GameObject bomb = UnityEngine.Object.Instantiate<GameObject>(bombPrefab, aimRay.origin + 2 * aimRay.direction, Quaternion.LookRotation(-1 * aimRay.direction));
    //    Rigidbody rig = bomb.GetComponent<Rigidbody>();
    //    rig.velocity = aimRay.direction * 30;

    //    mineList.Add(bomb);

    //    timer = Time.fixedTime;
    //}

    //private void detonate(GameObject bomb)
    //{
    //    if (bomb)
    //    {
    //        EffectData effectData = new EffectData();
    //        effectData.origin = bomb.transform.position;
    //        effectData.scale = 6;
    //        EffectManager.SpawnEffect(explodePrefab, effectData, false);

    //        Util.PlaySound("DrillCharge", bomb);

    //        bombJump(blastAttack);

    //        Destroy(bomb);
    //    }
    //    else
    //    {
    //        Chat.AddMessage("DM me if you got this error");
    //    }
    //}

    //private void bombJump(BlastAttack ba)
    //{
    //    Vector3 poz = ba.position;
    //    if (poz != null)
    //    {
    //        Vector3 displacement = characterBody.corePosition - poz;
    //        Vector3 direction = displacement.normalized;
    //        if (displacement.magnitude < 1.5f) { direction = aimRay.direction; }
    //        characterMotor.velocity += direction * (30f / displacement.magnitude);
    //    }
    //}
}
