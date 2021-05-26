using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if (anim.speed > 0) anim.speed = 0; //If speed is higher than 0, set speed to 0 
            else if (anim.speed == 0) anim.speed = 1; //If speed is 0, set it to 1
        }

        if(anim.speed > 0)
        {
            if (Input.GetKeyDown(KeyCode.T)) anim.speed += 1;
            if (Input.GetKeyDown(KeyCode.G)) anim.speed -= 1;
        }
    }

    
}
