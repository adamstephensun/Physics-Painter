import processing.core.*; 
import processing.data.*; 
import processing.event.*; 
import processing.opengl.*; 

import netP5.*; 
import oscP5.*; 
import oscP5.*; 
import netP5.*; 

import java.util.HashMap; 
import java.util.ArrayList; 
import java.io.File; 
import java.io.BufferedReader; 
import java.io.PrintWriter; 
import java.io.InputStream; 
import java.io.OutputStream; 
import java.io.IOException; 

public class oscServer extends PApplet {







OscP5 oscP5;
NetAddressList myNetAddressList = new NetAddressList();   //List of connected net addresses

int myBroadcastPort = 8000;     //The port that the server broadcasts through
int myListeningPort = 8050;     //The port that the server listens to

NetAddress myRemoteLocation = new NetAddress("127.0.0.1", myBroadcastPort);

int windowSizeX = 900;    //Width of window
int windowSizeY = 640;    //Height of window

int bgLight = color(255,252,247);
int bgDark = color(31,31,31);

String myConnectPattern = "/server/connect";    //Pattern to connect to the server
String connectConfirmPattern = "server/confirm";  //Pattern to confirm to Unity that it is connected
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

public void setup(){
    oscP5 = new OscP5(this, myListeningPort);  //Setup for listening for OSC messages
    frameRate(60);
    
        //Window size
    if(darkMode) background(bgDark);  //Sets the background colour depending on if dark mode is being used
    else background(bgLight);
}

public void draw(){
    if(mousePressed){    //Places an orange circle on mouse click. For testing
        //stroke(255);
        //fill(204, 102, 0);
        //ellipse(mouseX, mouseY, 40, 40);
        //println("circle on "+mouseX+" "+mouseY);
        
        //recievedPaintballs.add(new Paintball(mouseX, mouseY, 0.04, 0.04));
        //newPaintballData = true;
        
        recievedSprays.add(new Spray(mouseX,mouseY, 0.3f));
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

public void keyPressed()    //Saves a screengrab to the specified location on key press
{
  if(key == 'i') save("saves/testsave.png");
}

public void drawPaintball(float x, float y, float size, float h, float s, float v)    //Adds a new paintball to the array using the data from the osc message
{
  float newX = abs(x) * (width * 2);
  float newY = abs(y) * (height * 2);
  
  recievedPaintballs.add(new Paintball(newX, newY, size, h, s, v));
  
  newPaintballData = true;
  println("Paintball on "+newX+" "+newY+ "  size:"+size); 
}

public void drawLaser(float x, float y, boolean darkMode)      //Adds a new laser to the array using the data from the osc message
{
  float newX = abs(x) * (width * 2);
  float newY = abs(y) * (height * 2);
  
  recievedLasers.add(new Laser(newX, newY, darkMode));
  
  newLaserData = true;
  println("Laser on "+newX+" "+newY); 
}

public void drawSpray(float x, float y, float size, float h, float s, float v)    //Adds a new spray to the array using the data from the osc message
{
  float newX = abs(x) * (width * 2);
  float newY = abs(y) * (height * 2);
  
  recievedSprays.add(new Spray(newX,newY, size, h, s, v));
  
  newSprayData = true;
  println("Spray on "+newX+" "+newY+ "  size:"+size); 
}

public void oscEvent(OscMessage theOscMessage){    //check if the address pattern fits any of our patterns

    println("#OSC# Recieved message: "+theOscMessage.addrPattern()+"     from address: "+theOscMessage.netAddress());

    if(theOscMessage.addrPattern().equals(myConnectPattern)){   // if incoming = connect pattern, then connect
        connect(theOscMessage.netAddress().address());
    }
    else if(theOscMessage.addrPattern().equals(myDisconnectPattern)){   // if incoming = disconnect pattern, then disconnect
        disconnect(theOscMessage.netAddress().address());
    }
    else if(theOscMessage.addrPattern().equals(ballPattern)){    //Recieved a message for a paintball impact
        float oscX = theOscMessage.get(0).floatValue();    //Get the values from the OSC message
        float oscY = theOscMessage.get(1).floatValue();
        float impactSize = theOscMessage.get(2).floatValue();
        float hue = theOscMessage.get(3).floatValue();
        float sat = theOscMessage.get(4).floatValue();
        float val = theOscMessage.get(5).floatValue();
        
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

public void saveImage(String path)
{
  save(path);
  println("Screengrab saved to: "+ path);
  
  OscMessage imageMessage = new OscMessage(imageSentPattern);
  oscP5.send(imageMessage, myRemoteLocation);
}

public void reset()    //resets the scene, called when a reset osc message is recieved
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

public void toggleDarkMode()
{
   darkMode = !darkMode; 
}

public void sendConnectConfirm()
{
    OscMessage connectConfirmMessage = new OscMessage(connectConfirmPattern);
    oscP5.send(connectConfirmMessage, myRemoteLocation);
    println("Connect confirmation sent"); 
}
public class Laser    //Class to describe a laser impact
{
  float xPos;      //Parameters of the impact. Sent from Unity project
  float yPos;
  float diameter = 20;    //All laser impacts are the same size
  float colour;
  
  public Laser()  //default constructor
  {
    xPos = 200;
    yPos = 200;
    colour = 0;
  }
  
    public Laser(float x, float y, boolean darkMode)  //Constructor with pos and size
  {
    xPos = x;
    yPos = y;
    if(darkMode) colour = 255;
    else colour = 0;
  }
  
  public void draw()
  {
      fill(colour);
      ellipseMode(CENTER);
      noStroke();
      ellipse(xPos,yPos, diameter, diameter);
      println("Laser drawn at   x:"+xPos+"  y:"+yPos);
  }
}
public class Paintball    //Class to describe a paintball impact
{
  float xPos;    //Parameters of the impact. Sent from Unity project
  float yPos;
  float diameter;
  float smallDiameter;
  float smallDiFactor = 0.08f;
  float radius;
  int colour;
 
  int randomNum = PApplet.parseInt(random(5,30));
  float[] randomXPos = new float[randomNum];
  float[] randomYPos = new float[randomNum];
  float[] xVariation = new float[randomNum];
  float[] yVariation = new float[randomNum];
  
  float randomRange = 10;
  
  public Paintball()  //default constructor
  {
    xPos = 200;
    yPos = 200;
    diameter = 40;
    radius = diameter/2;
    smallDiameter = diameter * smallDiFactor;
    
    //randomNum = random(5,30);
    
    for(int i = 0; i<= randomNum;i++)
    {
      xVariation[i] = random(-10,10);
      yVariation[i] = random(-10,10);
      randomXPos[i] = random(-randomRange - radius,randomRange) + xVariation[i];
      randomYPos[i] = random(-randomRange,randomRange) + yVariation[i];
    }
    
    colour = color(255,255,255,255);
  }
  
  public Paintball(float x, float y, float _diameter)  //Constructor with pos and size
  {
    xPos = x;
    yPos = y;
    diameter = _diameter * 1000;    //scales the diameter from the scale variable from unity
    radius = diameter/2;
    colour = color(35, 200, 80, 255);  //green default colour
    smallDiameter = diameter * smallDiFactor;
    
    //randomNum = random(5,30);
    
    for(int i = 0; i<= randomNum;i++)
    {
      xVariation[i] = random(-10,10);
      yVariation[i] = random(-10,10);
      randomXPos[i] = random(-randomRange,randomRange) + xVariation[i];
      randomYPos[i] = random(-randomRange,randomRange) + yVariation[i];
    }
  }
  
  public Paintball(float x, float y, float _diameter, float h, float s, float v)  //Constructor with pos, size, and colour
  {
    xPos = x;
    yPos = y;
    diameter = _diameter * 1000;
    radius = diameter/2;
    smallDiameter = diameter * smallDiFactor;  //Small diameter is a quarter of the normal diameter
    
    colorMode(HSB,1.0f);
    colour = color(h,s,v);
    
    //randomNum = random(5,30);
    
    for(int i = 0; i< randomNum;i++)
    {
      xVariation[i] = random(-10,10);
      yVariation[i] = random(-10,10);
      randomXPos[i] = random(-randomRange - radius,randomRange + radius) + xVariation[i];
      randomYPos[i] = random(-randomRange - radius,randomRange + radius) + yVariation[i];
    }
  }
  
  public void draw()    //Draws the paintball
  {
      noStroke();
      fill(colour);
      ellipseMode(CENTER);
      ellipse(xPos,yPos,diameter,diameter);
      
      for(int i = 0;i < randomNum;i++)
      {
          ellipse(xPos + randomXPos[i], yPos + randomYPos[i], smallDiameter,smallDiameter);
      }
      
      println("Paintball drawn at   x:"+xPos+"  y:"+yPos+"  diameter:"+diameter);
  }
}
public class Spray      //Class to describe a spray impact
{
  float xPos;            //Parameters of the impact. Sent from Unity project
  float yPos;
  float sprayDiameter;
  float diameterMultiplier = 200;
  int colour;
  float radius;
  float dia = 2; 
  
  int n = 12;  //Number of particles per spray
  float[] randAngle = new float[n];  //Angles of all the particles
  float[] randDist = new float[n];   //Distance of all the particles
  float[] newX = new float[n];       //x pos of particles
  float[] newY = new float[n];       //y pos of particles
  
  public Spray()  //default constructor
  {
    xPos = 200;
    yPos = 200;
    sprayDiameter = 40;
    radius = sprayDiameter/2;
    colour = color(255,255,255,150);
    
    for(int i = 0; i < n; i++)
    {
      randAngle[i] = random(0,TWO_PI);
      randDist[i] = random(0,radius);
      
      newX[i] = xPos + (randDist[i] * cos(randAngle[i]));
      newY[i] = yPos + (randDist[i] * sin(randAngle[i]));
    }
  }
  
    public Spray(float x, float y, float _diameter)  //Constructor with pos and size
  {
    xPos = x;
    yPos = y;
    radius = (_diameter * diameterMultiplier)/2;    //scales the diameter from the scale variable from unity
    //radius = sprayDiameter/2;
    
    for(int i = 0; i < n; i++)
    {
      randAngle[i] = random(0,TWO_PI);
      randDist[i] = random(0,radius);
      
      newX[i] = xPos + (randDist[i] * cos(randAngle[i]));
      newY[i] = yPos + (randDist[i] * sin(randAngle[i]));
    }
    
    colour = color(200, 20, 0, 255);  //default red colour
  }
  
  public Spray(float x, float y, float _diameter, float h, float s, float v)  //Constructor with pos, size, and colour
  {
    xPos = x;
    yPos = y;
    sprayDiameter = _diameter * diameterMultiplier;
    radius = sprayDiameter/2;
    colorMode(HSB,1.0f);
    colour = color(h,s,v);
    
    for(int i = 0; i < n; i++)
    {
      randAngle[i] = random(0,TWO_PI);
      randDist[i] = random(0,radius);
      
      newX[i] = xPos + (randDist[i] * cos(randAngle[i]));
      newY[i] = yPos + (randDist[i] * sin(randAngle[i]));
    }
  }
  
  public void draw()
  {
      noStroke();
      fill(colour);
      ellipseMode(CENTER);
      
      for(int i = 0; i < n; i++)
      {
        ellipse(newX[i],newY[i], dia,dia);
      }
      println("Spray drawn at   x:"+xPos+"  y:"+yPos+"  diameter:"+sprayDiameter);
  }
}
  public void settings() {  size(900, 640); }
  static public void main(String[] passedArgs) {
    String[] appletArgs = new String[] { "oscServer" };
    if (passedArgs != null) {
      PApplet.main(concat(appletArgs, passedArgs));
    } else {
      PApplet.main(appletArgs);
    }
  }
}
