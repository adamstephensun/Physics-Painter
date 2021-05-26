public class Spray      //Class to describe a spray impact
{
  float xPos;            //Parameters of the impact. Sent from Unity project
  float yPos;
  float sprayDiameter;
  float diameterMultiplier = 200;
  color colour;
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
    xPos = x;  //X position
    yPos = y;  //Y position
    sprayDiameter = _diameter * diameterMultiplier; //Convert diameter val
    radius = sprayDiameter/2;  //Get radius
    colorMode(HSB,1.0);     //Set colour mode to hue,sat,val
    colour = color(h,s,v);  //Set colour
    
    for(int i = 0; i < n; i++)  //Loop through all particles in spray (n=12)
    {
      randAngle[i] = random(0,TWO_PI);    //Create 12 random angles 
      randDist[i] = random(0,radius);     //Create 12 random distances
      
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
