using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawner : MonoBehaviour //despawns everything that goes out of bounds to the left
{
    private Vector2 screenBoundaries; //stores the screen boundaries

    // Start is called before the first frame update
    void Start()
    {
         screenBoundaries = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)); //determine the screen boundaries
    }


    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -screenBoundaries.x * 2) //if the object has gone a considerable amount to the left off screen
        {
            //to avoid null pointer errors
            if (this.gameObject.CompareTag("Obstacle") || this.gameObject.CompareTag("FlyingObstacle") || this.gameObject.CompareTag("Refreshing") || this.gameObject.CompareTag("Confusing")) //if the objects are buildings and flying obstacles
            {
                Spawner.instances.Remove(this.gameObject.GetComponent<Rigidbody2D>());  //remove the instance from the list in the Spawner script
                Spawner.initialVelocity.Remove(this.gameObject.GetComponent<Rigidbody2D>()); // remove the saved velocity (to resume from pausing) in the Spawner script
            }
            else if(this.gameObject.CompareTag("Collectable")) //if the object is an envelope
            {
                EnvelopeSpawner.envelopes.Remove(this.gameObject.GetComponent<Rigidbody2D>()); //remove the instance from list in the EnvelopeSpawner script
            }
            else if(this.gameObject.CompareTag("Child")) //if the object is a child entity
            {
                ChildSpawner.children.Remove(this.gameObject.GetComponent<Rigidbody2D>()); //remove the instance from the list in the ChildSpawner script
            }
            else if(this.gameObject.CompareTag("Heavying")) //if the object is a rock entity
            {
                ChildAI.rocks.Remove(this.gameObject.gameObject.GetComponent<Rigidbody2D>()); //remove the instance from the list in the ChildAI script
            }
            else //the only other thing is a package
            {
                Deliver.packages.Remove(this.gameObject.GetComponent<Rigidbody2D>()); //remove the instance from the list in the Deliver script
            }
            Destroy(this.gameObject); //destory this object (despawn)
        }
    }
}
