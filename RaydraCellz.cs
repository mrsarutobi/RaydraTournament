using UnityEngine;
using System.Collections;

namespace RaydraTournament {
    /**
    /*! \class RaydraCell 
     * This class controls the Raydra Cell(ball) 
     * it contains information about when the cell is held in the players hand
     * And what state the Cell is currently in*/
    public class RaydraCellz: MonoBehaviour
    {
        private CellState LocationOfTheCell;
        private CellMotionState CellMoveState;

        public GameObject CarryPoint; //!Where the ball at in the players hand

       // private RaydraCell RaydraOrbState;//! The state the Cell is in at the moment
        private Animator X8Animator; //! reference to the Animator that is attached to this player
        private AnimatorStateInfo StateInfo; //! Current state to the the animator

        public GameObject PlayerTwo; //! Reference to the test player
        public GameObject Ball; //!Reference to the ball the players holds
        private RaydraPlayerz ThePlayer; //!The Player that is holding this ball
        private CentralHub Hub;//!Refrence to Central Hub variable that contains data central to the game

        private RaydraControls Controls; //referece to the controls of the player
        

        
        Vector3 BallPosition = new Vector3(0, 0, 0); //! Current position of the ball
        Vector3 CurrentBallPosition = new Vector3(0, 0, 0);//! Current position of the ball
        Vector3 BallDirection; //! The direction the ball is traveling in the (ground) X/Z
        Vector3 OriginalBallPosition; //! Original position of the ball
 

        Vector3 BallTravelPath;
        private float BallGroundSpeed,
            Gravity = 75f, //! Force of graving acting on the ball
            LandingDistance, //! Distance the ball travels to the target
            LandingTime = 0, //! The time it will take the ball to reach the target
            AirTime = 0,  //! Amount of time the ball has been in the air
            InitialVelocity; //<!Initial speed the the ball takes off with
        private float BallHeight, //<!Current vertical height of the ball
           frameRate = 1f / 60f; //!the current frame rate (this needs to be adjusted to a value the is given by the Unity Engine) 



        // Use this for initialization
        void Start()
        {
            Controls = FindObjectOfType<RaydraControls>(); //initialize the controls
            Rigidbody gr = this.GetComponent<Rigidbody>();
            Gravity = 40; //the drag will determine how a

            ThePlayer = FindObjectOfType<RaydraPlayerz>();

            X8Animator = ThePlayer.GetComponent<Animator>();
            StateInfo = X8Animator.GetCurrentAnimatorStateInfo(0);
            X8Animator.SetBool("ThrowBall", false);
            LocationOfTheCell = CellState.IsInHand;
            CellMoveState = CellMotionState.Neutral;
            // Update is called once per frame
        }


        void FixedUpdate()
        {

        
            //the ball is hand
            if(CellMoveState == CellMotionState.Neutral)
                if (LocationOfTheCell == CellState.IsInHand) //! If the ball in hand state is active then put the ball in the player hand
                    this.transform.position = CarryPoint.transform.position;


            //if the ball was throw from hand
            if (CellMoveState == CellMotionState.wasThrownFromHand)
                if (LocationOfTheCell == CellState.IsInAir) //! If the ball in hand state is active then put the ball in the player hand           
                    CellMovingTowardOtherPlayer();

            //if the ball was throw from hand
            if (CellMoveState == CellMotionState.wasShotFromWeapon)
                if (LocationOfTheCell == CellState.IsInAir) //! If the ball in hand state is active then put the ball in the player hand           
                    CellMovingTowardOtherGoal();

            //make sure that we are able to calculate the vector


            if (ThePlayer.AmInTheGoalZone)
            BallTravelPath = (ThePlayer.CurrentGoalTarget.transform.position - ThePlayer.ArmWeapon.transform.position) * .60f;


     


        }

            
        /*! The current location state of the Raydra Cell*/
        public CellState CurrentLocationOfCell
        {
            get { return LocationOfTheCell; }
            

        }

        /*! The current motion state of the Raydra Cell*/
        public CellMotionState CellMovementState
        {
            get { return CellMoveState; }


        }


        /** The animation state cell is currently in*/
        //!This medthod is supposed to initialize the ball at the moment it is drawn
        private void InitializeCellAtPointOfRelease()
        {
        
            if (LocationOfTheCell == CellState.IsInHand)
            {


                BallPosition = new Vector3(0, 0, 0); //! Current position of the ball

                AirTime = 0;
                OriginalBallPosition = this.transform.position;

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
                LocationOfTheCell = CellState.IsInAir;
            }
            //Change the Motion state to was thrown was from hand

            //the ball is not in the air

        }

        //Move the ball in the direction of the goal
        private void CellMovingTowardOtherGoal()
        {

            this.transform.position += BallTravelPath * .05f ; 

        }



        //!What happens to the ball when it is on the air
        private void CellMovingTowardOtherPlayer()
        {

          
                //!the ball height right at the moment
                BallHeight = (Gravity / -2f) * (AirTime * AirTime) + (InitialVelocity * AirTime);
                AirTime += frameRate; //amount of time the ball has been in the air
                CurrentBallPosition.x = BallPosition.x + OriginalBallPosition.x;
                CurrentBallPosition.z = BallPosition.z + OriginalBallPosition.z;
                CurrentBallPosition.y = BallPosition.y + BallHeight;
                BallPosition += BallDirection * (BallGroundSpeed / 60);//move the ball toward the player correct direction each frame
                                                                       //!WE DEVIDE THE BALLGROUND SPEED BY 60 TO GET HOW FAST THE BALL MOVES EVERY FRAME
                //!PlayerTwo.transform.position = NewOrginalBallPosition;
                this.transform.position = CurrentBallPosition;


               /* if ((AirTime > LandingTime))
                { //!if the ball has landed
                  
                    AirTime = 0;

                    BallPosition.z = 0;
                    BallPosition.x = 0;
                    this.transform.position = CarryPoint.transform.position ;
                    LocationOfTheCell = CellState.IsInHand;
                    ThePlayer.PlayerBallPossessionState = BallPossessionState.HasBall;

                }
                */

        }



        void OnTriggerEnter(Collider entity)
        {

      //      Debug.Log("ball Hit");

            //if the ball touched the player change the state of the cell
            if (entity!= null)
                if (entity.tag == "Player" || entity.tag == "InnerGoal"|| entity.tag == "Enemy") //if the cell collided with player Change the state of the cell
                    if (LocationOfTheCell == CellState.IsInAir || LocationOfTheCell == CellState.IsOnGround)
                    {
                        
                        X8Animator = entity.gameObject.GetComponent<Animator>();
                        LocationOfTheCell = CellState.IsInHand;
                        ThePlayer.PlayerBallPossessionState = BallPossessionState.HasBall;
                        CellMoveState = CellMotionState.Neutral; //ball is in the neutral position
                        if (ThePlayer.CheckPlayerThrowingBall() )
                            Debug.Log("Time is " + StateInfo.normalizedTime.ToString());
                    }

        }

        /*! will be called at the momemen the ball is thrown*/
        public void InitializeBallPosition()
        {
            this.transform.position = CarryPoint.transform.position;
            if (CellMoveState == CellMotionState.wasThrownFromHand)
             InitializeCellAtPointOfRelease();


        }

        public void BallWasThrown()
        {
            if(CellMoveState == CellMotionState.Neutral)
             CellMoveState = CellMotionState.wasThrownFromHand;
        }
  


        public void BallWasFired() {
            
            if (CellMoveState == CellMotionState.Neutral)
                CellMoveState = CellMotionState.wasShotFromWeapon;

            LocationOfTheCell = CellState.IsInAir;
        }


    } //end of CellActions Class

}//end of namespace




