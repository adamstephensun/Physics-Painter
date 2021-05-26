using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lifetime : MonoBehaviour
{
    [HideInInspector] public float life = 0;

    // Update is called once per frame
    void Update()
    {
        if(life < 0.2) life += Time.deltaTime;
    }

    public float getLifeTime()
    {
        return life;
    }
}
