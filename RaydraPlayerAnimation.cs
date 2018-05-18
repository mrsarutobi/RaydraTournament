using UnityEngine;
using System.Collections;

namespace RaydraTournament
{

    public class RaydraPlayerAnimations : MonoBehaviour

    /*! \class RaydraPlayerAnimations 
 this class handles animations................



    */
    {
        private Animator X8Animator; //! The Animator connected to this game object
        private RaydraControls PlayerControls;//! reference to the controls attached to the plaeyr object

        //! Intializes the variables for use
        void Start()
        {
            X8Animator = this.GetComponent<Animator>();
            PlayerControls = this.GetComponent<RaydraControls>();
           
        }

        // Update is called once per frame
        void FixedUpdate()
        {

        }
    }

}
