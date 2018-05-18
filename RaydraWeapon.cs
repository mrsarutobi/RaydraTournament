using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RaydraTournament {

    //*! class RaydraWeaon this class handles all the data for player weapon*/
    public class RaydraWeapon : MonoBehaviour {
    public RaydraProjectile Bolt; /*!this is the object that will be created when the fire button is pressed
      
        
        Use this for initialization*/
        RaydraCellz TheBall;
    //    public RaydraProjectile TheBall;
        public RaydraProjectile LargeBolt;//!
        public bool WeaponFired = false;//!
        public GameObject CameraFocus; //where the camera is looking
                            //this when this time hits 1 second the weapon is charged
        public GameObject BoltSpawnPoint; //! Where the projectile is spawned or created at 
        List<RaydraProjectile> Bolts; //!this is array of currently created bolts
        Animator X8Animator; //!the animator that is attached to this object
        RaydraCharger WeaponCharger;
        RaydraPlayerz ThePlayer; //!reference to the RaydraPlayerz object that holds this RaydraWeapon object

    void Start () {
            Bolts = new List<RaydraProjectile>();

            for (int i = 0; i < Bolts.Count; ++i)
                Bolts[i] = null; //initialize the array elements to null 
          

            

        

            X8Animator = this.GetComponentInParent<Animator>();
            WeaponCharger = new RaydraCharger(1);

            //find the player object
            
            ThePlayer = GetComponentInParent<RaydraPlayerz>();
            X8Animator.SetBool("Shoot", false);
            X8Animator.SetBool("Phase", false);
            X8Animator.SetBool("Tackle", false);
            X8Animator.SetBool("WeaponCharged", false);

            TheBall = FindObjectOfType<RaydraCellz>();
        }

  
    // Update is called once per frame
    void FixedUpdate()
    {




            AnimatorStateInfo Layer1State = X8Animator.GetCurrentAnimatorStateInfo(1);


            for (int i = 0; i < Bolts.Count; ++i)
            {
            
                if (Bolts[i].IsEnabled== false || Bolts[i].HitSomething == true)//if the timer is off and hence beyond its age limit
                {
                    RaydraProjectile CurrentBolt = Bolts[i];//get the instance
                    Bolts.RemoveAt(i);
                    Destroy(CurrentBolt.gameObject);

                }
                else
                {
                    Bolts[i].transform.position += Bolts[i].TravelPath; //move the projecti
                    
                }
            }

            if (!X8Animator.GetBool("Phase") && !X8Animator.GetBool("Tackle"))//As long as the character if not phasing or tackling
            {//start of phase if check
                if (Input.GetButton("Shoot"))
                {

                    ThePlayer.ShootTimer.Reset(); //reset the shoot timer
                    ThePlayer.ShootTimer.TurnTimerOn();//turn on the shoot timer





                    if (X8Animator.GetBool("Shoot") == false)
                        X8Animator.SetBool("Shoot", true);

                    WeaponCharger.EnableCharger(); //turn on the charge timer
                    WeaponCharger.Charging();//charge the weapon

                    if (WeaponCharger.IsFullyCharged == true) { 
                    //when the weapon is charged
                        X8Animator.SetBool("WeaponCharged", true);
                 
                    }
                    else
                        X8Animator.SetBool("WeaponCharged", false);

                }
                else
                    X8Animator.SetBool("Shoot", false);


                if(Input.GetButtonUp("Shoot")) //when the button is released
                {                  
                   this.ShootWeapon((CameraFocus.transform.position - ThePlayer.MyCamera.transform.position).normalized);
                        if (WeaponCharger.IsFullyCharged == true) //reset the the charged state of the weapon
                        ThePlayer.ShootTimer.Reset(); //reset the shoot timer

                    WeaponCharger.ChargerReset(); //reset the weapon charger
                }

            } //end of phase of check


            if (Layer1State.IsName("Shoot"))
                X8Animator.SetLayerWeight(1, 1);//increase the weigt of the layer with the shoot animation
            else
            {
                /// ThePlayer.PlayerAttackState = AttackMode.NotFiringAbility;
                X8Animator.SetLayerWeight(1, 0);
                if (this.WeaponFired == true)
               {               
                   this.WeaponFired = false;
               }
            }

        }


        /*!shoot path is the direction of the that the projectile will travel in
          \param ShootPath the directon that projectile will move in when fired*/
        public void ShootWeapon(Vector3 ShootPath) { 
   
        if (Bolts != null && Bolts.Count < 100) { 
       
                RaydraProjectile Projectile=null;
                if (WeaponFired == false)
                {//if we have not fired the weapon


                    if (WeaponCharger.IsFullyCharged == false)
                    {// if the weapon is not charged fire the smaller bolt

                        Projectile = (RaydraProjectile)Instantiate(Bolt, BoltSpawnPoint.transform.position, BoltSpawnPoint.transform.rotation);
                        // Projectile = (RaydraProjectile)Instantiate(TheBall, TheBall.transform.position, TheBall.transform.rotation);
              
                    }   
                    else
                    {


                        if (!ThePlayer.ReadyToScore())
                        { //the player is not able to score


                            Projectile = (RaydraProjectile)Instantiate(LargeBolt, BoltSpawnPoint.transform.position, BoltSpawnPoint.transform.rotation);
                          
                            WeaponCharger.ChargerReset(); //reset the charger
                       
                        }
                        else
                        {
                            TheBall.BallWasFired();
                            ThePlayer.PlayerBallPossessionState = BallPossessionState.DoseNotHaveBall;
                            Projectile = null;
                        
                        }
                        X8Animator.SetBool("WeaponCharged", false);
                    }
                }
                    if (Projectile != null)
                    {
                        //shoot the larger bolt faster
                        if(Projectile.tag == "LargeBolt")
                            Projectile.Path = ShootPath ;
                        else
                            Projectile.Path = ShootPath * .45f;

                        //the bolt will live for 4.5 seconds
                            Projectile.LifeLimit = 4.5f;
                        //add the bolt to the list of projectiles
                        Bolts.Add(Projectile);
                     
                    }

                WeaponFired = true;


            }
   
             


        }
    
        //!This indicates if the weapom is charging or not
        public bool IsCharging
        {
            
            get { return WeaponCharger.IsCharging; }
        }
       
        //!this returns true if the weapon is fully charged
        public bool WeaponFullyCharged
            {

                get
                {
                    return WeaponCharger.IsFullyCharged;
                }

        }
        /*!resets the weapon charge to zero , effectivel stopping it*/
        public void StopWeapon()
        {
            WeaponCharger.ChargerReset();
        }

    }//end of RaydraWeapon Class
} //end of RaydraTournament Namespace
