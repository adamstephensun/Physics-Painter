using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyOnContact : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Projectile") Destroy(this.gameObject);
    }
}
