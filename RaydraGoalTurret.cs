using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaydraTournament {

    public class RaydraGoalTurret : MonoBehaviour
    {
        /*! \class RaydraGoalTurrent
         The class the presets the turret that will defend the goal
             
             */ 


        //!<this is array of currently created bolts
        List<RaydraProjectile> Bolts;
        //! this is what the goal turrets is shooting at
        public GameObject Target;
        //! This is what the tower will shoot at the player
        public RaydraProjectile Bullet;
        //! the point bullet is created at
        public GameObject BulletSpawnPoint;
        //! the spot where the scoring takes place
        private GameObject Center;
        //! The timer that determines the timing of the bullet from the turret
        private X8Timer ShootTimer;
        //! A boolean value that determine weather or not to fire a bullet
        private bool FireTurret = false;
      
     
        // Use this for initialization
        void Start()
        {
            ShootTimer = new X8Timer(1f);
            Bolts = new List<RaydraProjectile>();

            for (int i = 0; i < Bolts.Count; ++i)
                Bolts[i] = null; //initialize the array elements to null 

            Center = GameObject.Find("GoalCenter");
         
            ShootTimer.TimeLimitReached = TimerLimitReached;


        }

        // Update is called once per frame
        void Update()
        {
            Vector3 DistanceToCenter;
            DistanceToCenter = Target.transform.position-this.transform.position ;
            DistanceToCenter.y = 0; //get rid of the vertical component

            if (FireTurret == true)
            {
                ShootWeapon(DistanceToCenter.normalized);
                FireTurret = false;
            }



            for (int i = 0; i < Bolts.Count; ++i)
            {

                if (Bolts[i].IsEnabled == false)//if the timer is off and hence beyond its age limit
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


            //Debug.Log("Center is" + DistanceToCenter.magnitude.ToString());
         //   Vector3 DistanceToTarget= Target - Th;
        }
        /*!shoot path is the direction of the that the projectile will travel in
         \param ShootPath this is the direction that the project will move in when fired*/
        public void ShootWeapon(Vector3 ShootPath)
        {
            //  ;
            Vector3 DistanceToCenter=  Target.transform.position - this.transform.position;
            //  DistanceToCenter.y = 0; //get rid of the vertical component
            //  if (DistanceToCenter.magnitude <= 20f)
            

                if (Bolts != null && Bolts.Count < 100)
                {
                    RaydraProjectile Projectile;

                    Projectile = (RaydraProjectile)Instantiate(Bullet, BulletSpawnPoint.transform.position, BulletSpawnPoint.transform.rotation);

                    Projectile.Path = ShootPath * .875f;
                    Projectile.LifeLimit = 4.5f;
                    Bolts.Add(Projectile);


                }

            }


        /*!Called when the timer limit is reached*/
        public void TimerLimitReached()
        {

            FireTurret = true;

        }


        /*!Called when the timer limit is reached*/
        public void FireShootTimer()
        {
            ShootTimer.TurnTimerOn();
            ShootTimer.IncrementTimer();
        }

        /*!resets the shoot timer*/
        public void ResetShootTimer()
        {
            ShootTimer.Reset();
        }

    }//end of class

   
}//end of namespace
