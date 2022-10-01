using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    void Update()
    {
        //カメラの切り替え
        int floorNum = (int)((GameObject.Find("Myon").transform.position.y + 5.0f) / 10);
        gameObject.transform.position = new Vector3(0, 10 * floorNum, -10);
    }
}
