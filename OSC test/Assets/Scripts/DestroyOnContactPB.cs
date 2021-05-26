using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnContactPB : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Canvas") Destroy(this.gameObject);
    }
}
