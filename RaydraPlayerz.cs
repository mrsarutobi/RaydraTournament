using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using  BreadcrumbAi;

namespace RaydraTournament
{

    public class RaydraPlayerz : MonoBehaviour
    {
        //*! \class RaydraPlayerz this class controls all the data for the player


        //!reference to the player controls
        RaydraControls Controls;
        //! when the camera will look at
        public GameObject CameraFocus;
        private RaydraCellz TheBall;

        private bool ballReleased;

        //!where camera will focus above the player head
        public GameObject CameraFocusOverHead;
        //!Refrence the the current animator attached to the player
        Animator X8Animator;
        //! The current state of the animator
        AnimatorStateInfo StateInfo;
        bool hit = false, strafedRight = false, strafedLeft = false;

        float dampin;

        public GameObject Player; //!this is the player data that unity manipulates
        public MovingState PlayerMovementState; //!The current action that the player is taking at the moment
        public RaydraState PlayerRaydraState; //! Indicates wether or not the player is in Raydra mode or not
        public AttackMode PlayerAttackState; //! Determines wether the player is firing or not
        public JumpingState PlayerJumpState;//! The current jumping state of the player
        public CameraPerspective PlayerCameraState; //! what the camera is doing at the moment
        public BallThrowingState PlayerBallThrowState;//! the vertical motion of the player 
        public TackleState PlayerTackleState; //! indicates wether the player is tackling or not
        public TackeHitState PlayerTackleHitState; //! indicates what type of contact has been made
        public PhaseState PlayerPhaseState; //! indicates wether the player is tackling or not
        public PhaseHitState PlayerPhaseHitState; //! indicates what type of contact has been made
        public BallPossessionState PlayerBallPossessionState; //!indicate if the player has the ball or not
        private X8Timer innerEventTimer = new X8Timer(7f); ////! this private timer class will time the duration of certain actions
        private Camera _myCamera=null; //!This is the camera  that is connected to this player
        private Vector3 AccumForce; //! the force that is acculated on the player --this quantity is a sum a mulitple forece
        private bool isGrounded= false;//!private varible that is determining if the player is on solid ground or not
        private bool inGoalZone; //!private varible for determing if the player is on the goal zone or not
        protected RaydraShield innerShield = null;//!this is a reference to the shield that the player has attached to them
        private Vector3 _ballGoalVector;
        public Vector3 PlayerDirection = new Vector3(0, 0, 0);  //! the direction that the player is facton
                                                                // the angle that player is facing
        public Mesh PlayerModel; //! The actual model the player is using, will chnage based Raydra state
        public float Jumptime = 0; //!the time that player is in the air
        public float ForceOfGravity = 0; //!force of gracity in the vertical direction
        public float TackleForce = 3; //!this is Tackle force that is exerted on the player when he gets hit
        bool ScoreMode; //!This varible determines is we are in score mode
        private RaydraGoalPoint _GoalPoint;   //!This the Current Goal Target(private provariableperty) that player going to score on
        private RaydraWeapon _armWeapon; //private  propery for the arm weapon
                                         //**Initiializes all the data for the player*/

        private Ai EnemyAI; //!This is varianble that will handle the AI for the enemys
        public X8Timer TackleTimer = new X8Timer(3);
        public X8Timer ShootTimer = new X8Timer(.5f); //this will time how longer the player faces the recticle
        void Start()
        {
            innerShield = gameObject.GetComponentInChildren<RaydraShield>(); //!search for the sheild in the children of the parent object

            _myCamera = this.GetComponentInChildren<Camera>(); //get the camera that is connected to this game object
            bool ScoreMode = false; //initally we are not in score mode
            //!the player info that unity will keep tract of
            p_fCameraAngleTheta = 43f; //!initialize the angle that player if facing
            p_fCameraAnglePhi = 45f; //!The camera is initially yyyyin this state
            p_fPlayerToFocusAngleTheta = 43f; //!initialize the angle that player if facing
            p_fPlayerToFocusAnglePhi = 45f; //!//!initialize the angle that player if facing in the upward direction
            p_fJumpVelocity = .40f; //!initialize the upward jump force
            ForceOfGravity = 1f / 60f; //!this will have a unit per second force of gravity, in the downward direction 
            p_fOrientationAngle = 90f; //!Angle that the player is facing in degrees
            p_v3OrientationVect = new Vector3(0, 0, 1f); //!Direction player is facing represented as an angle
            p_v3PlayerToFocusVect = new Vector3(0, 0, 0); //!The vector the represents the direction the camera if facing
            p_v3PlayerToCameraVect = new Vector3(0, 0, 0);//!The Vector Data the represent the player to camera direction
            PlayerMovementState = MovingState.IsStandingStill; //!Intially the player is not moving
            PlayerJumpState = JumpingState.NotJumping; //!initlialize the the player is not jumping
            PlayerTackleHitState = TackeHitState.NoContact; //!Initial the tackle state of the player
            PlayerTackleState = TackleState.IsNotTackling; //!Initial the tackle state of the player
            PlayerAttackState = AttackMode.NotFiringAbility; //!Initial the shoot state of the player
            PlayerRaydraState = RaydraState.NotInRaydraMode; //!initailly the player is not in Raydra Mode
            PlayerCameraState = CameraPerspective.Normal; //!initially the camera to do nothing
            PlayerPhaseState = PhaseState.IsNotPhasing;
            PlayerBallThrowState = BallThrowingState.NotBeingThrown;  //!Initial the vertical state of the player
            PlayerBallPossessionState = BallPossessionState.HasBall; //!Initial the Possession State of the player                                                         //	PlayerMotionVector = new Vector3 (this.transform.position.x,.transform.position.y,playerData.transform.position.z);
            p_v3PlayerVectorDirection = Vector3.zero; //! This is direction that te player is headed as a vector
            _armWeapon = this.GetComponentInChildren<RaydraWeapon>(); //!reference to the arm weapon on the player which is null to begin with
            p_bFacePlayer = false; //! variable that determines weather or not to face the player, this variable determine weather or not this medthod is recursive
            p_fMass = 0f; //!initialize the mass of the player
            p_StateTime = 0f;// Time elasped in current state
            p_Collider = null; //initialize the first collider connect to this body
            pRidgidBody = null;


            Controls = GetComponent<RaydraControls>();
            if(CameraFocusOverHead!=null)
            p_v3PlayerToCameraVect = CameraFocusOverHead.transform.position - _myCamera.transform.position;
            p_fCameraAnglePhi = Vector3.Angle(new Vector3(0, 1, 0), p_v3PlayerToCameraVect);
            p_fCameraAngleTheta = Vector3.Angle(new Vector3(1, 0, 0), p_v3PlayerToCameraVect);
            p_fPlayerToFocusAnglePhi = Vector3.Angle(new Vector3(0, 1, 0), p_v3PlayerToFocusVect);
            p_fPlayerToFocusAngleTheta = Vector3.Angle(new Vector3(1, 0, 0), p_v3PlayerToFocusVect);
            X8Animator = GetComponent<Animator>();
            isGrounded = false; 
            if (p_v3PlayerToFocusVect.z < 0)
                p_fPlayerToFocusAngleTheta = 360 - p_fPlayerToFocusAngleTheta; //make sure that the angle is at the correct corridinate

            if (p_v3PlayerToCameraVect.z < 0)
                p_fCameraAngleTheta = 360 - p_fCameraAngleTheta; //make sure that the angle is at the correct corridinate

            TheBall = FindObjectOfType<RaydraCellz>();
            X8Animator = GetComponent<Animator>(); //get the reference to the animator

            PlayerMovementState = MovingState.IsStandingStill;
         
            EnemyAI = GetComponent<Ai>(); //get the ai script attached to the player (if there is one)

            if(EnemyAI!=null)
            Debug.Log("enemy AI" + EnemyAI.ToString());

            
      
        }
        /**!Move the player in the given direction
         \param LeftStickXAxis the x direction that the player will move in (supplied by the joy stick movement)
         \param LeftStickYAxis the y direction that the player will move in (supplied by the joy stick movement)
         \param velocity the velocity that the player will travel at
         \param strafDirection direction the direction the player will straf in*/
        public void MovePlayer(float LeftStickXAxis, float LeftStickYAxis, float velocity, float strafDirection)
        {

            //This is the Dert
            Vector3 DestinationVector = new Vector3(LeftStickXAxis, 0, LeftStickYAxis);

            p_fDestinationAngle = CentralHub.VectorToAngle(DestinationVector);

            //  CharacterDrawLine.SetPosition(1, this.transform.position + DestinationVector);
            p_fDestinationAngle += strafDirection;
            p_fDestinationAngle = CentralHub.AdjustAngle(p_fDestinationAngle);

            //Point direction is the direction that we want to head
            float ControllerDifference = p_fDestinationAngle - 90;


            Vector3 Destination = CameraFocus.transform.position - _myCamera.transform.position; //new Vector3 (CentralHub.COS[(int)(CameraDirection)], 0, CentralHub.SIN[(int)(CameraDirection)]);
            float angle = CentralHub.VectorToAngle(Destination);
            float angleFloatPart = 0;
            int IntegerAngle = 0;
            //add the offset that of where the controller is facing 
            angle += ControllerDifference;
            //make sure the angle is at the correct value
            angle = CentralHub.AdjustAngle(angle);
            IntegerAngle = (int)angle;
            angleFloatPart = angle - (int)angle;





            float arrayoffset = (CentralHub.COS.Offsetslookup[(int)(angleFloatPart * CentralHub.COS.IndexJump)]);


            p_v3PlayerVectorDirection = new Vector3(CentralHub.COS[IntegerAngle * CentralHub.COS.IndexJump + (int)(arrayoffset * CentralHub.COS.IndexJump)], 0, CentralHub.SIN[IntegerAngle * CentralHub.COS.IndexJump + (int)(arrayoffset * CentralHub.COS.IndexJump)]);
            // PlayerDirectionalData(1.5f * CentralHub.COS[(int)angle], 1.5f * CentralHub.SIN[(int)angle]);


            if (PlayerAttackState != AttackMode.IsFiringAbility && ShootTimer.IsEnabled == false)
            { 
                Debug.Log("Turning ");
            FaceThePlayer(angle, 0, p_fOrientationAngle);
            }

            this.transform.position += p_v3PlayerVectorDirection * velocity;
            CameraFocus.transform.position += p_v3PlayerVectorDirection * velocity;

        }

        /**!Move the player in the given direction
     \param MoveVector the vector direction to move the player in*/

    void MovePlayer(Vector3 MoveVector )
    {
            this.transform.position += MoveVector;
    }


    /**Make sure that our angles do not go out of bounds*/
        public void CheckAngles() //this medthods checks all the player to camera angles and camera to look at angles
        {                                               //it also makes sure the AdjustedAngle parameter warps around corrected and does not go negative or beyond 359
                                                        //check the camera to focus point angles
                                                        //check the horizontal angle
            if (p_fCameraAngleTheta > 359)
                p_fCameraAngleTheta = 0;

            if (p_fCameraAngleTheta < 0)
                p_fCameraAngleTheta = 359;

            //check the vertical angle
            if (p_fCameraAnglePhi > 359)
                p_fCameraAnglePhi = 359;

            if (p_fCameraAnglePhi < 0)
                p_fCameraAnglePhi = 359;

            //check the player to camera angles
            //check the horizontal angle
            if (p_fPlayerToFocusAngleTheta > 359)
                p_fPlayerToFocusAngleTheta = 0;

            if (p_fPlayerToFocusAngleTheta < 0)
                p_fPlayerToFocusAngleTheta = 359;

            //check the vertical angle
            if (p_fPlayerToFocusAnglePhi > 359)
                p_fPlayerToFocusAnglePhi = 0;

            if (p_fPlayerToFocusAnglePhi < 0)
                p_fPlayerToFocusAnglePhi = 359;

            if (p_fOrientationAngle < 0)//make sure that the angle wraps arounds properly
                p_fOrientationAngle = 359 + p_fOrientationAngle;


            //make sure the angle stays in the correct bounds

        }



        //!this is angle direction that the player is facing. Player Movement is relative
        public float p_fOrientationAngle
        {
            set;
            get;
        }
        //!The vector the player is facing
        public Vector3 p_v3OrientationVect
        {
            set;
            get;

        }
        //!the angle that the player is headed toward
        public float p_fDestinationAngle
        {
            set;
            get;

        }

        //!this is the direction that the player is moving in
        public Vector3 p_v3PlayerMotionVector
        {
            set;
            get;
        }
        //!this the horizontal angle of the camera relative to the focas point
        public float p_fCameraAngleTheta
        {
            set;
            get;
        }

        //!this is the vertical angle of the camera relative to the ground
        public float p_fCameraAnglePhi
        {
            set;
            get;
        }
        public float p_fPlayerToFocusAngleTheta  //!initialize the angle that player if facing
        {
            get;
            set;

        }
        public float p_fPlayerToFocusAnglePhi
        {
            get;
            set;
        }

        //!this the accerlation of the vertical velocity when the player is jumping
        public float p_fJumpVelocity
        {
            get;
            set;

        }

        //!this the direction the player is going at the moment
        public Vector3 p_v3PlayerVectorDirection
        {
            get;
            set;
        }


        public Vector3 p_v3PlayerToCameraVect
        {
            get;
            set;

        }

        //!this is a public property that indicates weather or not the player is touching solid ground
        public bool IsOnSolidGround
        {
            get { return isGrounded; }

        }


        //!this the vector that goes from the camera to the player
        public Vector3 p_v3PlayerToFocusVect
        {
            get;
            set;

        }

        /*!adds a force to the accumlated force this frame 
         * \param force this a the force vector that will be add to the player
          the x direction that the player will move in (supplied by the joy stick movement)*/
        public void addForce(Vector3 force)
        {

            AccumForce += force;
        }

        //!clears the forces that have added up in the player for this frame 
        public void clearForces()
        {

            AccumForce = Vector3.zero;

        }


        //!Reference to the player arm weapon
        public RaydraWeapon ArmWeapon
        {
            get
            {
               if (_armWeapon == null)
                    _armWeapon= this.GetComponentInChildren<RaydraWeapon>();

                return _armWeapon;
            }


    

        }
        //!Reference to rigid body attached to the player
        public Rigidbody pRidgidBody
        {
            get;
            set;

        }
        /*! if this is set to true, the player will ture toward the direction the camera is facing.
         another way to put  it :the player will face direction they will aim shoot in */
        public bool p_bFacePlayer
        {
            get;
            set;

        }
        //!the mass of the player
        public float p_fMass
        {
            get;
            set;
        }
        //!The amount of time the current state has been running for
        public float p_StateTime
        {
            get;
            set;
        }
        //!the collider that is attached to the player
        public Collider p_Collider
        {
            get;
            set;
        }
        //!Indicate weither or not the player has the bal or not
        public BallPossessionState PlayerBallState
        {
            //  get { return PlayerBallPossessionState; }
            get;
            set;


        }
        /*! helper function that call the FaceThePlayer function
          * \param angle the destionation angle that the player will face in
          *  \sa FaceThePlayer
          */
        public void FaceMe(float angle)
        {



            FaceThePlayer(angle, 0, p_fOrientationAngle);
        }

        /*!turn the player until they reached facing the desired destination in degrees 
          \param Destination the angle the player will turning toward
          \param Difference an offset that is using to face the player
          \param Origin where the player was orignally facing at the time this fuction is called
             */

        public void SetJumpAnimatorVariable()
        {
            X8Animator.SetBool("Jump", false);
        }


        public void FaceThePlayer(float Destination, float Difference, float Origin)//! The parameters of this medthod are as follows Destination is where we want to face, Difference is an offset for controller input , Origin is where te
        {

    
            //!heading caluclates the shortest rotation point to the desired rotation angle and also gives the acute (<= 180) difference between between the angles
            float heading = (Destination + Difference - Origin);
            //    if (heading == 0) { //if we are facing where we want to go

            //       p_bFacePlayer = false; //set face the player to false
            //      return;
            //   }
            //make sure that we get the shortest rotation point
            if (heading < -180) heading += 360;
            if (heading > 180) heading -= 360;

            for (; heading != 0;)
            { //start of big if



                if (heading > 0)
                {


                    if (Math.Abs(heading) >= 5)
                    {

                        this.transform.Rotate(0, -5, 0);
                        p_fOrientationAngle += 5;
                    }
                    else
                    {

                        this.transform.Rotate(0, -5, 0);
                        p_fOrientationAngle += 5;
                    }


                    if (p_fOrientationAngle > 359)//make sure that the angle wraps arounds properly
                        p_fOrientationAngle = p_fOrientationAngle - 360;


                }
                else if (heading < 0)
                {
                    if (Math.Abs(heading) >= 5)
                    {


                        this.transform.Rotate(0, 5, 0);
                        p_fOrientationAngle -= 5;
                    }
                    else
                    {
                        this.transform.Rotate(0, -heading, 0);
                        p_fOrientationAngle += heading;
                    }

                    if(Controls != null) //if this is a real player an not an Enemy AI
                    Controls.CheckAngles();

                }
                heading = (Destination + Difference - p_fOrientationAngle);
                if (heading < -180) heading += 360;
                if (heading > 180) heading -= 360;


            }//end of for loop

            // if (p_bFacePlayer == true)
            //  FaceThePlayer(Destination, 0, p_fOrientationAngle);



        }

        //!check to see if the the player is strafing either right 
        public bool CheckStrafeRight()
        {
            if (StateInfo.IsName("StrafeRight"))
                return true;
            return false;

        }
        //!check to see if the the player is strafing either  left
        public bool CheckStrafeLeft()
        {
            if (StateInfo.IsName("StrafeLeft"))
                return true;
            return false;

        }

        //!check to see if the the player is strafing either right or left
        public bool CheckStrafe()
        {

            //we might have starfed during a jump so stop the jump animation


            if (StateInfo.IsName("StrafeLeft"))
            {
                PlayerMovementState = MovingState.IsStrafing;
                if (PlayerJumpState == JumpingState.IsJumping)
                    StopJump();

                StateInfo = X8Animator.GetCurrentAnimatorStateInfo(0);
                FaceThePlayer(p_fCameraAngleTheta, 0, p_fOrientationAngle);

                if (StateInfo.normalizedTime > .001f && StateInfo.normalizedTime < .50f)
                {

                    FaceMe(p_fCameraAngleTheta);
                    if (Controls != null) //if this is a real player an not an Enemy AI
                        MovePlayer(Controls.CapturedLeftSrafeVector*.75f); //move th player in the left direction


                    return true;
                }
                else
                    PlayerMovementState = MovingState.IsStandingStill;


                if (StateInfo.normalizedTime > .50f)
                {
                    strafedLeft = true;
                    X8Animator.SetBool("StrafeLeft", false);

                }
          


            }

            if (StateInfo.IsName("StrafeRight"))
            {
                if (PlayerJumpState == JumpingState.IsJumping)
                    StopJump();
                PlayerMovementState = MovingState.IsStrafing;

                StateInfo = X8Animator.GetCurrentAnimatorStateInfo(0);

                FaceMe(p_fCameraAngleTheta);

                if (StateInfo.normalizedTime > .001f && StateInfo.normalizedTime < .50f)
                {

                    FaceMe(p_fCameraAngleTheta);
                    PlayerMovementState = MovingState.IsStrafing;
                    if (Controls != null) //if this is a real player an not an Enemy AI
                        MovePlayer(Controls.CapturedRightSrafeVector*.75f); //move th player in the left direction



                    return true;
                }
                else
                    PlayerMovementState = MovingState.IsStandingStill;

                //  X8Animator.SetBool("StrafeRight", true);
                if (StateInfo.normalizedTime > .50f)
                {
                    X8Animator.SetBool("StrafeRight", false);
                    strafedRight = true;

                }
       


            }

            return false;

        }


        //!check to see if we are currently in phase strike mode
        public bool CheckPhaseStrike()
        {

            AnimatorStateInfo StateInfo2 = X8Animator.GetCurrentAnimatorStateInfo(0);




            if (StateInfo2.IsName("Phase"))
            {
                //we might have starfed during a jump so stop the jump animation
                if (PlayerJumpState == JumpingState.IsJumping)
                    StopJump();


                pRidgidBody.useGravity = false;
                p_bFacePlayer = true;

                X8Animator.SetBool("Phase", true);


                if (StateInfo2.normalizedTime > .01f && StateInfo2.normalizedTime < .65f)
                {
                    Vector3 TackleDirection=Vector3.zero;

                    p_Collider.enabled = false;
                    FaceMe(p_fCameraAngleTheta);
                    //player is in tackle mode
                    //the Direction of attack is the same as the direction of fire without the vertical direction
                    if (Controls != null) //if this is a real player an not an Enemy AI
                     TackleDirection = Controls.ShootDirection.normalized;
                    //zero out the y direction because the we dont want any vertical movement
                    TackleDirection.y = 0;
                    //add the tackle vector the player position
                    PlayerPhaseState = PhaseState.IsPhasing;
                    if (StateInfo2.normalizedTime > .30f && StateInfo2.normalizedTime < .65f)
                        this.transform.position += (TackleDirection * .75f);
                    return true;
                }
                else
                {

                    p_Collider.enabled = true;
                    pRidgidBody.useGravity = true;

                    PlayerPhaseState = PhaseState.IsNotPhasing; //player is no longer in tackle mode

                    if (StateInfo2.normalizedTime > .99f)
                        X8Animator.SetBool("Phase", false);
                    return false;
                }

            }

            return false;


        } //!stop phase mode
        void StopPhaseStrike()
        {
            X8Animator.SetBool("Phase", false);
            PlayerPhaseState = PhaseState.IsNotPhasing;
        } //stops the phase ability





        //!check to see if the player is in phase strike mode
        public bool CheckTackle()
        {


            if (StateInfo.IsName("Tackle"))
            {

                //we might have Tackled during a jump so stop the jump animation
                if (PlayerJumpState == JumpingState.IsJumping)
                    StopJump();

                if (hit == true&& StateInfo.normalizedTime > .30f)
                    X8Animator.SetBool("Tackle", false);



                if (StateInfo.normalizedTime > .65)
                    if (hit == true)
                        hit = false; //reset the hit

                if (StateInfo.normalizedTime > .01f && StateInfo.normalizedTime < .65 && X8Animator.GetBool("Tackle")== true)
                {//&& StateInfo.normalizedTime < .65 )
                 
                    FaceMe(p_fCameraAngleTheta);
                    // Debug.Log("hit  is" + hit.ToString());

                    //player is in tackle mode
                    //the Direction of attack is the same as the direction of fire without the vertical direction
                    Vector3 TackleDirection = Vector3.zero;

                    if (hit == false) { 
          
                        if (EnemyAI == null) //if this is a real player an not an Enemy AI
                           TackleDirection = Controls.ShootDirection.normalized * .45f;
                        else
                        {                        
                            //with enemy AI the vector will be from the enemy to the player
                            TackleDirection = (EnemyAI.Player.position - this.transform.position).normalized * .45f;

                        }
                    }
                    //zero out the y direction because the we dont want any vertical movement

                    TackleDirection.y = 0;
                    //add the tackle vector the player position
                    PlayerTackleState = TackleState.IsTackling;
                    //  Debug.Log("Tackle:  " + TackleDirection.normalized.ToString());
                    if (StateInfo.normalizedTime > .30f && StateInfo.normalizedTime < .65f)
                        this.transform.position += (TackleDirection.normalized * .55f);

                    return true;
                }
                else
                {
                    PlayerTackleState = TackleState.IsNotTackling; //player is no longer in tackle mode
                    if (StateInfo.normalizedTime > .99f)
                        X8Animator.SetBool("Tackle", false);

                    return false;
                }

            }


            return false;

        }


        void CheckedChargedRun()
        {

            if (X8Animator.GetBool("ChargedRun") == true)
            {
                EventTimer.IncrementTimer();

                if (EventTimer.IsEnabled == false)
                {

                    X8Animator.SetBool("ChargedRun", false);
                    EventTimer.Reset();
                }


            }

        }
        //!checks to see if the player is tackling
        void StopTackle()
        {
            X8Animator.SetBool("Tackle", false);
            PlayerTackleState = TackleState.IsNotTackling;

        }


        //stops the phase ability
        //!checks if to see if the player is currenty jumping





        public bool IsJumping()
        {
            if (StateInfo.IsName("JumpUp"))
            {
                PlayerJumpState = JumpingState.IsJumping;
                isGrounded = false;

                if (StateInfo.normalizedTime > .86f)
                    X8Animator.SetBool("Jump", false);
                else
                    X8Animator.SetBool("Jump", true);

                return true;
            }


            PlayerJumpState = JumpingState.NotJumping;
            return false;
        }
        //!stop the shooting of the weapon
        void StopShoot() { X8Animator.SetBool("Shoot", false); } //stops the phase ability
                                                                 //!stop the player from strafing
        void StopStrafe()
        {
            if (X8Animator.GetBool("StrafeLeft") == true)
            { //stop the left strafe

                strafedLeft = true;
                X8Animator.SetBool("StrafeLeft", false);
            }

            if (X8Animator.GetBool("StrafeRight") == true)
            { //stop the left strafe

                strafedRight = true;
                X8Animator.SetBool("StrafeRight", false);
            }

            PlayerMovementState = MovingState.IsStandingStill;



        }

        public void StopJump()
        {
            //make sure that we are in the jump animation
            if (X8Animator.GetBool("Jump") == true)
                X8Animator.SetBool("Jump", false);
            //make sure that we are in the jump state
            if (PlayerJumpState == JumpingState.IsJumping)
                PlayerJumpState = JumpingState.NotJumping;

        }


        //!checks to see if the player is shooting
        public bool CheckShoot()
        {
            AnimatorStateInfo Layer2State = X8Animator.GetCurrentAnimatorStateInfo(1);
            if (Layer2State.IsName("Shoot"))
            {
                dampin = .55f;//slow down the rotation player movement for accuate shooting
                PlayerAttackState = AttackMode.IsFiringAbility;
                //if we are ready to score focus on goal point
                if (ReadyToScore()) //
                {
                    
                   // _myCamera.transform.LookAt(CurrentGoalTarget.transform.position);
                }
                else
                    ScoreMode = true;

                float angle = CentralHub.VectorToAngle(CameraFocus.transform.position - this.transform.position);
                angle = CentralHub.AdjustAngle(angle);
                FaceMe(angle);
        
            }
            else
            {
                PlayerAttackState = AttackMode.NotFiringAbility;
                dampin = 1; //resume normal shooting
            }


            return false;
        } //end of medthod
        //!Checks to see if the player is in the process of throwing the ball
        public bool CheckPlayerThrowingBall()
        {
            if (StateInfo.IsName("ThrowBall"))
            {

                if (PlayerJumpState == JumpingState.IsJumping)
                    StopJump();


                if (StateInfo.normalizedTime > .70f)
                {
                    X8Animator.SetBool("ThrowBall", false);
                    return false;
                }
                return true;

            }

            return false;
        }




        public void FixedUpdate()
        {



            if (X8Animator != null)
            {
                StateInfo = X8Animator.GetCurrentAnimatorStateInfo(0);
                CheckPhaseStrike();
                CheckTackle();
                CheckShoot();
                CheckStrafe();
                CheckPlayerThrowingBall();
                CheckedChargedRun();
            }


            ReadyToScore();
            if (EnemyAI != null)
                EnemyAI_Movement();

            if (TackleTimer.IsEnabled)
                TackleTimer.IncrementTimer();

            if (ShootTimer.IsEnabled)
            {
                ShootTimer.IncrementTimer();

                float angle = CentralHub.VectorToAngle(CameraFocus.transform.position - this.transform.position);
                angle = CentralHub.AdjustAngle(angle);
              FaceMe(angle); //face the player in the direction he will shoot for short timer
             //   PlayerAttackState = AttackMode.IsFiringAbility; //we are shooting

            }


        }
        //!collision response when the player tackles another player
        void OnCollisionStay(Collision collision)
        {

            Vector3 v3ShootDirection= Vector3.zero;

            

             StateInfo = X8Animator.GetCurrentAnimatorStateInfo(0);
   
            if (collision.rigidbody != null && (collision.rigidbody.tag == "Player" || collision.rigidbody.tag == "Enemy"))
                if (PlayerTackleState == TackleState.IsTackling)
                { //if were are in tackle mode                    Debug.Log("go it");



                        if (EnemyAI == null) //if this is a real player an not an Enemy AI
                            v3ShootDirection = Controls.ShootDirection; //use the controller direction
                        else                                             //use the vector from the enemy to the target
                            v3ShootDirection = (collision.rigidbody.transform.position - EnemyAI.transform.position).normalized;

                    v3ShootDirection.Normalize();//lets get the absolute direction


                        PlayerTackleHitState = TackeHitState.HasHitOpponent; //you have made contact with another player                                                                         // collision.rigidbody.AddForce(new Vector3(p_v3PlayerVectorDirection.x * 1500, 1000, p_v3PlayerVectorDirection.z * 1500), ForceMode.Acceleration);
                        hit = true;

                    //get a reference to the player that was hit
                    RaydraPlayerz AttackedPlayer  = collision.gameObject.GetComponent<RaydraPlayerz>();
                    if (AttackedPlayer) //make sure the search is successfull
                    {
                        if (AttackedPlayer.PlayerTackleState == TackleState.IsTackling)
                        {
                            Debug.Log("in the zone");
                            AttackedPlayer.StopTackle();//cancel the tackle  the other player
                            StopTackle(); //cancel the tackle of this specific player
                        }
                           else
                            collision.rigidbody.AddForce(new Vector3(v3ShootDirection.x * 2100, 150, v3ShootDirection.z * 2100), ForceMode.Acceleration);


                    }



              

                    if (StateInfo.IsName("Phase"))
                    {
                        Debug.Log("hit yah phase");
                        if (collision.rigidbody != null)
                            if (collision.rigidbody.tag == "Goal")
                            {

                                PlayerPhaseHitState = PhaseHitState.HasHitOpponent;//you have made contact with another player

                            }

                    }
                }


       

            if (collision.collider != null)
            {

                if (collision.collider.tag == "Ground")
                    isGrounded = true;

            }

        }

     
        //!collision response when the player tackles another player
        void OnCollisionStsay(Collision Entity)
        {
            //check if the player is currently colliding with solid ground
     




        }

        void OnTriggerEnter(Collider Entity)
        {


            if (Entity.tag == "GoalZone")
            {
                _GoalPoint = null;
                inGoalZone = true;
          
                _GoalPoint = Entity.GetComponent<RaydraGoalZone>().GoalPoint; //make sure we get the goal point for the goal that we a currently 

                Debug.Log("The goal is " + _GoalPoint.ToString());
            }


        }

        void OnTriggerExit(Collider Entity)
        {

            if (Entity.tag == "GoalZone")
                inGoalZone = false;

       

        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.collider.tag == "Ground")
                isGrounded = false; //when we leave ground set the is ground property to false

            if (collision.collider.isTrigger)
            {
                if (collision.collider.tag == "GoalZone")
                    inGoalZone = false;

            }
        }
        public void PlayerStrafing()
        {

        }

        /*!This is an medthod that will be called from the animator controller object
         It changes the possesion state of the player when it is call*/
        public void BallThrown()
        {
            TheBall.BallWasThrown(); //make sure the ball knows it was thrown
            TheBall.InitializeBallPosition();//initialize the postion of the ball before it is thrown
            PlayerBallPossessionState = BallPossessionState.DoseNotHaveBall;
            PlayerBallThrowState = BallThrowingState.NotBeingThrown;
        }

        public void AiTackle()
        {
  


         

        }

        /*!This is an medthod that will be called from the animator controller object
     It changes the possesion state of the player when it is call*/
        public void IsThrowingBall()
        {
            PlayerBallThrowState = BallThrowingState.IsBeingThrown; //make sure the ball knows it was thrown
            PlayerBallPossessionState = BallPossessionState.DoseNotHaveBall;
        }

        //!Player is able to score in the goal
        public bool ReadyToScore()
        {
            if (Controls != null) //if this is a real player an not an Enemy AI
                if (AmInTheGoalZone && Controls.AimingAtTheGoal() && PlayerBallPossessionState == BallPossessionState.HasBall)
                return true;

            //  Camera.main.transform.LookAt(GoalPoint.transform.position); //focus on goalpoint
            //   float angle = CentralHub.VectorToAngle(GoalPoint.transform.position - this.transform.position);
            // angle = CentralHub.AdjustAngle(angle);
            // FaceMe(angle);


            return false;


        }
        //The means 
        //!Instuct the AI how to react to movement situations
        void EnemyAI_Movement()
        {
            if (EnemyAI.moveState == Ai.MOVEMENT_STATE.IsFollowingPlayer)
                X8Animator.SetFloat("Speed", .6f);
            else
            {
                EnemyAI_Attack();
                X8Animator.SetFloat("Speed", 0f);
            }
        }


        void EnemyAI_Attack()
        {
       
            if (EnemyAI.attackState == Ai.ATTACK_STATE.CanAttackPlayer )
            {
                if (TackleTimer.IsEnabled == false)
                    TackleTimer.TurnTimerOn();

                if (PlayerTackleState == TackleState.IsNotTackling)
                {
                    if (TackleTimer.AgeinSeconds < Time.deltaTime) //if the timer has been on for no more than one frame this simulates a button press
                        {
                  //      Debug.Log("legomania");
                        X8Animator.SetFloat("Speed", 0f);
                        X8Animator.SetBool("Tackle", true);
                    }
          
                }
            }
       


        }
        void InScoreMode()
        {


        }


        public bool ballWasReleased
        {
            get { return ballReleased; }
        }

        public X8Timer EventTimer
        {
            get { return innerEventTimer; }

        }
        //! this timer class will time the duration of certain actions

        //!property for indicating if the player is on the goal zone or not
        public bool AmInTheGoalZone
        {
            get { return inGoalZone; }
        }

        public Vector3 BallGoalVector
        {
            get { return _ballGoalVector; }
        }
        //!This the Current Goal Target(public property) that player going to score on
        public RaydraGoalPoint CurrentGoalTarget
        {
            get {
                return _GoalPoint;

            }

        }

        //!This the camera object that is attached to the player(public property) that player going to score on
        public Camera MyCamera
            {
            get {
                if (_myCamera == null)
                    _myCamera = this.GetComponentInChildren<Camera>();
                return _myCamera;
             }

        }
            //!This is for object that is going to be scored on
    }// end of class
}//end of namespace
