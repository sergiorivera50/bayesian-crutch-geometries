using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform targetObject;

    void LateUpdate() {
        if (targetObject != null) {
            transform.position = new Vector3(targetObject.position.x, transform.position.y, transform.position.z);
        }
    }
}