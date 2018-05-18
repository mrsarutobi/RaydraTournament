using UnityEngine;
using System.Collections;
namespace RaydraTournament
{
    public class SetPosition : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (this.transform.parent != null)
                this.transform.position = this.transform.parent.position; //make sure that the parent and this position is the same
        }
    }
}