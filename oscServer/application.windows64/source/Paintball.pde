public class Paintball    //Class to describe a paintball impact
{
  float xPos;    //Parameters of the impact. Sent from Unity project
  float yPos;
  float diameter;
  float smallDiameter;
  float smallDiFactor = 0.08;
  float radius;
  color colour;
 
  int randomNum = int(random(5,30));
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
    
    colorMode(HSB,1.0);
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
