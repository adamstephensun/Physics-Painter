using UnityEngine;

public class OSCReciever : MonoBehaviour
{
    private string imageAddress = "/server/image";

    [SerializeField] private extOSC.OSCReceiver reciever;
    public SpriteRenderer spriteRenderer;

    private Texture2D myTex;
    private Sprite mySprite;
    private bool newImageFlag;

    void Start()
    {
        reciever.Bind(imageAddress, ReceivedMessage);
        newImageFlag = false;
    }

    private void Update()
    {
        if(newImageFlag)
        {
            myTex = Resources.Load<Texture2D>("processingImage");
            if (myTex == null) Debug.Log("Load failed");
            else
            {
                Debug.Log("Image file loaded");
                mySprite = Sprite.Create(myTex, new Rect(0, 0, myTex.width, myTex.height), Vector2.zero);
                spriteRenderer.sprite = mySprite;
            }
            newImageFlag = false;
        }
    }

    protected void ReceivedMessage(extOSC.OSCMessage message)
    {
        Debug.Log("Recieved message: " + message.Address);

        if (message.Address == imageAddress)
        {
            newImageFlag = true;
        }
    }
}
