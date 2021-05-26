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
