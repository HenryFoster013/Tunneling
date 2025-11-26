using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderTracker : MonoBehaviour
{
    public GameObject SpiderBody;

    void Update()
    {
        if (SpiderBody != null)
        {
            //update this object to follow SpiderBody's Y
            Vector3 trackerPos = transform.position;
            trackerPos.y = SpiderBody.transform.position.y; // follow Y
            transform.position = trackerPos;

            //tracker takes X/Z from SpiderBody, keeps Y
            Vector3 trackerEuler = new Vector3(
                SpiderBody.transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,  // keep Y
                SpiderBody.transform.rotation.eulerAngles.z
            );
            transform.rotation = Quaternion.Euler(trackerEuler);

            //spiderBody keeps X/Z positions from tracker, takes Y from tracker
            Vector3 bodyPos = new Vector3(
                transform.position.x,
                transform.position.y,   // Y from tracker
                transform.position.z
            );
            SpiderBody.transform.position = bodyPos;

            Vector3 bodyEuler = new Vector3(
                SpiderBody.transform.rotation.eulerAngles.x, // keep X
                transform.rotation.eulerAngles.y,            // Y from tracker
                SpiderBody.transform.rotation.eulerAngles.z  // keep Z
            );
            SpiderBody.transform.rotation = Quaternion.Euler(bodyEuler);
        }
    }
}
