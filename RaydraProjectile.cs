using UnityEngine;
using System.Collections;


namespace RaydraTournament
{
    public class RaydraProjectile : MonoBehaviour
    {/*
        \class RaydraProjectile
        class the represents the projectile that the 

        */

        //!how long this projectiel will last
        public float LifeLimit;
        //! the assigne path of this projectile
        public Vector3 Path;
        //! the variable where the path will be store
        private Vector3 Trajectory;
        //! this will determine the age of the projectle
        private X8Timer LifeTimer;
        //! future mess data for the projectile
        private Mesh WeaponMesh;

        private bool _hitSomething= false; //!indicates if we have hit a player or not




        //! Use this for initialization
        void Start()
        {
            Trajectory = Path;
            LifeTimer = new X8Timer(LifeLimit);
            LifeTimer.TurnTimerOn();
            
           
        //get the timer that is attached to this object

    }

        // Update is called once per frame
        void Update()
        {
            Trajectory = Path;
            if(LifeTimer != null)
                LifeTimer.IncrementTimer();// update the ````timer on the timer 
        }



       void OnTriggerEnter(Collider Entity)
        {
            if (Entity.gameObject.tag == "Player" || Entity.gameObject.tag == "Ground"|| Entity.gameObject.tag == "Enemy")
                _hitSomething = true;         
           
             

       

        }


        //! How many frames the projectile has been alive for
        public float AgeInFrames
        {
            get
            { return LifeTimer.AgeInFrames; }

        }
        //! How many seconds the projectile has been alive for
        public float AgeinSeconds
        {
            get
            {
                return LifeTimer.AgeinSeconds;  //this is will give us the frames in seconds;
            }

        }
        //! If the timer for the life timer for the projectile is on
        public bool IsEnabled
        {
            get { return LifeTimer.IsEnabled; }


        }

        //!The age limit of the projectile
        public float AgeLimit
        {
            get { return LifeTimer.AgeLimit; }
        }
        //! this medthod will turn the timer on
        public void TurnTimerOn()
        {
            LifeTimer.TurnTimerOn();
        }
        //! this is the travelpath of the projectile
        public Vector3 TravelPath
        {
            get { return Trajectory; }

        }

        public bool HitSomething
        {

            get  { return _hitSomething; }
        }


    }//end of class

}//end of namespace
