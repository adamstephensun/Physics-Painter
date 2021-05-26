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

int myBroadcastPort = 8000;
int myListeningPort = 9000;

String myConnectPattern = "/server/connect";
String myDisconnectPattern = "/server/disconnect";

public void setup(){
    oscP5 = new OscP5(this, myListeningPort);
    frameRate(60);
    
    
    fill(126);
    background(102);
}

public void draw(){
    if(mousePressed){
        stroke(255);
        ellipse(mouseX, mouseY, 40, 40);
        println("circle on "+mouseX+" "+mouseY);
    }
}

public void oscEvent(OscMessage theOscMessage){

    println("#OSC# Recieved message: "+theOscMessage.addrPattern()+"     from address: "+theOscMessage.netAddress());

    if(theOscMessage.addrPattern().equals(myConnectPattern)){
        connect(theOscMessage.netAddress().address());
    }
    else if(theOscMessage.addrPattern().equals(myDisconnectPattern)){
        disconnect(theOscMessage.netAddress().address());
    }
    else oscP5.send(theOscMessage,myNetAddressList);
}

private void connect(String theIPaddress)
{
    if(!myNetAddressList.contains(theIPaddress, myBroadcastPort)){
        myNetAddressList.add(new NetAddress(theIPaddress, myBroadcastPort));
        println("#OSC# IP address "+theIPaddress+" added to list.");
    }
    else println("#OSC# IP address "+theIPaddress+"already in list.");

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

  public void settings() {  size(640,360); }
  static public void main(String[] passedArgs) {
    String[] appletArgs = new String[] { "oscServer" };
    if (passedArgs != null) {
      PApplet.main(concat(appletArgs, passedArgs));
    } else {
      PApplet.main(appletArgs);
    }
  }
}
