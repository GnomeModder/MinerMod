using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;

public class MineStick : MonoBehaviour
{
    public HealthComponent component;
    public Transform otherTransform;
    public Vector3 displace;
    bool once = false;
    float time;
    void OnTriggerEnter(Collider other)
    {
        time = Time.fixedTime;
        string ok = other.gameObject.name;
        //Chat.AddMessage(ok);
        if (ok != "TPBackZone" && ok != "KillZone" && ok != "General OOB" && ok != "ZONE: Outer Limit" && ok != "OOB Zone" && ok != "Player TP Zone" && ok != "TP Zone" && ok != "TeleportBackZone" && ok != "Env,Cave" && ok != "SpawnShopkeepTrigger" && ok != "Kill Zone" && ok != "HOLDER: Map Zone" && ok != "MapZone")
        {
            BoxCollider box = GetComponent<BoxCollider>();
            box.enabled = false;

            Rigidbody rig = GetComponent<Rigidbody>();
            rig.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rig.isKinematic = true;
            rig.useGravity = false;

            HealthComponent component2 = other.GetComponent<HealthComponent>();
            if (component2)
            {
                component = component2;
                otherTransform = other.transform;
                displace = this.transform.position - otherTransform.position;
            }
        }
    }

    void Update()
    {
        if (component)
        {
            this.transform.position = otherTransform.position + displace;
            once = true;
        }
        else if (once)
        {
            BoxCollider box = GetComponent<BoxCollider>();
            box.enabled = true;

            Rigidbody rig = GetComponent<Rigidbody>();
            rig.isKinematic = false;
            rig.useGravity = true;
            once = false;
        }

        if ((Time.fixedTime - time) % 2 == 0)
        {
            Util.PlaySound("Tick", this.gameObject);
        }
    }
}
