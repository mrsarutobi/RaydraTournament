using UnityEngine;
using System.Collections;

namespace RaydraTournament {
    /**
    /*! \class CellActions 
     * This class controls the Raydra Cell(ball) 
     * it contains information about when the cell is held in the players hand
     * And what state the Cell is currently in*/
    public class CellActions : MonoBehaviour
    {

        public GameObject Cell; //! This is the actual RaydarCell object
        public GameObject CarryPoint; //!Where the ball at in the players hand

        private RaydraCell RaydraOrbState;//! The state the Cell is in at the moment
        private Animator X8Animator; //! reference to the Animator that is attached to this player
        private AnimatorStateInfo StateInfo; //! Current state to the the animator




        // Use this for initialization
        void Start()
        {

            RaydraOrbState = new RaydraCell();

            // X8Animator= Hiruzen.GetComponents<Animator>()[0];


            CellState1 = CellState.IsInHand;
            // Update is called once per frame
        }
        void Update()
        {

            if (CellState1 == CellState.IsInHand) //! If the ball in hand state is active then put the ball in the player hand
                this.transform.position = CarryPoint.transform.position;

        }
        /** THe current state of the Rayda Cell*/
        public CellState CellState1
        {
            get;
            set;

        }
        /** The animation state cell is currently in*/
        public CellAnimationState CellAnimationState
        {
            get
            {
                return RaydraOrbState.CellPlayState;
            }

            set
            {
                RaydraOrbState.CellPlayState = value;
            }

        }



    }


}

