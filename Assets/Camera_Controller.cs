using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Camera_Controller : NetworkBehaviour
{


    //public override void OnNetworkSpawn()
    //{
    //    camera = GetComponent<Camera>();
    //    // move the player out of the player gameobject 


    //    if(!IsOwner)
    //    {
    //        camera.gameObject.SetActive(false);
    //        enabled = false;

    //        return;
    //    }



    //}


    public Transform Target;
    public float MaxDistance = 4f;
    public float Speed = 4;

    private new Camera camera;
    private Vector3 viewportPoint;
    private Vector3 desiredPosition;

    

    void LateUpdate()
    {
        if (Target == null)
            return;
        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 centerPoint = camera.ViewportToWorldPoint(viewportPoint);
        Vector3 deltaPosition = Target.position - centerPoint;

        if (deltaPosition.sqrMagnitude > MaxDistance * MaxDistance)
            desiredPosition = transform.position + deltaPosition;

        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Speed * Time.deltaTime);
    }

    public void CustomStart(Transform target)
    {
        Target = target;
        camera = GetComponent<Camera>();
        viewportPoint = new Vector3(0.5f, 0.5f, Mathf.Abs(transform.position.z - Target.position.z));
        desiredPosition = transform.position;
    }



}
