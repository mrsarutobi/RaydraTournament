using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RaydraTournament;

namespace RaydraTournament {
    public class RaydraGoalZone : MonoBehaviour {
        /*! \class RaydraGoalZone
             class that represents Goalzone that the player scores in
          */

        RaydraGoalSpinner Spinner;  /*! Turrets object that is connected to this object*/
        RaydraGoalTurret[] Turrets=null; /*!array of turrets connected to goalzone*/
        private bool _BallMovingTowardGoal = false;
            
        bool InTheZone; //!private varible that indicate weither or not a player in in the goal zone or not
       private RaydraGoalPoint _GoalPoint; //!
        // Use this for initialization
        void Start() {

            //find the turrets that are connect to this goal only
            Spinner = this.GetComponentsInChildren<RaydraGoalSpinner>()[0];
            _GoalPoint = this.GetComponentsInChildren<RaydraGoalPoint>()[0];

        }

        // Update is called once per frame
        void Update() {

       

        }

        /*! called when the player enters the goal detection area*/
        void OnTriggerStay(Collider Entity)
        {


            if (Entity != null)
            {
                if (Entity.tag == "Player" || Entity.tag == "Enemy")
                    InTheZone = true;

                RaydraCellz TheBall;
                //the entity colliding in the zone is the ball
                if (Entity.tag == "Ball")
                {
                    TheBall = Entity.GetComponent<RaydraCellz>();
                    //we know that if the ball was shot from the player we are trying to score
                    if (TheBall.CellMovementState == CellMotionState.wasShotFromWeapon)
                        _BallMovingTowardGoal = true; //ball is moving toward the goal i.e( score being attempted)
                    else
                        _BallMovingTowardGoal = false; //ball is not moving toward the goal i.e( score not being attempted)


                }


            }

        }

        /*! called when the player leaves the goal detection area*/
        void OnTriggerExit(Collider Entity)
        {
            if (Entity.tag == "Player" || Entity.tag == "Enemy")
                  InTheZone = false;

        
        }


     public   bool BallMovingTowardGoal
        {
            get {return _BallMovingTowardGoal; }

        }


        public RaydraGoalPoint GoalPoint
        {
          get { return _GoalPoint; }
        }
        //!pubic property that indicate weither or not a player in in the goal zone or not
        public bool PlayerInGoalZone
        {
            get { return InTheZone; }
        }
    }
}
