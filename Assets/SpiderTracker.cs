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
                SpiderBody.transform.position = transform.position;
                SpiderBody.transform.rotation = transform.rotation;
            }
        }
}
