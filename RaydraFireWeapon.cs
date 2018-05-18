using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RaydraTournament
{ 
    /**
\class RaydraCell
        */
 public class RaydraFireWeapon : MonoBehaviour {
    public RaydraProjectile Bolt; /*!<this is the object that will be created when the fire button is pressed
      
        
        Use this for initialization*/

        public RaydraProjectile LargeBolt; //!The bolt that is fired when the weapon is full charged( Test only)
        public bool WeaponFired = false;//! indicate wither or not the weapon is fired
        public GameObject CameraFocus; //!where the camera is looking
        bool WeaponCharged; //!the state of the weapon charge
        X8Timer ChargeTimer; //!this when this time hits 1 second the weapon is charged

    public GameObject BoltSpawnPoint; //! Where the projectile is spawned or created at 
    List<RaydraProjectile> Bolts; //!this is array of currently created bolts
  //  List<Vector3> Paths;
    Animator X8Animator; //the animator that is attached to this object
       

    void Start () {
        Bolts = new List<RaydraProjectile>();

        for (int i = 0; i < Bolts.Count; ++i)
        {
            Bolts[i] = null; //initialize the array elements to null 
        
        }

        

            X8Animator = this.GetComponentInParent<Animator>();
            ChargeTimer = new X8Timer(1f);

            //assign a medthod to be called once the time limit is reached
            //TimeLimitReached is a delegate
            ChargeTimer.TimeLimitReached = WeaponIsCharged;

            //set the initial weapon charge
            WeaponCharged = false;
            X8Animator.SetBool("Shoot", false);

     
        }

    // Update is called once per frame
    void Update()
    {
            AnimatorStateInfo Layer1State = X8Animator.GetCurrentAnimatorStateInfo(1);


            for (int i = 0; i < Bolts.Count; ++i)
            {
            
                if (Bolts[i].IsTicking== false)//if the timer is off and hence beyond its age limit
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


            if (Input.GetButton("Shoot"))
            {         
                if (X8Animator.GetBool("Shoot") == false)
                    X8Animator.SetBool("Shoot", true);

                ChargeTimer.TurnTimerOn(); //turn on the charge timer
                ChargeTimer.Update();


            }
            else
            {
              
                X8Animator.SetBool("Shoot", false);
              

            }

            if (Input.GetButtonUp("Shoot")) {
                //fire the weapon upon the release of the the button
                this.ShootWeapon(CameraFocus.transform.position - Camera.main.transform.position);
                if(WeaponCharged == true) //reset the the charged state of the weapon
                   WeaponCharged = false;

                ChargeTimer.Reset(); //reset the charge time
            }

            if (Layer1State.IsName("Shoot")) {

                X8Animator.SetLayerWeight(1, 1);//increase the weigt of the layer with the shoot animation
         
            }
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
\param ShootPath this is the direction that the project will move in when fired*/

        public void ShootWeapon(Vector3 ShootPath)

        {
        if (Bolts != null && Bolts.Count < 100)
        {
                RaydraProjectile Projectile;
                if (WeaponFired == false)
                {
                
                    if(WeaponCharged == true) //the weapon is charge fire the Larger bolt
                       Projectile = (RaydraProjectile)Instantiate(LargeBolt, BoltSpawnPoint.transform.position, BoltSpawnPoint.transform.rotation);
                    else
                    Projectile = (RaydraProjectile)Instantiate(Bolt, BoltSpawnPoint.transform.position, BoltSpawnPoint.transform.rotation);
                    Projectile.Path = ShootPath * .875f;
                    Projectile.LifeLimit = 4.5f;
                    Bolts.Add(Projectile);
                    WeaponFired = true;
                }
               

       
        }

    }

        /*!Sets the weapon charge to true*/
        public void WeaponIsCharged()
        {
            WeaponCharged = true;
        }

        /*!booleen value that is set to true when the weapon is fully charged*/
        public bool FullyCharged
            {

            get
            {
                return WeaponCharged;
            }

        }

        /*!Stops the weapon*/
        public void ResetWeapon()
        {
            ChargeTimer.Reset();
        }

}//end of RaydraFireWeapon Class
    } //end of Namespace
