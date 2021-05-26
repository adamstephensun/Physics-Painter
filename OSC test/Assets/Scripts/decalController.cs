using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decalController : MonoBehaviour
{
    enum ProjectileType //list of projectile types for different decals
    {
        None = 0,
        Paintball = 1,
        Laser = 2,
        Spray = 3
    }

    GameObject projectile;  //The projectile that is colliding with the canvas

    OSCSender oscSender;    //sends the osc messages to processing

    [SerializeField] GameObject decal;    //The decal for this projectile
    [SerializeField] ProjectileType projType = ProjectileType.None;    //The projectile type

    float hitXPos;  //The position of the hit
    float hitYPos;

    Vector3 decalSizeFactor = new Vector3(1,1,1);   //Used to scale the paintball decal depending on power
    float sprayLifeTime;

    // Start is called before the first frame update
    void Start()
    {
        projectile = this.gameObject;
        oscSender = GameObject.Find("OSC Sender").GetComponent<OSCSender>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Canvas")
        {
            Vector3 hitPos = collision.collider.ClosestPoint(collision.GetContact(0).point);    //Gets the location on the canvas of the impact

            GameObject decalCopy = Instantiate(decal, hitPos, collision.transform.rotation, collision.transform);   //Creates the decal
            decalCopy.transform.forward = collision.GetContact(0).normal * -1.0f;       //Sets the decal to face forward on the canvas
            decalCopy.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;  //Sets the decal to the colour of this projectile

            Vector3 newScale = new Vector3(decalCopy.transform.localScale.x * decalSizeFactor.x, decalCopy.transform.localScale.y * decalSizeFactor.y, decalCopy.transform.localScale.z * decalSizeFactor.z);   //Calculates the new size of the decal depending on ball speed
            decalCopy.transform.localScale = newScale;  //Sets the scale of the decal

            decalCopy.GetComponent<Renderer>().material.SetFloat("_Cutoff", Random.Range(0.25f,0.85f)); //Randomises the decal renderer cutoff to make decals have a varied edge

            hitXPos = decalCopy.transform.localPosition.z;
            hitYPos = decalCopy.transform.localPosition.y;

            switch(projType)
            {
                case ProjectileType.None:
                    break;
                case ProjectileType.Paintball:
                    float pH, pS, pV;  //Temp variable to store hsv colour value
                    Color.RGBToHSV(decalCopy.GetComponent<Renderer>().material.color, out pH, out pS, out pV); //Converts RGB colour of material to hsv
                    Debug.Log("Paintball impact local pos x:" + hitXPos + "  y:" + hitYPos + "  with size:"+newScale.x);
                    oscSender.sendBallPos(hitXPos, hitYPos, newScale.x, pH, pS, pV);    //sends newscale.x as the diameter as it is equal size. Sends hue
                    break;
                case ProjectileType.Laser:
                    Debug.Log("Laser impact local pos x:" + hitXPos + "  y:" + hitYPos);
                    oscSender.sendLaserPos(hitXPos, hitYPos);
                    break;
                case ProjectileType.Spray:
                    Debug.Log("Spray impact at local pos x:" + hitXPos + "  y:" + hitYPos);

                    float life = GetComponent<lifetime>().getLifeTime();
                    float min = 0;
                    float max = 0.2f;
                    float val = (1 - 0) / (max - min) * (life - max) + 1;
                    val = val * 5;
                    Debug.Log("Normalised life:" + val);    //Gets a normalised value for the lifespan of the spray projectile. Used for decal size

                    Vector3 sprayScale = new Vector3(decalCopy.transform.localScale.x * val, decalCopy.transform.localScale.y * val, decalCopy.transform.localScale.z * val);   //Calculates the new size of the decal depending on ball speed
                    decalCopy.transform.localScale = sprayScale;  //Sets the scale of the decal

                    float sH, sS, sV;
                    Color.RGBToHSV(decalCopy.GetComponent<Renderer>().material.color, out sH, out sS, out sV);  //Get colour of decal to send to osc
                    oscSender.sendSprayPos(hitXPos, hitYPos, val/5, sH, sS, sV);
                    break;
                default:
                    Debug.Log("No projectile type assigned to prefab");
                    break;
            }
        }
    }

    public void decalSize(float power)
    {
        decalSizeFactor = new Vector3(power *2, power * 2, 1);
    }

    public void sprayLife(float life)
    {
        float min = 0;
        float max = 0.2f;
        float val = (max - min) / (max - min) * (life - max) + max;
        sprayLifeTime = val;
        Debug.Log("Normalised life:" + val);
    }

    public float getHitXPos() { return hitXPos; }

    public float getHitYPos() { return hitYPos; }
}
