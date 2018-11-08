using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParralaxSize : MonoBehaviour {

    public GameObject leftObject;
    public GameObject rightObject;

    public float parralaxSize
    {
        get
        {
            return rightObject.transform.position.x - leftObject.transform.position.x;
        }
    }
    public float rightestPosition
    {
        get
        {
            return rightObject.transform.localPosition.x ;
        }
    }

    public float leftestPosition
    {
        get
        {
            return leftObject.transform.localPosition.x;
        }
    }
}
