using UnityEngine;
using System.Collections;
using RaydraTournament;


/*class \PasstheBall 
    this class is used to launch the ball to the targeted player
*/

public class PassTheBall : MonoBehaviour
{
	
	public GameObject PlayerTwo; //! Reference to the test player
    public GameObject Ball; //!Reference to the ball the players holds

	private CentralHub Hub;//!Refrence to Central Hub variable that contains data central to the game
	
	
	CellActions CellAction;  //! What the Cell is doing at the moment

    Vector3 BallPosition = new Vector3(0, 0, 0); //! Current position of the ball
    Vector3 CurrentBallPosition = new Vector3(0, 0, 0);//! Current position of the ball
    Vector3 BallDirection; //! The direction the ball is traveling in the (ground) X/Z
    Vector3 OriginalBallPosition; //! Original position of the ball
    Vector3 NewOrginalBallPosition; //!Reset position of the ball
	
	
    private     float BallGroundSpeed,
        Gravity=50f, //! Force of graving acting on the ball
        LandingDistance, //! Distance the ball travels to the target
        LandingTime=0, //! The time it will take the ball to reach the target
        AirTime=0,  //! Amount of time the ball has been in the air
        InitialVelocity; //<!Initial speed the the ball takes off with
	 private float BallHeight, //<!Current vertical height of the ball
        frameRate = 1f/60f; //!the current frame rate (this needs to be adjusted to a value the is given by the Unity Engine) 

	
	//*! Use this for initialization
	void Start ()
	{
        GameObject RaydraCell = GameObject.Find("RaydraCell");//Find the object called RaydraCell

        CellAction = RaydraCell.GetComponent<CellActions>();//retrive the state of the cell from the ball
     
        CellAction.CellState1 = CellState.IsInHand;
		
		
		NewOrginalBallPosition = PlayerTwo.transform.position;
		OriginalBallPosition = CellAction.CarryPoint.transform.position; //initailize the ball position to the thrower
        Debug.Log("Ball State " + CellAction.CellState1.ToString());
        // Debug.Log("Player two position first " +PlayerTwo.transform.position.ToString());

    }
	
	/** Update is called once per frame */
	void Update ()
	{
        
		
		
		GameObject[] Ara = new GameObject[3];
		
		Animator X8Animator = this.GetComponents<Animator>()[0];
		AnimatorStateInfo StateInfo = X8Animator.GetCurrentAnimatorStateInfo(0);
		//!if the Current animation state is played through reset the state
		if (!StateInfo.IsName("ThrowBall") )
			CellAction.CellAnimationState = CellAnimationState.isNotPlaying;


        if (CellAction.CellState1 == CellState.IsInHand && CellAction.CellAnimationState == CellAnimationState.isNotPlaying)
        { //is the ball is not in the air

            //! Vector3 NewOriginalBallPosition = this.transform.position;
            //!make sure the ball is with the player when its not in the air
            
            OriginalBallPosition = new Vector3(Ball.transform.position.x, Ball.transform.position.y, Ball.transform.position.z);
          
            /*!	OriginalBallPosition.z+=3;
						launch the ball when the button has been pressed*/

            if (StateInfo.IsName("ThrowBall"))
            {
          
                if (StateInfo.normalizedTime > .37) {//we are past the throwing action
                
                    X8Animator.SetBool("ThrowBall", false);
                    X8Animator.SetBool("ThrowingAction", false);
                    CellAction.CellState1 = CellState.IsInAir;  //!ball is in the air
                    CellAction.CellAnimationState = CellAnimationState.isPlaying; //!The Current Animation is playing
                                                                                //OriginalBallPosition = new Vector3(OrbPosition.transform.position.x,OrbPosition.transform.position.y,OrbPosition.transform.position.z);

                    BallDirection = (PlayerTwo.transform.position - OriginalBallPosition);

                    //!we are only interested in the x and z components at the moment
                    LandingDistance = BallDirection.magnitude;

                    BallDirection.Normalize();/*!lets get the absolute direction of the ball
								this has to be done AFTER the initial ball magnitide is taken
								so the we dont get a ball distance of one*/
                    LandingTime = LandingDistance / Gravity;

                    BallGroundSpeed = LandingDistance / LandingTime; //!LandingDistance / LandingTime;//speed the ball is moving along the plane of

                    BallPosition.y = OriginalBallPosition.y;
                    InitialVelocity = (LandingTime / 2) * Gravity; //!how fast the ball starts off moving

                }
                else
                    X8Animator.SetBool("ThrowingAction", true);//we are in the throwing action

            }
        }
		
		if (CellAction.CellState1  ==  CellState.IsInAir  ) {//!if the ball has been launched
                                                         //calculate the distance to the player you want to throw the ball 2

            
            //!the ball height right at the moment
            BallHeight = (Gravity/-2f) * (AirTime * AirTime) + (InitialVelocity * AirTime); 
			AirTime += frameRate;
			CurrentBallPosition.x = BallPosition.x + OriginalBallPosition.x;
			CurrentBallPosition.z = BallPosition.z + OriginalBallPosition.z;
			CurrentBallPosition.y = BallPosition.y + BallHeight ;
			BallPosition+= BallDirection* (BallGroundSpeed/60);//move the ball toward the player correct direction each frame
                                                               //!WE DEVIDE THE BALLGROUND SPEED BY 60 TO GET HOW FAST THE BALL MOVES EVERY FRAME

            //!PlayerTwo.transform.position = NewOrginalBallPosition;
            Ball.transform.position = CurrentBallPosition;
          
			
			if ((AirTime > LandingTime) ) { //!if the ball has landed
				
				AirTime = 0;

				BallPosition.z=0;
				BallPosition.x=0;

                CellAction.CellState1 = CellState.IsInHand;


            }
			
		}


    }




    public GameObject  GetPlayer2() //<!this medtod give access to the second player
	{
		
		
		return PlayerTwo; //!give access to the second Player
	}
	
	
}

