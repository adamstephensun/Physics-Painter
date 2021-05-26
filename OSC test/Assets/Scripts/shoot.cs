using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shoot : MonoBehaviour
{
    #region defs

    public float paintballForce;  //The force that the paintball is fired with
    public float laserForce;
    public float sprayForce;
    public float sprayRange;

    [SerializeField]
    private GameObject paintballPrefab; //Prefab for the paintball
    [SerializeField]
    private GameObject laserPrefab;     //Prefab for the laser
    [SerializeField] private GameObject sprayPrefab;
    [SerializeField] AudioManager audioManager;

    private GameObject shootPoint;
    private GameObject FPController;

    private GameObject paintballGun;
    private GameObject laserGun;
    private GameObject sprayGun;

    private RectTransform wepSelectBackground;

    private Slider powerSlider;
    private Slider paintColourSlider;
    private Slider sprayColourSlider;

    [SerializeField] private GameObject sprayBody;
    [SerializeField] private GameObject paintballBody;

    private GameObject controls;
    private GameObject colourControls;


    private Color paintballColour;
    private Color sprayColour;

    private ParticleSystem sprayPartical;

    private bool pWhiteFlag;    //Flag for setting paintball to white
    private bool pBlackFlag;    //Flag for setting paintball to black
    private bool sWhiteFlag;    //Flag for setting spray to white
    private bool sBlackFlag;    //Flag for setting spray to black  

    enum Inventory      //Enum to store each weapon the player can use
    {
        None = 0,
        Paintball = 1,
        Laser = 2,
        Spray = 3
    }

    private Inventory currentWeapon;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        #region GameObject population
        FPController = GameObject.Find("FPController");
        shootPoint = GameObject.Find("ShootPoint");

        powerSlider = GameObject.Find("PowerSlider").GetComponent<Slider>();
        paintColourSlider = GameObject.Find("PaintColourSlider").GetComponent<Slider>();
        sprayColourSlider = GameObject.Find("SprayColourSlider").GetComponent<Slider>();

        wepSelectBackground = GameObject.Find("WepSelect").GetComponent<RectTransform>();

        paintballGun = GameObject.Find("PaintballGun");
        laserGun = GameObject.Find("LaserGun");
        sprayGun = GameObject.Find("SprayCan");

        sprayPartical = GameObject.Find("SprayPartical").GetComponent<ParticleSystem>();

        controls = GameObject.Find("Controls");
        colourControls = GameObject.Find("ColourControls");

        pWhiteFlag = false;
        pBlackFlag = false;
        sWhiteFlag = false;
        sBlackFlag = false;

        #endregion
        sprayPartical.Pause();
        changeWeapon(Inventory.Paintball);
    }

    void Update()
    {
        if (Input.GetKeyDown("1")) changeWeapon(Inventory.None);      //Changing weapons with key press
        if (Input.GetKeyDown("2")) changeWeapon(Inventory.Paintball);
        if (Input.GetKeyDown("3")) changeWeapon(Inventory.Laser);
        if (Input.GetKeyDown("4")) changeWeapon(Inventory.Spray);

        if (Input.GetButtonDown("Fire1")) {     //Single fire weapons
            switch(currentWeapon) {
                case Inventory.None:
                    break;
                case Inventory.Paintball:
                    audioManager.playSound("PaintballCharge");
                    break;
                case Inventory.Laser:
                    StartCoroutine(shootLaser());
                    audioManager.playSound("LaserFire");
                    break;
                case Inventory.Spray:
                    StartCoroutine(shootSpray());
                    audioManager.playSound("SprayFire");
                    break;
                default:
                    break;
            }
        }

        if(Input.GetButtonUp("Fire1"))
        {
            if (currentWeapon == Inventory.Laser) audioManager.stopSound("LaserFire");
            if (currentWeapon == Inventory.Spray) audioManager.stopSound("SprayFire");
        }

        if (currentWeapon == Inventory.Paintball)    //Input for paintball. Used for power meter
        {
            ///////Power slider////////
            powerSlider.gameObject.SetActive(true);
            if (Input.GetButton("Fire1"))
            {
                powerSlider.value += 0.05f;   //Increases the slider per frame
            }
            if (Input.GetButtonUp("Fire1"))
            {
                shootBall(powerSlider.value);
                powerSlider.value = 0;
                audioManager.stopSound("PaintballCharge");
                audioManager.playSound("PaintballFire");
            }
            if (powerSlider.value >= 10) audioManager.stopSound("PaintballCharge");

            ////////Colour Slider///////

            paintColourSlider.gameObject.SetActive(true);   //Enables the paintball colour slider
            float pastVal = paintColourSlider.value;
            if(!pWhiteFlag && !pBlackFlag) paintColourSlider.value += Input.mouseScrollDelta.y * 0.01f;    //Changes the value with scroll wheel

            if (Input.GetKeyDown(KeyCode.E)) //set to white with e press
            {
                if (pWhiteFlag) pWhiteFlag = false;
                else { pWhiteFlag = true; pBlackFlag = false; }
                audioManager.playSound("ScrollClick");
            }
            if(Input.GetKeyDown(KeyCode.Q)) //set to black with q press
            {
                if (pBlackFlag) pBlackFlag = false;
                else { pWhiteFlag = false; pBlackFlag = true; }
                audioManager.playSound("ScrollClick");
            }

            if (pWhiteFlag) paintballColour = Color.white;           //Sets colour to white is white flag is true
            else if (pBlackFlag) paintballColour = Color.black;      //Sets colour to black if black flag is true
            else if(!pWhiteFlag && !pBlackFlag) paintballColour = Color.HSVToRGB(paintColourSlider.value, 1, 1);      //Converts the slider from hue to rgb with saturation and value at 1

            if (paintColourSlider.value != pastVal) audioManager.playSound("ScrollClick");

            GameObject.Find("PaintColourHandle").GetComponent<Image>().color = paintballColour;     //Sets the colour of the slider handle
            paintballBody.GetComponent<Renderer>().material.color = paintballColour;        //Sets the colour of the paintball gun model to the current paintball colour

            colourControls.SetActive(true);
        }
        else        //Disable the power slider and paintball colour slider if paintball is unequipped
        {
            powerSlider.gameObject.SetActive(false);
            paintColourSlider.gameObject.SetActive(false);
        }

        if (currentWeapon == Inventory.Spray)       //Colour slider for spray can and model colour
        {
            sprayColourSlider.gameObject.SetActive(true);
            float pastVal = sprayColourSlider.value;
            if(!sWhiteFlag && !sBlackFlag) sprayColourSlider.value += Input.mouseScrollDelta.y * 0.01f;

            if (Input.GetKeyDown(KeyCode.E)) //set to white with e press
            {
                if (sWhiteFlag) sWhiteFlag = false;
                else { sWhiteFlag = true; sBlackFlag = false; }
                audioManager.playSound("ScrollClick");
            }
            if (Input.GetKeyDown(KeyCode.Q)) //set to black with q press
            {
                if (sBlackFlag) sBlackFlag = false;
                else { sWhiteFlag = false; sBlackFlag = true; }
                audioManager.playSound("ScrollClick");
            }

            if (sWhiteFlag) sprayColour = Color.white;          //Sets colour
            else if (sBlackFlag) sprayColour = Color.black;
            else if(!sWhiteFlag && !sBlackFlag) sprayColour = Color.HSVToRGB(sprayColourSlider.value, 1, 1);

            if (sprayColourSlider.value != pastVal) audioManager.playSound("ScrollClick");

            GameObject.Find("SprayColourHandle").GetComponent<Image>().color = sprayColour; //Sets the colour of the slider handle to the spray colour
            sprayBody.GetComponent<Renderer>().material.color = sprayColour;                //Sets the colour of the spray can to the spray colour

            var main = sprayPartical.main;  //Sets the colour of the partical to the spray colour
            main.startColor = sprayColour;

            if(Input.GetButton("Fire1")) sprayPartical.Emit(10);    //Emits particals when mouse is pressed

            colourControls.SetActive(true);
        }
        else sprayColourSlider.gameObject.SetActive(false);     //Deactivates the spray colour picker slider when it is unequipped

        if (currentWeapon == Inventory.Laser || currentWeapon == Inventory.None) colourControls.SetActive(false); //Dectivate colour controls when no colour tool is equipped
        if (Input.GetKeyDown(KeyCode.X)) controls.SetActive(!controls.activeSelf);
    }

    void changeWeapon(Inventory id)
    {
        switch (id)
        {
            case Inventory.None:
                currentWeapon = Inventory.None;
                Vector3 nonePos = new Vector3(-300, 0,0);
                wepSelectBackground.anchoredPosition = nonePos;     //Changes the UI to show the selected weapon

                paintballGun.SetActive(false);      //Changes the weapon viewmodel
                laserGun.SetActive(false);
                sprayGun.SetActive(false);
                break;
            case Inventory.Paintball:
                currentWeapon = Inventory.Paintball;
                Vector3 paintPos = new Vector3(-200, 0,0);
                wepSelectBackground.anchoredPosition = paintPos;

                paintballGun.SetActive(true);
                laserGun.SetActive(false);
                sprayGun.SetActive(false);
                break;
            case Inventory.Laser:
                currentWeapon = Inventory.Laser;
                Vector3 laserPos = new Vector3(-100, 0,0);
                wepSelectBackground.anchoredPosition = laserPos;

                paintballGun.SetActive(false);
                laserGun.SetActive(true);
                sprayGun.SetActive(false);
                break;
            case Inventory.Spray:
                currentWeapon = Inventory.Spray;
                Vector3 sprayPos = new Vector3(0, 0,0);
                wepSelectBackground.anchoredPosition = sprayPos;

                paintballGun.SetActive(false);
                laserGun.SetActive(false);
                sprayGun.SetActive(true);
                break;
            default:
                break;
        }

        audioManager.playSound("ChangeInv");
        Debug.Log("Weapon " + currentWeapon.ToString() + " equiped");
    }

    void shootBall(float power)        //add randomicity
    {
        //instantiate ball
        GameObject ballCopy = Instantiate<GameObject>(paintballPrefab, shootPoint.transform.position, shootPoint.transform.localRotation) as GameObject;
        ballCopy.GetComponent<Rigidbody>().AddRelativeForce(shootPoint.transform.forward * paintballForce * power);
        ballCopy.GetComponent<Renderer>().material.color = paintballColour;
        ballCopy.SendMessage("decalSize", power);   //Sends the power to the decal script so it can be resized
        Destroy(ballCopy, 8);       //Destroys the prefab after 8 seconds of no impact, saves performance
    }

    IEnumerator shootLaser()    //IEnumerator used for repeated fire laser with delay between shots. Shot delay timer could also be used
    {
        GameObject laserCopy = Instantiate<GameObject>(laserPrefab, shootPoint.transform.position, shootPoint.transform.localRotation) as GameObject;   //Creates a copy of the laser projectile prefab

        laserCopy.transform.rotation = FPController.transform.rotation;
        laserCopy.GetComponent<Rigidbody>().AddRelativeForce(shootPoint.transform.forward * laserForce);

        Destroy(laserCopy, 4);
        yield return new WaitForSeconds(0.1f);

        if(Input.GetButton("Fire1"))
        {
            StartCoroutine(shootLaser());
        }
    }

    IEnumerator shootSpray()
    {
        GameObject sprayCopy = Instantiate<GameObject>(sprayPrefab, shootPoint.transform.position, shootPoint.transform.localRotation) as GameObject;

        sprayCopy.transform.rotation = FPController.transform.rotation;
        sprayCopy.GetComponent<Rigidbody>().AddRelativeForce(shootPoint.transform.forward * laserForce);
        sprayCopy.GetComponent<Renderer>().material.color = sprayColour;

        Destroy(sprayCopy, sprayRange); //Gives the spray limited range
        sprayCopy.SendMessage("sprayLife", sprayCopy.GetComponent<lifetime>().getLifeTime());   //Sends the lifetime of the spray to the decal controller

        yield return new WaitForSeconds(0.1f);

        if (Input.GetButton("Fire1")) StartCoroutine(shootSpray());
    }
}
