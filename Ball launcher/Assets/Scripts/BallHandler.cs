using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{

    //Serialize fields needs to be referenced from Unity Handler component
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float respawnDelay;
    [SerializeField] private float detachDelay;

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSpringJoint;
    private Camera mainCamera;
    private bool isDragging;


    // Start is called before the first frame update
    void Start() {
        mainCamera = Camera.main; 
        SpawnNewBall();       
    }

    // Update is called once per frame
    void Update()
    {
        //If we have no ball, run no code        
        if(currentBallRigidbody == null){ return; }

        //If user does not touch screen
        if(!Touchscreen.current.primaryTouch.press.isPressed){

            //If we have touched the screen previously, launch the ball
            if(isDragging) {                
                LaunchBall();                
            }

            //No longer touching the screen
            isDragging = false;

            return;
        }

        //Touching the screen
        isDragging = true;

        //Take ball out of physics controls when it is moved, so it will follor touch
        currentBallRigidbody.isKinematic = true;

        //Read the position where user is touching and convert it to worldpoints
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
         
        //Set ball location to where user is touching the screen
        currentBallRigidbody.position = worldPosition;       
    }

    private void SpawnNewBall(){

        //Create new ball prefab object at the pivot position, using default quartenion
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        //Connect/attach created ball to the pivot
        currentBallSpringJoint.connectedBody = pivot;
    }

    private void LaunchBall(){

        //Set ball taken physics controls when it is moved, so it will be flinget towards spring pivot point
        currentBallRigidbody.isKinematic = false;

        //Clear reference to ball, so it wont get back where user touches screen next
        currentBallRigidbody = null;

        Invoke(nameof(DetachBall), detachDelay);
    }

    private void DetachBall(){    
        //Detach ball from spring joint, so ball can fly freely
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;
        
        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
