using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using System;

public class MineStickHurt : MonoBehaviour
{
    //HealthComponent component;
    //Transform otherTransform;
    //Vector3 displace;
    //bool once = false;
    void OnTriggerEnter(Collider other)
    {
        string ok = other.gameObject.name;
        Rigidbody rig = GetComponentInParent<Rigidbody>();
        if (!rig.isKinematic && ok != "TPBackZone" && ok != "KillZone" && ok != "General OOB" && ok != "ZONE: Outer Limit" && ok != "OOB Zone" && ok != "Player TP Zone" && ok != "TP Zone" && ok != "TeleportBackZone" && ok != "Env,Cave" && ok != "SpawnShopkeepTrigger" && ok != "Kill Zone" && ok != "HOLDER: Map Zone" && ok != "MapZone")
        {
            BoxCollider box = GetComponentInParent<BoxCollider>();
            box.enabled = false;
            
            rig.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rig.isKinematic = true;
            rig.useGravity = false;

            HurtBox component = other.GetComponent<HurtBox>();
            if (component)
            {
                HealthComponent component2 = component.healthComponent;
                MineStick theRealOne = GetComponentInParent<MineStick>();
                theRealOne.component = component2;
                theRealOne.otherTransform = other.transform;
                theRealOne.displace = this.transform.position - other.transform.position;
            }
        }
    }

    //void Update()
    //{
    //    if (component)
    //    {
    //        Transform trinsf = this.GetComponentInChildren<Transform>();
    //        trinsf.position = otherTransform.position + displace;
    //        once = true;
    //    }
    //    else if (once)
    //    {
    //        BoxCollider box = GetComponent<BoxCollider>();
    //        box.enabled = true;

    //        Rigidbody rig = GetComponent<Rigidbody>();
    //        rig.isKinematic = false;
    //        rig.useGravity = true;
    //        once = false;
    //    }
    //}
}