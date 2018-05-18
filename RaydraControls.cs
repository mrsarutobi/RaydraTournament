		
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;




namespace RaydraTournament
{

    /*! \class RaydraControls 
      this class handles the controls for the player
     
         */
    public class RaydraControls : MonoBehaviour
    {

        //! All the data of player
        private RaydraPlayerz ThePlayer;
        //!Reference to another player 
        public GameObject TheOtherGuy; 

        public GameObject CameraOrientationDrawLine; //!Test data showing the direction of the camera
        public GameObject PlayerOrientationDrawLine; //!Test data showing the current direction the player is facing

        //  public GameObject PlayerToTargetDr; //This is also for testing purpp
        public GameObject CameraFocus; //!this the point the camera will look at
        public GameObject CameraFocusOverHead; // //!The camera will wil gradually move to this point when its when vertical position is over a certain height

        private Vector3 PlayerDrawLineVector; //!The vector data of the direction the player is facing
        private Vector3 CameraToPlayerSpring; /*!this vector determines when the player should pull the camera*/

        private float PlayerToFocusLength; //!This the length of the camera to Player Vector
        private float PlayerToCameraLength; //!The is the length of the player to camera direction
        LineRenderer CharacterDrawLine; //!The actual player direction line renderer
        LineRenderer CameraDrawLine;//!The actual camera direction line renderer

        Animator X8Animator; //! Reference to the animator about connect to the player
        AnimatorStateInfo StateInfo; //! Current state of the animator connected to the player


        private float originalX, originalY, originalZ;

        private Vector3 v3ShootDirection;//!The direction that player is going to shoot in
        public Canvas X8TestCanvas;//!The screen that targeting rectilce is drawn on

        public GameObject GoalSphere; //!THe actual object that the player needs to hit with to score a point
        private GameObject ShootPoint; //!The pointer where a players projectile is fired from
        private RaydraCellz TheBall;
       
        CentralHub Hub;
        //These are in the initial values that the Left joy sticl vertical and horizontal positions are 
        //set to, These are the controllers neutral position values
        float InitialHorizontalControllerPosition1, InitialVerticalControllerPosition1, InitialStrafeAxisValue;
        float InitialHorizontalControllerPosition0, InitialVerticalControllerPosition0, InitialRightFlipperValue, InitialLeftFlipperValue;
        float CapturedJoyX, CapturedJoyY;


        Vector3 CapturedLeft, CapturedRight, CapturedDirection, BallTravelPath;
   
        float newPlayerToCameraFactor;

       //float CapturedJoyStkX, CapturedJoyStkY;
        private bool strafedLeft = false, strafedRight = false; //this tells us if we have strafed or not
        private float turnSpeed=.25f, dampin=1; //this slows down the rotation when shooting for more accuracy
        bool hit = false, LeftPressureReset = true, RightPressureReset = true;
        RaycastHit[] hits; //!the number of objects the targeting raycaster hit


       float CameraPhiOffest= 0;
        float  PlayerPhiOffset =0;
        float CameraThetaOffest = 0;
        float PlayerThetaOffset = 0;

        /** Start the initialization of variable */
        void Start()
        {
            TheBall = FindObjectOfType<RaydraCellz>(); //find the RaydraCell in the scene editor
        GameObject ControlHub = GameObject.Find("ControlHub");

            ShootPoint =GameObject.FindGameObjectWithTag("ShootPoint");
           // GoalSphere =GameObject.FindGameObjectWithTag("GoalPoint");


            //Get the line rendered that is attached to this game object
            LineRenderer[] CameraDrawLineArray = CameraOrientationDrawLine.GetComponents<LineRenderer>();
            LineRenderer[] PlayerDrawLineArray = PlayerOrientationDrawLine.GetComponents<LineRenderer>();
            //OrientationDrawLine = OrientationDrawLineArray[0];
            //GetComponent<Animator>() returns and array, so we are just grabbing the first element
            X8Animator = this.GetComponents<Animator>()[0];


            if (CameraOrientationDrawLine == null)//return null if there is problem
                return;

            //if(CameraDrawLine == null

            CharacterDrawLine = PlayerDrawLineArray[0];
            CharacterDrawLine.SetPosition(0, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));
            CharacterDrawLine.SetPosition(1, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1));


            CameraDrawLine = CameraDrawLineArray[0];
            CameraDrawLine.SetPosition(0, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));
            CameraDrawLine.SetPosition(1, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1));



            CharacterDrawLine.SetWidth(.05f, .05f);

            CameraDrawLine.SetWidth(.05f, .05f);

            Hub = ControlHub.GetComponent<CentralHub>();
            ThePlayer = GetComponent<RaydraPlayerz>(); //new RaydraPlayer (this.gameObject);

            ThePlayer.MyCamera.transform.LookAt(CameraFocus.transform.position, CameraFocus.transform.up);
            //initial the camera to the focus point vector
            PlayerDrawLineVector = ThePlayer.p_v3PlayerToFocusVect = ThePlayer.MyCamera.transform.position - CameraFocus.transform.position;

            //initialize the camera to the player
            ThePlayer.p_v3PlayerToCameraVect = CameraFocusOverHead.transform.position - ThePlayer.MyCamera.transform.position;


            //get the length of the camera to focus point vector
            PlayerToFocusLength = Math.Abs(ThePlayer.p_v3PlayerToFocusVect.magnitude);

            //get the length of player to the camera
            PlayerToCameraLength = Math.Abs(ThePlayer.p_v3PlayerToCameraVect.magnitude);

            ThePlayer.p_fCameraAnglePhi = Vector3.Angle(new Vector3(0, 1, 0), ThePlayer.p_v3PlayerToCameraVect);
            ThePlayer.p_fCameraAngleTheta = Vector3.Angle(new Vector3(1, 0, 0), ThePlayer.p_v3PlayerToCameraVect);

            ThePlayer.p_fCameraAnglePhi = (int)ThePlayer.p_fCameraAnglePhi;
            ThePlayer.p_fCameraAngleTheta = (int)ThePlayer.p_fCameraAngleTheta;

            ThePlayer.p_fPlayerToFocusAnglePhi = (int)Vector3.Angle(new Vector3(0, 1, 0), ThePlayer.p_v3PlayerToFocusVect);
            ThePlayer.p_fPlayerToFocusAngleTheta = (int)Vector3.Angle(new Vector3(1, 0, 0), ThePlayer.p_v3PlayerToFocusVect);



            if (ThePlayer.p_v3PlayerToFocusVect.z < 0)
                ThePlayer.p_fPlayerToFocusAngleTheta = 360 - ThePlayer.p_fPlayerToFocusAngleTheta; //make sure that the angle is at the correct corridinate

            if (ThePlayer.p_v3PlayerToCameraVect.z < 0)
                ThePlayer.p_fCameraAngleTheta = 360 - ThePlayer.p_fCameraAngleTheta; //make sure that the angle is at the correct corridinate



            //The player orientation or direction the player is walking  is always the direction that the camera is looking.
            PlayerDrawLineVector.y = originalY;
            PlayerDrawLineVector.x = originalX;
            PlayerDrawLineVector.z = originalZ;

            InitialHorizontalControllerPosition1 = Input.GetAxis("RightHorizontal"); //give the initial Horizontal poistion 
            InitialVerticalControllerPosition1 = Input.GetAxis("RightVertical");
            InitialHorizontalControllerPosition0 = Input.GetAxis("LeftHorizontal"); //give the initial Horizontal poistion 
            InitialVerticalControllerPosition0 = Input.GetAxis("RightVertical");
            

            X8Animator.SetFloat("Speed", 0);
            X8Animator.SetBool("Jump", false);
            X8Animator.SetBool("RunJump", false);
            X8Animator.SetBool("Tackle", false);
            X8Animator.SetBool("Shoot", false);
            X8Animator.SetBool("ThrowBall", false);
            X8Animator.SetBool("ChargedRun", false);
            X8Animator.SetBool("StrafeRight", false);
            X8Animator.SetBool("StrafeLeft", false);
            X8Animator.SetBool("WeaponCharged", false);

            StateInfo = X8Animator.GetCurrentAnimatorStateInfo(0);

            ThePlayer.pRidgidBody = GetComponent<Rigidbody>();

            ThePlayer.p_Collider = GetComponent<CapsuleCollider>();//get a reference to the first collider connected to this object

            turnSpeed = 2;

            InitialStrafeAxisValue = .16f;

            InitialRightFlipperValue = -.05f; //this is initail value of the right pressure button
            InitialLeftFlipperValue = InitialStrafeAxisValue ; //this is initail value of the right pressure button


            ThePlayer.MyCamera.transform.LookAt(CameraFocus.transform.position, Vector3.up);

        }

        //! Update is called once per frame
        public void FixedUpdate()
        {
            turnSpeed = CentralHub.COS.CosOffSet* turnSpeed;

            float LeftStickXAxis, LeftStickYAxis, RightStickXAxis, RightStickYAxis;

            LeftStickXAxis = Input.GetAxis("LeftHorizontal");
            LeftStickYAxis = Input.GetAxis("LeftVertical");


            LeftStickYAxis *= -1;

            RightStickXAxis = Input.GetAxis("RightHorizontal");
            RightStickYAxis = Input.GetAxis("RightVertical");

            ControlInputData(LeftStickXAxis, LeftStickYAxis, RightStickXAxis, RightStickYAxis);


            Vector3 PlayerToTargetVector = new Vector3(TheOtherGuy.transform.position.x - this.transform.position.x,
                   0, TheOtherGuy.transform.position.z - this.transform.position.z);

            Vector3 To = new Vector3(CentralHub.COS[(int)ThePlayer.p_fDestinationAngle], 0, CentralHub.SIN[(int)ThePlayer.p_fDestinationAngle]);

            float val = Vector3.Angle(To, PlayerToTargetVector);

            float DestinationAngle = Vector3.Angle(new Vector3(1, 0, 0), PlayerToTargetVector);

            if (PlayerToTargetVector.z < 0)
                DestinationAngle = 360 - DestinationAngle;

            // Ray  ShootRay = ThePlayer.MyCamera.ScreenPointToRay(new Vector3(ThePlayer.MyCamera.pixelWidth, ThePlayer.MyCamera.pixelHeight, 0));
            v3ShootDirection = CameraFocus.transform.position - ThePlayer.MyCamera.transform.position;

            float fShootDirection = Vector3.Angle(new Vector3(1, 0, 0), v3ShootDirection);

            if (v3ShootDirection.z < 0)
                fShootDirection = 360 - fShootDirection;

            v3ShootDirection = CameraFocus.transform.position - ThePlayer.MyCamera.transform.position;


            StateInfo = X8Animator.GetCurrentAnimatorStateInfo(0);//get current state of the animator
            AnimatorStateInfo Layer2State = X8Animator.GetCurrentAnimatorStateInfo(1);
            if (Input.GetButtonDown("Throw"))
            {
         
                if (ThrowReady()) { //&& !ThePlayer.CheckPlayerThrowingBall()){

            
                        X8Animator.SetBool("ThrowBall", true);
                
                        ThePlayer.FaceThePlayer(ThePlayer.p_fCameraAngleTheta, 0, ThePlayer.p_fOrientationAngle);
                    }
            }

            
            if (Input.GetButton("Tackle") && X8Animator.GetBool("Shoot")==false )
            {

                if (TackleReady())
                {
                    X8Animator.SetBool("Tackle", true);
                 
                    ThePlayer.FaceThePlayer(ThePlayer.p_fCameraAngleTheta, 0, ThePlayer.p_fOrientationAngle);
                    CapturedJoyX = CentralHub.COS[(int)ThePlayer.p_fOrientationAngle]; //Input.GetAxis("LeftHorizontal");
                    CapturedJoyY = CentralHub.SIN[(int)ThePlayer.p_fOrientationAngle]; ;// Input.GetAxis("LeftVertical");
                }

            }


                if (Input.GetButtonDown("Jump") && X8Animator.GetBool("Shoot") == false) {



                if (JumpReady()) { //if the player is not already jumping and is on solid ground
             
                    
                        X8Animator.SetBool("Jump", true);
                        ThePlayer.pRidgidBody.AddForce(new Vector3(0, 700, 0), ForceMode.Acceleration);
                    }
                 

                }



            //if we are not phase striking

            if (X8Animator.GetBool("Shoot"))
                    ThePlayer.PlayerAttackState = AttackMode.IsFiringAbility;
                else
                    ThePlayer.PlayerAttackState = AttackMode.NotFiringAbility;


            if (Input.GetButton("Phase") && X8Animator.GetBool("Shoot") == false)
            {

                if (PhaseReady())   // activate phase only if the weapon is not charged
                   {
                        X8Animator.SetBool("Phase", true);

                        ThePlayer.FaceThePlayer(ThePlayer.p_fCameraAngleTheta, 0, ThePlayer.p_fOrientationAngle);
                        CapturedJoyX = LeftStickXAxis; //Input.GetAxis("LeftHorizontal");
                        CapturedJoyY = LeftStickYAxis;// Input.GetAxis("LeftVertical");
                    }

                if (ThePlayer.ArmWeapon.WeaponFullyCharged == true)
                {
                    X8Animator.SetBool("ChargedRun", true);//if weapon is charged and phase is pressed actitvate charged run
                    ThePlayer.EventTimer.TurnTimerOn(); //Enable the timer
                   // ThePlayer.PlayerMovementState = MovingState.IsChargedRunning;
                }


            }


            if (StrafeLeftInInitialPostion() )
            {
                if (StrafeLeftReady()){
                        X8Animator.SetBool("StrafeLeft", true);

                        //ThePlayer.FaceThePlayer(ThePlayer.p_fCameraAngleTheta, 0, ThePlayer.p_fOrientationAngle);
                        CapturedJoyX = LeftStickXAxis; //Input.GetAxis("LeftHorizontal");
                        CapturedJoyY = LeftStickYAxis;// Input.GetAxis("LeftVertical");
                        CapturedLeft = RotateAngleCreateVector(ThePlayer.p_fCameraAngleTheta, 90);


                }
            }


        
            if (StrafeRightInInitialPostion() )
            {

                if (StrafeRightReady())
                    {
                        X8Animator.SetBool("StrafeRight", true);
                        //ThePlayer.PlayerMovementState = MovingState.IsStrafing;
                        //ThePlayer.FaceThePlayer(ThePlayer.p_fCameraAngleTheta, 0, ThePlayer.p_fOrientationAngle);
                        CapturedJoyX = LeftStickXAxis; //Input.GetAxis("LeftHorizontal");
                        CapturedJoyY = LeftStickYAxis;// Input.GetAxis("LeftVertical");
                        CapturedRight =  RotateAngleCreateVector(ThePlayer.p_fCameraAngleTheta, -90);

                }
            }



            //   Debug.Log("Right flipper value : " + Input.GetAxisRaw("StrafeAxis").ToString());
            //reset the strafed state when the right strafe button is released
            if (Input.GetAxisRaw("StrafeAxis") < .16f && Input.GetAxisRaw("StrafeAxis") > -.01f && strafedRight == true)
                strafedRight = false; //reset the strafed state

            //reset the strafed Left state when the left strafe button is released
            if (Input.GetAxisRaw("StrafeAxis") > 0f && Input.GetAxisRaw("StrafeAxis") < .16f && strafedLeft == true)
                strafedLeft = false; //reset the strafed state




            //  Debug.Log("Weaponm is charged:" + ThePlayer.ArmWeapon.WeaponFullyCharged.ToString());
            // Debug.Log("TAckle state " + ThePlayer.PlayerTackleState.ToString());
            //Debug.Log("TAckle hit state " + ThePlayer.PlayerTackleHitState.ToString());
            //  Debug.Log("Player is" + ThePlayer.PlayerMovementState.ToString());
            //   
            /**!!!!!!!!!DONT NOT DELETE THIS!!!!!!!!!!!!!!!!!!!!!!************
            if (X8Animator.GetBool("ChargedRun") == true){

                ThePlayer.EventTimer.IncrementTimer();
  
                if(ThePlayer.EventTimer.IsEnabled == false)
                { 
                    X8Animator.SetBool("ChargedRun", false);
                    ThePlayer.EventTimer.Reset();
                }


            }
            */
            // check the value of the pressure buttons
            CheckPressureButtonReset();
     

                 MoveCamera();

            if (ThePlayer.ArmWeapon.IsCharging)
                turnSpeed = 8f;
            else
                turnSpeed = 5f;








        }
        //!Medthod indicates that the player is capable of stafing right
        public bool StrafeRightReady()
        {
            // if(strafedRight == false && X8Animator.GetBool("StrafeRight") == false && !InOtherAction())

         

            if (!X8Animator.GetBool("StrafeLeft")  && !InOtherAction()&& !StrafeLeftHeldDown())
                    return true;

            return false;
        }
        //!Medthod indicates that the player is capable of stafing left
        public bool StrafeLeftReady()
        {
            
                // if(strafedleft == false && X8Animator.GetBool("StrafeLeft") == false && !InOtherAction())
                if (!X8Animator.GetBool("StrafeRight")  && !InOtherAction() && !StrafeRightHeldDown() )
                return true;

            return false;
        }
        //!Medthod indicates that the player is capable Phase Attacking
        public bool PhaseReady()
        {
            if (X8Animator.GetBool("WeaponCharged") == false && !InOtherAction() && X8Animator.GetBool("Phase") == false)
                return true;
            return false;
        }

        //!Medthod indicates that the player is capable of throwing
        public bool ThrowReady() {

            if (Input.GetButtonDown("Throw") && !InOtherAction() && ThePlayer.PlayerBallPossessionState == BallPossessionState.HasBall)
                return true;
            
            return false;
        }
        //!Medthod indicates that the player is capable of jumping
        public bool JumpReady()
        {
            if(!ThePlayer.IsJumping() && ThePlayer.IsOnSolidGround && !InOtherAction())
                return true;

            return false;
        }
        //!Medthod indicates that the player is capable of tackling
        public bool TackleReady()
        {
            if (X8Animator.GetBool("Tackle") == false && !InOtherAction())
                return true;

                return false;
        }
   
       //!  this called when a player tackles another player
        public void TackleCollision()
        {

            Vector3 v3TackleDirecion = new Vector3(CapturedJoyStkX,0,CapturedJoyStkY).normalized ;

            //Debug.Log("get down");
            Collider[] colliders = Physics.OverlapCapsule( this.transform.position + new Vector3(0, 1, 0),  this.transform.position + new Vector3(0, 1, 0), 3f);

            if (colliders.Length > 0)
            {

                //check the colliders 
                foreach (Collider c in colliders) { 
                 //if we are in tackle mode
                  // Debug.Log("length " + colliders.Length.ToString());

                    //Debug.Log("the tag " + c.tag.ToString());
                    if (c.tag == "Sphere")
                        if (ThePlayer.PlayerTackleState == TackleState.IsTackling) { 
                        
                            c.GetComponent<Rigidbody>().AddForce(new Vector3(v3ShootDirection.x * 510, 330, v3ShootDirection.z * 510), ForceMode.Acceleration);
                            hit = true; //there was a hit
                        }
                    }

            }

             colliders = null;
        }

        void LatsffseUpdate()
        {

            MoveCamera();
            TackleCollision();
            StartCoroutine(Tiger());

        }
        //! Delays excecution of an action until after the FixedUpdate() medthod is called
      public  IEnumerator Tiger()
        {
            yield return new WaitForFixedUpdate();

            TackleCollision();

        }


        /*! This medthods reads and directs joystick movement
         *  \param LeftStickXAxis X component of the left joystick movement
            \param LeftStickYAxis Y component of the left joystick movement
            \param RightStickXAxis X component of the right joystick movement
            \param RightStickYAxis Y component of the right joystick movement
            */
        private void ControlInputData(float LeftStickXAxis, float LeftStickYAxis, float RightStickXAxis, float RightStickYAxis)
        {


            CharacterDrawLine.SetPosition(0, this.transform.position);
            CameraDrawLine.SetPosition(0, this.transform.position);
    //if there is not movement in either axis
                if (RightStickYAxis == 0 && RightStickXAxis==0)
                {
                    ThePlayer.PlayerCameraState = CameraPerspective.Normal;
                    X8Animator.SetFloat("Speed", 0f);

                }
           
            if (RightStickYAxis > -.13f && RightStickYAxis !=0)
                {
    
                    // keep the camea with in a upper bound
                    if (ThePlayer.p_fPlayerToFocusAnglePhi < 144)
                    {
                    ThePlayer.PlayerCameraState = CameraPerspective.IsRotating;

                    CameraPhiOffest -= turnSpeed; //increment the angle offset
                        PlayerPhiOffset += turnSpeed; //increment the angle offset
                    }

                }
       
                if ((RightStickYAxis < .13f && RightStickYAxis != 0))
            {
    

                //  Debug.Log("tunring..");
                // keep the camea with in a lower bound
                if (ThePlayer.p_fPlayerToFocusAnglePhi > 21f)
                    {


                        newPlayerToCameraFactor = 1;


                        ThePlayer.PlayerCameraState = CameraPerspective.IsRotating;

                        CameraPhiOffest += turnSpeed;
                        PlayerPhiOffset -= turnSpeed;
                    }

                }

                if (RightStickXAxis < -.13f && RightStickXAxis != 0)
            {
            

                ThePlayer.PlayerCameraState = CameraPerspective.IsRotating;
                    //  ThePlayer.p_fCameraAngleTheta += (turnSpeed * dampin);
                    //  ThePlayer.p_fPlayerToFocusAngleTheta += (turnSpeed * dampin);
                    CameraThetaOffest += turnSpeed;
                    PlayerThetaOffset += turnSpeed;

                }


                if ((RightStickXAxis > .13f && RightStickXAxis != 0))
            {
              
                ThePlayer.PlayerCameraState = CameraPerspective.IsRotating;

                    CameraThetaOffest -= turnSpeed;
                    PlayerThetaOffset -= turnSpeed;

                    //  Debug.Log("turning ");

                }
            
           


            CheckAngles(); //make sure none of the angles go out of bounds

            float velocity = .25f;

            if (X8Animator.GetBool("ChargedRun") == true)
                velocity = .75f;


            if (ControllerIsMoving(LeftStickXAxis, LeftStickYAxis) && !InOtherAction() )
            {
       
                float Speed = Math.Abs(LeftStickYAxis) + Math.Abs(LeftStickXAxis);
                if (Speed > .10f)
                {

                    ThePlayer.PlayerMovementState = MovingState.IsRunning;

                   if( X8Animator.GetBool("ChargedRun")== false)
                        ThePlayer.PlayerMovementState = MovingState.IsRunning;
                   else
                        ThePlayer.PlayerMovementState = MovingState.IsChargedRunning;


                    X8Animator.SetFloat("Speed", .70f);
                    ThePlayer.MovePlayer(LeftStickXAxis, LeftStickYAxis, velocity, 0);

                }
            }

            if (!ControllerIsMoving(LeftStickXAxis, LeftStickYAxis) && !InOtherAction())
            {
                X8Animator.SetFloat("Speed", 0);
                ThePlayer.PlayerMovementState = MovingState.IsStandingStill;
            }
            


            //Line Direction is the direction that the camera is facing
            float LineDirection = ThePlayer.p_fCameraAngleTheta;
            //float CameraDirection = LineDirection ;
            if ((int)LineDirection > 359)
                LineDirection -= 360;
            else
                if (LineDirection < 0)
                LineDirection += 360;
            Vector3 v3CamDir = CameraFocus.transform.position - ThePlayer.MyCamera.transform.position;

            CameraDirectionalData(v3CamDir.x, v3CamDir.z);

          //  Debug.Log("Camera theta " + CameraThetaOffest.ToString());
        //    Debug.Log("Player theta " + PlayerThetaOffset.ToString());
            // SwingTheCamera(ang);


            
        }

        public bool InOtherAction()//! this medthod checks if the player is in action i.e Tackling, Phase Attack
        {
            if (ThePlayer.PlayerTackleState == TackleState.IsTackling ) //player is  tacking
                return true;

            if (ThePlayer.PlayerMovementState == MovingState.IsStrafing)// //player is  strafing
                return true;

            if (ThePlayer.PlayerPhaseState == PhaseState.IsPhasing)//player is  phasing
                return true;

            if(ThePlayer.PlayerBallThrowState == BallThrowingState.IsBeingThrown)
                return true;

       
            return false;

        }
        /*!this medthod rotates the camera when the player move the joystick
         \param PositiveRotation this parameter does not do anything anymore 
         it need
             */
        private void MoveCamera()
        {

            Vector3 tmpPlayerToCamera = new Vector3(0, 0, 0);
            Vector3 tmpPlayerToFocus = new Vector3(0, 0, 0);

            CheckAngles(); //make sure that camera and focus angles dont not go out of bounds
        
                //   Debug.Log("index " + (ThePlayer.p_fCameraAngleTheta * 4 + (int)(CameraThetaOffest * 4)).ToString());
                // Debug.Log("angle " + ThePlayer.p_fCameraAngleTheta.ToString());
                //   Debug.Log("offset " + CameraThetaOffest.ToString());

                tmpPlayerToCamera.x = PlayerToCameraLength * CentralHub.COS[(int)ThePlayer.p_fCameraAngleTheta * CentralHub.SIN.IndexJump + (int)(CameraThetaOffest * CentralHub.SIN.IndexJump)] * CentralHub.SIN[(int)ThePlayer.p_fCameraAnglePhi * CentralHub.SIN.IndexJump + (int)(CameraPhiOffest * CentralHub.SIN.IndexJump)];
                tmpPlayerToCamera.y = PlayerToCameraLength * CentralHub.COS[(int)ThePlayer.p_fCameraAnglePhi * CentralHub.SIN.IndexJump + (int)(CameraPhiOffest * CentralHub.SIN.IndexJump)];
                tmpPlayerToCamera.z = PlayerToCameraLength * CentralHub.SIN[(int)ThePlayer.p_fCameraAngleTheta * CentralHub.SIN.IndexJump + (int)(CameraThetaOffest * CentralHub.SIN.IndexJump)] * CentralHub.SIN[(int)ThePlayer.p_fCameraAnglePhi * CentralHub.SIN.IndexJump + (int)(CameraPhiOffest * CentralHub.SIN.IndexJump)];//(oldZ * CosComponent - oldX * SinComponent) 

                tmpPlayerToFocus.x = PlayerToFocusLength * CentralHub.COS[(int)ThePlayer.p_fPlayerToFocusAngleTheta * CentralHub.SIN.IndexJump + (int)(PlayerThetaOffset * CentralHub.SIN.IndexJump)] * CentralHub.SIN[(int)ThePlayer.p_fPlayerToFocusAnglePhi * CentralHub.SIN.IndexJump + (int)(PlayerPhiOffset * CentralHub.SIN.IndexJump)];
                tmpPlayerToFocus.y = PlayerToFocusLength * CentralHub.COS[(int)ThePlayer.p_fPlayerToFocusAnglePhi * CentralHub.SIN.IndexJump + (int)(PlayerPhiOffset * CentralHub.SIN.IndexJump)];
                tmpPlayerToFocus.z = PlayerToFocusLength * CentralHub.SIN[(int)ThePlayer.p_fPlayerToFocusAngleTheta * CentralHub.SIN.IndexJump + (int)(PlayerThetaOffset * CentralHub.SIN.IndexJump)] * CentralHub.SIN[(int)ThePlayer.p_fPlayerToFocusAnglePhi * CentralHub.SIN.IndexJump + (int)(PlayerPhiOffset * CentralHub.SIN.IndexJump)];





                ThePlayer.p_v3PlayerToCameraVect = tmpPlayerToCamera;//* newPlayerToCameraFactor;
                ThePlayer.p_v3PlayerToFocusVect = tmpPlayerToFocus;

                PlayerDrawLineVector = ThePlayer.p_v3PlayerToCameraVect;
                PlayerDrawLineVector.y = 0; //get rid of y plane we dont need up and down motion for the direcion the player walks
                PlayerDrawLineVector.Normalize(); /*!give us the absolute direction that the player is facing	
											this will always be in the XZ(ground) plane and in the direction of
											 camera angle theta */


            //as long as we player is not ready to score
            ThePlayer.MyCamera.transform.position = CameraFocusOverHead.transform.position - ThePlayer.p_v3PlayerToCameraVect;
            CameraFocus.transform.position = ThePlayer.MyCamera.transform.position - ThePlayer.p_v3PlayerToFocusVect;


            ThePlayer.MyCamera.transform.LookAt(CameraFocus.transform.position, CameraFocus.transform.up);// CameraFocus.transform.up);






        }
        // this medthod is for testing data is used to show where the character is facing 
        //and intend to go

        /*! This medthod move both the camera the camera and the player*/
        public void TackleAction()
{
Vector3 move = new Vector3(CapturedJoyStkX, 0, CapturedJoyStkY);
this.transform.position += new Vector3(CentralHub.COS[(int)ThePlayer.p_fOrientationAngle] * (1 / 45f), 0, CentralHub.SIN[(int)ThePlayer.p_fOrientationAngle] * (1 / 45f));
CameraFocus.transform.position += new Vector3(CentralHub.COS[(int)ThePlayer.p_fOrientationAngle] * (1 / 45f), 0, CentralHub.SIN[(int)ThePlayer.p_fOrientationAngle] * (1 / 45f));

            ThePlayer.MyCamera.transform.position = CameraFocus.transform.position + ThePlayer.p_v3PlayerToFocusVect;
}

        /*! This medthod swings the camera as the player moves and keep the camera perspective looking right during movement
         * neither of the parameter given are used in the medthod
       *  \param CameraDestination destination that the camera is headed in
       *  \param RightStickMovement the movement the of the right joystick 
        */
 public void SwingTheCamera(float CameraDestination, float RightStickMovement)
{
float heading = ThePlayer.p_fDestinationAngle + 90 - ThePlayer.p_fCameraAngleTheta;

    if (ThePlayer.p_fCameraAnglePhi != ThePlayer.p_fDestinationAngle) {
        if (ThePlayer.p_fCameraAnglePhi < 89) {
            ThePlayer.p_fCameraAnglePhi += CentralHub.COS.CosOffSet * 8 ;
            MoveCamera();
        }
    }

}

 /*! This medthods take 2 parameters and draws a line  in the direction that the player is facing
 \param LeftStickXAxis X component of the left joystick movement
 \param LeftStickYAxis Y component of the left joystick movement*/
        private void CameraDirectionalData(float LeftStickXAxis, float LeftStickYAxis)
{

Vector3 DestinationVector = new Vector3(LeftStickXAxis, 0, LeftStickYAxis);
CameraDrawLine.SetPosition(1, this.transform.position + DestinationVector);

}

 /*! This medthods take 2 parameters and draws a line in the direction that the player is facing
 \param LeftStickXAxis X component of the left joystick movement
 \param LeftStickYAxis Y component of the left joystick movement*/
private void PlayerDirectionalData(float LeftStickXAxis, float LeftStickYAxis)
{

 Vector3 DestinationVector = v3ShootDirection;// new Vector3(LeftStickXAxis, 0, LeftStickYAxis);
CharacterDrawLine.SetPosition(1, this.transform.position + DestinationVector);


}



  //!this medthods checks all the player to camera angles and camera to look at angles */

public void CheckAngles() 
{                                               //it also makes sure the AdjustedAngle parameter warps around corrected and does not go negative or beyond 359
                                                //check the camera to focus point angles
            float upperSinDegree = 1 - CentralHub.SIN.SinOffSet; //this the  highest decimal value 
            float upperCosDegree = 1 - CentralHub.COS.CosOffSet;
            //a degree becomes before it comes the next degree up
            //check the horizontal angle


            if (CameraPhiOffest < 0)
            {
                CameraPhiOffest =  upperSinDegree;
                --ThePlayer.p_fCameraAnglePhi;

            }

            if (CameraPhiOffest > upperSinDegree)
            {
                CameraPhiOffest = 0;
                ++ThePlayer.p_fCameraAnglePhi;
            }

            if (PlayerPhiOffset < 0)
            {
                PlayerPhiOffset = upperSinDegree;
                --ThePlayer.p_fPlayerToFocusAnglePhi;
            }
            if (PlayerPhiOffset > upperSinDegree)
            {
                PlayerPhiOffset = 0;
                ++ThePlayer.p_fPlayerToFocusAnglePhi;

            }

            if (CameraThetaOffest < 0)
            {
                CameraThetaOffest = upperCosDegree;
                --ThePlayer.p_fCameraAngleTheta;

            }

            if (CameraThetaOffest > upperCosDegree)
            {
                CameraThetaOffest = 0;
                ++ThePlayer.p_fCameraAngleTheta;
            }


            if (PlayerThetaOffset < 0)
            {
                PlayerThetaOffset = upperCosDegree;
                --ThePlayer.p_fPlayerToFocusAngleTheta;
            }
            if (PlayerThetaOffset > upperCosDegree)
            {
                PlayerThetaOffset = 0;
                ++ThePlayer.p_fPlayerToFocusAngleTheta;
            }



if (ThePlayer.p_fCameraAngleTheta > 359 )
    ThePlayer.p_fCameraAngleTheta = 0;

if (ThePlayer.p_fCameraAngleTheta < 0)
    ThePlayer.p_fCameraAngleTheta = 359;

//check the vertical angle
if (ThePlayer.p_fCameraAnglePhi > 359)
    ThePlayer.p_fCameraAnglePhi = 0;

if (ThePlayer.p_fCameraAnglePhi < 0)
    ThePlayer.p_fCameraAnglePhi = 359;

//check the player to camera angles
//check the horizontal angle
if (ThePlayer.p_fPlayerToFocusAngleTheta > 359)
    ThePlayer.p_fPlayerToFocusAngleTheta = 0;

if (ThePlayer.p_fPlayerToFocusAngleTheta < 0)
    ThePlayer.p_fPlayerToFocusAngleTheta = 359;

//check the vertical angle
if (ThePlayer.p_fPlayerToFocusAnglePhi > 359)
    ThePlayer.p_fPlayerToFocusAnglePhi = 0;

if (ThePlayer.p_fPlayerToFocusAnglePhi < 0)
    ThePlayer.p_fPlayerToFocusAnglePhi = 359;

if (ThePlayer.p_fOrientationAngle < 0)//make sure that the angle wraps arounds properly
    ThePlayer.p_fOrientationAngle = 360 + ThePlayer.p_fOrientationAngle;



            //make sure the angle and angle-offsets stays in the correct bounds

        }


 /*! this medhtod determines if there is joytick movement or not
 \param LeftStickXAxis X component of the left joystick movement
 \param LeftStickYAxis Y component of the left joystick movement*/
bool ControllerIsMoving(float LeftStickXAxis, float LeftStickYAxis)
{
return (LeftStickXAxis != 0 || LeftStickXAxis != InitialHorizontalControllerPosition0 && LeftStickYAxis != 0 || LeftStickYAxis != InitialVerticalControllerPosition0);

}
//*!This medthid check to see if the goal is being targeted(target recticle is on the goal) */
 public bool AimingAtTheGoal()
        {   //will indicate if we hit the goal or not
            bool HitGoal = false;
            hits = null;
            //create a ray that points at the center of the screen 
            Ray ray = ThePlayer.MyCamera.ScreenPointToRay(new Vector3(ThePlayer.MyCamera.pixelWidth / 2, ThePlayer.MyCamera.pixelHeight / 2, 0));
            int layerMask = 1 << LayerMask.NameToLayer("Goals");
            //store all the collitions that the raycaster hit
            hits = Physics.RaycastAll(ray, 1000, layerMask);
            if(hits!=null){ //check all object that were hit by the raycaster
                foreach (RaycastHit hit in hits)
                    if (hit.collider != null)
                        if (hit.collider.tag == "InnerGoal"){
                            //set the hit value to goal to true to indicate that the raycast hit the goal
                            HitGoal = true;
                        }
            }

        

            if (HitGoal)
            {
               
               // BallTravelPath = (ThePlayer.CurrentGoalTarget.transform.position - ThePlayer.ArmWeapon.transform.position) * .60f;


            }
        
          
            return HitGoal;
       


        }


        

//!x captured direction when certain buttons are pressed
public float CapturedJoyStkX 
{

get { return CapturedJoyX; }
}


//!direction captured when certain buttons are pressed
public float CapturedJoyStkY
{

get { return CapturedJoyY; }
}

        //!the direction of the left strafe vector at the moment the left strafe button is pressed
        public Vector3 CapturedLeftSrafeVector
        {

            get { return CapturedLeft; }
        }


        //!the direction of the right strafe vector at the moment the right strafe button is pressed
        public Vector3 CapturedRightSrafeVector
        {

            get { return CapturedRight; }
        }


        //! the current direction that the player will shoot in
        public Vector3 ShootDirection
{
get  { return v3ShootDirection; }
}
        //! the current direction that the ball will travel in
        public Vector3 CapturedBallTravelPath
        {
   get { return ThePlayer.CurrentGoalTarget.transform.position - ThePlayer.transform.position; }
 }

        //! the goal point the player is able to score in
        private RaydraGoalPoint CurrentGoalTarget
        {
            set ;
            get;
        }


        //!return true if the left pressure is in the initial position
        bool StrafeLeftInInitialPostion()
        {
        return(    StrafeLeftHeldDown() && !ThePlayer.CheckStrafeLeft() && (X8Animator.GetBool("StrafeLeft") == false) && LeftPressureReset == true);
        
        }

        bool StrafeRightInInitialPostion()
        { 


        
            return (StrafeRightHeldDown() && !ThePlayer.CheckStrafeRight() &&( X8Animator.GetBool("StrafeRight") == false) && RightPressureReset == true);

        }

        //!returns true if the left straf axis is being pushed beyond a certain amount
        bool StrafeLeftHeldDown()
        {



            return (Input.GetAxisRaw("StrafeAxis") >= InitialLeftFlipperValue);    
      }

        //!returns true if the right straf axis is being pushed beyond a certain amount
        bool StrafeRightHeldDown()
        {


            return (Input.GetAxisRaw("StrafeAxis") <= InitialRightFlipperValue);

      }

        //this medthod resest the pressure buttons variables if they are not being pressed beyond certain values
       void CheckPressureButtonReset()
            {


            if ((Input.GetAxisRaw("StrafeAxis") < InitialLeftFlipperValue) && (Input.GetAxisRaw("StrafeAxis") >= 0))
                LeftPressureReset = true;
            else
                LeftPressureReset = false;

            if ((Input.GetAxisRaw("StrafeAxis") > InitialRightFlipperValue) && (Input.GetAxisRaw("StrafeAxis") < InitialStrafeAxisValue))
                RightPressureReset = true;
            else
                RightPressureReset = false;


    

            return;
            }

        //! this medthod takes in an angle parameter, rotates by the degress parameter, and returns the vector version of the rotated angle
        Vector3 RotateAngleCreateVector(float oldangle,int degrees)
        {
            Vector3 RoatedAngleVector = Vector3.zero; //create a vector
            float angle = CentralHub.AdjustAngle(oldangle + degrees); //rotated the old passed angle by degrees and make sure the angle wrap correctly
            RoatedAngleVector.x = PlayerToCameraLength * CentralHub.COS[(int)angle * CentralHub.SIN.IndexJump + (int)(CameraThetaOffest * CentralHub.SIN.IndexJump)] * CentralHub.SIN[(int)ThePlayer.p_fCameraAnglePhi * CentralHub.SIN.IndexJump + (int)(CameraPhiOffest * CentralHub.SIN.IndexJump)];
            RoatedAngleVector.y = 0;// PlayerToCameraLength * CentralHub.COS[(int)ThePlayer.p_fCameraAnglePhi * CentralHub.SIN.IndexJump + (int)(CameraPhiOffest * CentralHub.SIN.IndexJump)];
            RoatedAngleVector.z = PlayerToCameraLength * CentralHub.SIN[(int)angle * CentralHub.SIN.IndexJump + (int)(CameraThetaOffest * CentralHub.SIN.IndexJump)] * CentralHub.SIN[(int)ThePlayer.p_fCameraAnglePhi * CentralHub.SIN.IndexJump + (int)(CameraPhiOffest * CentralHub.SIN.IndexJump)];//(oldZ * CosComponent - oldX * SinComponent) 

            return RoatedAngleVector.normalized; //return the absolute direction

        }

} //end of the class
}//end of namespace

