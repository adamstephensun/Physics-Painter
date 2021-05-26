import netP5.*;
import oscP5.*;

import oscP5.*;
import netP5.*;

OscP5 oscP5;
NetAddressList myNetAddressList = new NetAddressList();   //List of connected net addresses

int myBroadcastPort = 8000;     //The port that the server broadcasts through
int myListeningPort = 8050;     //The port that the server listens to

NetAddress myRemoteLocation = new NetAddress("127.0.0.1", myBroadcastPort);

int windowSizeX = 900;    //Width of window
int windowSizeY = 640;    //Height of window

color bgLight = color(255,252,247);
color bgDark = color(31,31,31);

String myConnectPattern = "/server/connect";        //Pattern to connect to the server
String connectConfirmPattern = "server/confirm";    //Pattern to confirm to Unity that it is connected
String myDisconnectPattern = "/server/disconnect";  //Pattern to disconnect from the server
String ballPattern = "/server/ball";                //Pattern for a paintball impact
String laserPattern = "/server/laser";              //Pattern for a laser impact
String sprayPattern = "/server/spray";              //Pattern for a spray impact
String resetPattern = "/server/reset";              //Pattern for resetting the scene
String savePattern = "/server/save";                //Pattern for saving a seperate copy of the canvas
String imageSentPattern = "/server/image";          //Pattern for confirming an image sent to Unity
String darkmodePattern = "/server/darkmode";        //Pattern for toggling dark mode

ArrayList<Paintball> recievedPaintballs = new ArrayList<Paintball>();    //Arrays to store the impacts
ArrayList<Laser> recievedLasers = new ArrayList<Laser>();
ArrayList<Spray> recievedSprays = new ArrayList<Spray>();
 
boolean newPaintballData = false;    //Flags to determine when a new impact is registered
boolean newLaserData = false;
boolean newSprayData = false;
boolean resetFlag = false;
boolean darkMode = true;

void setup(){
    oscP5 = new OscP5(this, myListeningPort);  //Setup for listening for OSC messages
    frameRate(60);
    
    size(900, 640);    //Window size
    if(darkMode) background(bgDark);  //Sets the background colour depending on if dark mode is being used
    else background(bgLight);
}

void draw(){
    if(mousePressed){    //Places an orange circle on mouse click. For testing
        //stroke(255);
        //fill(204, 102, 0);
        //ellipse(mouseX, mouseY, 40, 40);
        //println("circle on "+mouseX+" "+mouseY);
        
        //recievedPaintballs.add(new Paintball(mouseX, mouseY, 0.04, 0.04));
        //newPaintballData = true;
        
        recievedSprays.add(new Spray(mouseX,mouseY, 0.3));
        newSprayData = true;
    }
    
    if(resetFlag)    //Resets the scene using a flag to avoid the threading shenanigans
    {
      if(darkMode) background(bgDark); 
      else background(bgLight);
      
      resetFlag = false;
    }
    
    if(newPaintballData){  //Draws the list of paintballs if the new data flag is true
        for(Paintball p : recievedPaintballs) p.draw(); 
        newPaintballData = false;
    }

    if(newLaserData){    //draws the lasers 
      for(Laser l : recievedLasers) l.draw();
      newLaserData = false;
    }
    
    if(newSprayData){    //draws the sprays
      for(Spray s : recievedSprays) s.draw();
      newSprayData = false;
    }
}

void keyPressed()    //Saves a screengrab to the specified location on key press
{
  if(key == 'i') save("saves/testsave.png");
}

void drawPaintball(float x, float y, float size, float h, float s, float v)    //Adds a new paintball to the array using the data from the osc message
{
  float newX = abs(x) * (width * 2);
  float newY = abs(y) * (height * 2);
  
  recievedPaintballs.add(new Paintball(newX, newY, size, h, s, v));
  
  newPaintballData = true;
  println("Paintball on "+newX+" "+newY+ "  size:"+size); 
}

void drawLaser(float x, float y, boolean darkMode)      //Adds a new laser to the array using the data from the osc message
{
  float newX = abs(x) * (width * 2);
  float newY = abs(y) * (height * 2);
  
  recievedLasers.add(new Laser(newX, newY, darkMode));
  
  newLaserData = true;
  println("Laser on "+newX+" "+newY); 
}

void drawSpray(float x, float y, float size, float h, float s, float v)    //Adds a new spray to the array using the data from the osc message
{
  float newX = abs(x) * (width * 2);
  float newY = abs(y) * (height * 2);
  
  recievedSprays.add(new Spray(newX,newY, size, h, s, v));
  
  newSprayData = true;
  println("Spray on "+newX+" "+newY+ "  size:"+size); 
}

void oscEvent(OscMessage theOscMessage){    //check if the address pattern fits any of our patterns

    println("#OSC# Recieved message: "+theOscMessage.addrPattern()+"     from address: "+theOscMessage.netAddress());

    if(theOscMessage.addrPattern().equals(myConnectPattern)){   // if incoming = connect pattern, then connect
        connect(theOscMessage.netAddress().address());
    }
    else if(theOscMessage.addrPattern().equals(myDisconnectPattern)){   // if incoming = disconnect pattern, then disconnect
        disconnect(theOscMessage.netAddress().address());
    }
    else if(theOscMessage.addrPattern().equals(ballPattern)){    //Recieved a message for a paintball impact
        float oscX = theOscMessage.get(0).floatValue();    //Get values from message. X position
        float oscY = theOscMessage.get(1).floatValue();    //Y position
        float impactSize = theOscMessage.get(2).floatValue();  //Size
        float hue = theOscMessage.get(3).floatValue();         //Hue
        float sat = theOscMessage.get(4).floatValue();         //Saturation
        float val = theOscMessage.get(5).floatValue();         //Value
        
        println("#OSC# Recieved message: "+theOscMessage.addrPattern()+"  x:"+oscX+"  y:"+oscY+"  size:" + impactSize + "  hue:" + hue);
        drawPaintball(oscX,oscY, impactSize, hue, sat, val);  //Draw the paintball using the data from the OSC message
    }
    else if(theOscMessage.addrPattern().equals(laserPattern)){    //recievd a message for a laser impact
        float oscX = theOscMessage.get(0).floatValue();
        float oscY = theOscMessage.get(1).floatValue();
        
        println("#OSC# Recieved message: "+theOscMessage.addrPattern()+"  x:"+oscX+"  y:"+oscY);
        drawLaser(oscX,oscY, darkMode);
    }
    else if(theOscMessage.addrPattern().equals(sprayPattern)){    //Recieved message for a spray impact
        float oscX = theOscMessage.get(0).floatValue();
        float oscY = theOscMessage.get(1).floatValue();
        float impactSize = theOscMessage.get(2).floatValue();
        float hue = theOscMessage.get(3).floatValue();
        float sat = theOscMessage.get(4).floatValue();
        float val = theOscMessage.get(5).floatValue();
        
        println("#OSC# Recieved message: "+theOscMessage.addrPattern()+"  x:"+oscX+"  y:"+oscY+"  size:" + impactSize);
        drawSpray(oscX,oscY,impactSize, hue, sat, val);
    }
    else if(theOscMessage.addrPattern().equals(resetPattern)) reset();  //resets the scene
    else if(theOscMessage.addrPattern().equals(darkmodePattern)) toggleDarkMode();
    else oscP5.send(theOscMessage,myNetAddressList);    // if incoming != any existing patterns, broadcast message
    
    if(theOscMessage.addrPattern().equals(savePattern))
    {
        int saveIndex = theOscMessage.get(0).intValue();
        
        saveImage("saves/savedCanvas"+saveIndex+".png");
    }
    
    if(theOscMessage.addrPattern().equals(ballPattern) || theOscMessage.addrPattern().equals(laserPattern) || theOscMessage.addrPattern().equals(sprayPattern))  //Saves an image whenever a impact is sent
    {
      saveImage("C:/Users/adams/OneDrive/Documents/Projects/Unity Projects/OSC test/Assets/Resources/processingImage.png");
    }
    
    
}

private void connect(String theIPaddress)
{
    if(!myNetAddressList.contains(theIPaddress, myBroadcastPort)){      /* if incoming address isn't in address list, add it*/
        myNetAddressList.add(new NetAddress(theIPaddress, myBroadcastPort));
        println("#OSC# IP address "+theIPaddress+" added to list.");
    }
    else println("#OSC# IP address "+theIPaddress+"already in list.");
    
    sendConnectConfirm();
        
    println("#OSC# There are currently "+myNetAddressList.list().size()+" remote locations connected");
}

private void disconnect(String theIPaddress)
{
    if(myNetAddressList.contains(theIPaddress, myBroadcastPort)){
        myNetAddressList.remove(theIPaddress, myBroadcastPort);
        println("#OSC# IP address "+theIPaddress+" removed from list.");
    }
    else println("#OSC# IP address "+theIPaddress+"is not connected.");

    println("#OSC# There are currently "+myNetAddressList.list().size()+" remote locations connected");
}

void saveImage(String path)
{
  save(path);
  println("Screengrab saved to: "+ path);
  
  OscMessage imageMessage = new OscMessage(imageSentPattern);
  oscP5.send(imageMessage, myRemoteLocation);
}

void reset()    //resets the scene, called when a reset osc message is recieved
{
   println("Scene reset");
   resetFlag = true;    //sets the flag to reset the scene to true. this avoids problems with threading
   
   recievedPaintballs.clear();
   recievedLasers.clear();
   recievedSprays.clear();
   newPaintballData = true;
   newLaserData = true;
   newSprayData = true;
   
   saveImage("C:/Users/adams/OneDrive/Documents/Projects/Unity Projects/OSC test/Assets/Resources/processingImage.png");  //saves a new image of the blank screen to send to unity
}

void toggleDarkMode()
{
   darkMode = !darkMode; 
}

void sendConnectConfirm()
{
    OscMessage connectConfirmMessage = new OscMessage(connectConfirmPattern);
    oscP5.send(connectConfirmMessage, myRemoteLocation);
    println("Connect confirmation sent"); 
}
