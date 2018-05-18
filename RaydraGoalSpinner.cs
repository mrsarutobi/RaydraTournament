using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RaydraTournament
{
    public class RaydraGoalSpinner : MonoBehaviour
    {


        /*! \class RaydraGoalSpinner 
          this class handle actions of the spinning piece on the goal
          */

        //!The Goalzone object that this spinner is contained in
        private RaydraGoalZone ParentZone;
        void Start()
        {
            ParentZone = this.GetComponentInParent<RaydraGoalZone>();
        }

        // Update is called once per frame
        void Update()
        {


            //if the player is on the goal zone
            if (ParentZone.PlayerInGoalZone == false)
            {
                Quaternion transform = new Quaternion(0, 90, 90, 0);


                this.transform.rotation = transform;
            }
            else
            {
                //stop the spinner if the ball is moving toward the goal
                if(ParentZone.BallMovingTowardGoal == false)
                    this.transform.Rotate(5f, 0, 0); //rotate the spinner
            }


        }
    }

}
