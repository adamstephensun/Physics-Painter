using UnityEngine;

public class OSCSender : MonoBehaviour
{
    private readonly string connectAddress = "/server/connect";
    private readonly string ballMessageAddress = "/server/ball";
    private readonly string laserMessageAddress = "/server/laser";
    private readonly string sprayMessageAddress = "/server/spray";
    private readonly string resetMessageAddress = "/server/reset";
    private readonly string saveMessageAddress = "/server/save";
    private readonly string darkmodeAddress = "/server/darkmode";

    public extOSC.OSCTransmitter transmitter;

    private int saveIndex;

    void Start()
    {
        var message = new extOSC.OSCMessage(connectAddress);
        transmitter.Send(message);
        Debug.Log("Sent connect message");

        saveIndex = 0;
    }

    public void sendBallPos(float x, float y, float sizeFactor, float hue, float sat, float val)
    {
        Debug.Log("Sent ball message x:" + x + "  y:" + y + "  diameter:" + sizeFactor);

        var message = new extOSC.OSCMessage(ballMessageAddress);
        message.AddValue(extOSC.OSCValue.Float(x));  //x axis position
        message.AddValue(extOSC.OSCValue.Float(y));  //y axis position
        message.AddValue(extOSC.OSCValue.Float(sizeFactor));  //size of decal
        message.AddValue(extOSC.OSCValue.Float(hue));   //hue
        message.AddValue(extOSC.OSCValue.Float(sat));   //saturation
        message.AddValue(extOSC.OSCValue.Float(val));   //value

        transmitter.Send(message);
    }

    public void sendLaserPos(float x, float y)
    {
        Debug.Log("Sent laser message x:" + x + "  y:" + y);

        var message = new extOSC.OSCMessage(laserMessageAddress);
        message.AddValue(extOSC.OSCValue.Float(x));  //x axis
        message.AddValue(extOSC.OSCValue.Float(y));  //y axis

        transmitter.Send(message);
    }

    public void sendSprayPos(float x, float y, float sizeFactor, float hue, float sat, float val)
    {
        Debug.Log("Sent spray message x:" + x + "  y:" + y + "  diameter:" + sizeFactor);

        var message = new extOSC.OSCMessage(sprayMessageAddress);
        message.AddValue(extOSC.OSCValue.Float(x));  //x axis
        message.AddValue(extOSC.OSCValue.Float(y));  //y axis
        message.AddValue(extOSC.OSCValue.Float(sizeFactor));  //size of decal
        message.AddValue(extOSC.OSCValue.Float(hue)); //hue
        message.AddValue(extOSC.OSCValue.Float(sat)); //sat
        message.AddValue(extOSC.OSCValue.Float(val)); //val

        transmitter.Send(message);
    }

    public void resetScene()
    {
        Debug.Log("Sent message: Scene reset");

        var message = new extOSC.OSCMessage(resetMessageAddress);
        transmitter.Send(message);
    }

    public void saveCopy()
    {
        Debug.Log("Send message: Save copy");

        var message = new extOSC.OSCMessage(saveMessageAddress);
        message.AddValue(extOSC.OSCValue.Int(saveIndex));
        saveIndex++;
        transmitter.Send(message);
    }

    public int getSaveIndex()
    {
        return saveIndex;
    }

    public void toggleDarkMode()
    {
        Debug.Log("Toggle dark mode");
        var message = new extOSC.OSCMessage(darkmodeAddress);
        transmitter.Send(message);
    }
}
