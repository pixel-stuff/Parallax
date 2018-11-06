﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ParralaxPlanConfiguration : System.Object
{
	[Header("Parralax plan prefab")]
	public GameObject prefabParralaxPlan;
	[Header("\"Deep\" of the parralax plan, this will define the factor of speed")]
	[Tooltip("0 for ground, > 0 for foreground and <0 for background")]
	public float distance;
	public parralaxAssetGenerator generatorScript;
	public float lowSpaceBetweenAsset;
	public float hightSpaceBetweenAsset;
    public float relativeSpeed;
	public Color colorTeinte = Color.clear;
	public string nameParalaxPlan;
}

public class parralaxManager : MonoBehaviour {

	[Header("Tab of all parralax plan configutation")]
	[SerializeField]
	//[Tooltip("Health value between 0 and 100.")]
	private ParralaxPlanConfiguration[] configurationParralax;

	[Header("Configuration of parralax Manager")]
	[SerializeField]
	[Tooltip("Camera that the parralax will follow.event if the camera don't move, set one")]
	private Camera cameraToFollow = null;
	[SerializeField]
	[Tooltip("independante speed. This speed willaffect all the parralax plan ")]
	private float constantSpeed;

	[SerializeField]
	[Tooltip("Distance between the game plan and the camera, this will affect the parallax effect")]
	private float cameraDistance=3;
	[SerializeField]
	[Tooltip("Distance of the last plan, a plan at this distance will not move at all")]
	private float horizonLine=-4000;

	private float speed;
	private GameObject rightBorder;
	private GameObject leftBorder;
	private List<GameObject> parralaxPlans;



    private float CameraWidthSize = 0;

	public bool debugMode = false;

	private bool isPreviousPositionSet = false;
	private Vector3 previousCameraPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
		speed = constantSpeed;
		rightBorder = new GameObject();
		rightBorder.name = "rightBorder";
		rightBorder.transform.position = new Vector3 (rightBorder.transform.position.x, cameraToFollow.transform.position.y - cameraToFollow.rect.height * cameraToFollow.orthographicSize, rightBorder.transform.position.z);
		rightBorder.transform.parent = this.transform;
		leftBorder = new GameObject ();
		leftBorder.name = "leftBorder";
		leftBorder.transform.position = new Vector3 (leftBorder.transform.position.x, cameraToFollow.transform.position.y - cameraToFollow.rect.height * cameraToFollow.orthographicSize, leftBorder.transform.position.z);
		leftBorder.transform.parent = this.transform;
		parralaxPlans = new List<GameObject> ();
		foreach (ParralaxPlanConfiguration config in configurationParralax) {
			GameObject tempParralaxPlan = Instantiate(config.prefabParralaxPlan);
			tempParralaxPlan.transform.parent = this.transform;
			tempParralaxPlan.name = config.nameParalaxPlan;

			parallaxPlan tempScript = tempParralaxPlan.GetComponent<parallaxPlan>();
			tempScript.popLimitation = rightBorder;
			tempScript.depopLimitation = leftBorder;
			tempScript.generator = config.generatorScript;
			tempScript.distance = config.distance;
			tempScript.lowSpaceBetweenAsset = config.lowSpaceBetweenAsset;
			tempScript.hightSpaceBetweenAsset = config.hightSpaceBetweenAsset;
            tempScript.relativeSpeed = config.relativeSpeed;
			tempScript.colorTeint = config.colorTeinte;
			tempScript.cameraDistancePlan0 = cameraDistance;
			tempScript.horizonLineDistance = horizonLine;

			parralaxPlans.Add(tempParralaxPlan);
		}
		parralaxPlans.Sort(delegate(GameObject x, GameObject y)
		{
			parallaxPlan tempScriptX = x.GetComponent<parallaxPlan>();
			parallaxPlan tempScriptY = y.GetComponent<parallaxPlan>();
			if (tempScriptX.distance == tempScriptY.distance) {
				return 0;
			} else if (tempScriptX.distance < tempScriptY.distance) {
				return 1;
			} else return -1;
		});

		float zinf = 2.0f;
		float zsupp = -2.0f;
		foreach (GameObject temp in parralaxPlans) {
			if(temp.GetComponent<parallaxPlan>().distance < 0){
				temp.transform.localPosition = new Vector3(temp.transform.localPosition.x,temp.transform.localPosition.y,temp.transform.localPosition.z + zinf++);
			} else if(temp.GetComponent<parallaxPlan>().distance == 0) {
				temp.transform.localPosition = new Vector3(temp.transform.localPosition.x,temp.transform.localPosition.y,temp.transform.localPosition.z );
			} else {
				temp.transform.localPosition = new Vector3(temp.transform.localPosition.x,temp.transform.localPosition.y,temp.transform.localPosition.z+ zsupp--);
			}
		}
		Update ();
	}
	
	// Update is called once per frame
	void Update () {
        //reset the Pop and depop position 
        bool refreshZoom = false;
        float height = cameraToFollow.orthographicSize;
        float cameraOrthographiqueSize = height * cameraToFollow.aspect;
        //float cameraOrthographiqueSize = cameraToFollow.;
		float CameraW = cameraToFollow.rect.width;

        if(CameraWidthSize != cameraOrthographiqueSize*CameraW || CameraWidthSize ==0)
        {
            //zoom
            CameraWidthSize = cameraOrthographiqueSize * CameraW;
            refreshZoom = true;
        }
		rightBorder.transform .position = new Vector3 (cameraToFollow.transform.position.x + cameraOrthographiqueSize * CameraW, rightBorder.transform.position.y,rightBorder.transform .position.z);
		leftBorder.transform .position = new Vector3 (cameraToFollow.transform.position.x - cameraOrthographiqueSize * CameraW, leftBorder.transform.position.y,leftBorder.transform .position.z);


		float cameraSpeedX=0;
		float cameraSpeedY = 0;
		if (cameraToFollow != null){
			if (!isPreviousPositionSet) {
				isPreviousPositionSet = true;
				previousCameraPosition = cameraToFollow.transform.position;
			}
			cameraSpeedX = (cameraToFollow.transform.position.x - previousCameraPosition.x);
			cameraSpeedY = (cameraToFollow.transform.position.y - previousCameraPosition.y);
			previousCameraPosition = cameraToFollow.transform.position;
			this.transform.position = new Vector3(cameraToFollow.transform.position.x, this.transform.position.y, this.transform.position.z);
		}
		
		foreach (GameObject plan in parralaxPlans) {
			plan.GetComponent<parallaxPlan> ().setSpeedOfPlan (speed+ cameraSpeedX,cameraSpeedY); // TODO set speed Y
            if (refreshZoom)
            {
                plan.GetComponent<parallaxPlan>().refreshOnZoom();
            }
		}

		if (debugMode) {
			setPlanConstante ();
		}
	}

	public float getGroundSpeedf() {
		return speed;
	}

	public void isPaused(bool pause) {
		if (pause) {
			speed = 0;
		} else {
			speed = constantSpeed;
		}
	}

	private void setPlanConstante() {
		speed = constantSpeed;

		foreach (ParralaxPlanConfiguration config in configurationParralax) {
			GameObject tempParralaxPlan = parralaxPlans.Find (plan => plan.name == config.nameParalaxPlan);

			parallaxPlan parralaxScript = tempParralaxPlan.GetComponent<parallaxPlan>();
			parralaxScript.generator = config.generatorScript;
			parralaxScript.distance = config.distance;
			parralaxScript.lowSpaceBetweenAsset = config.lowSpaceBetweenAsset;
			parralaxScript.hightSpaceBetweenAsset = config.hightSpaceBetweenAsset;
			parralaxScript.relativeSpeed = config.relativeSpeed;
			parralaxScript.colorTeint = config.colorTeinte;
			parralaxScript.cameraDistancePlan0 = cameraDistance;
			parralaxScript.horizonLineDistance = horizonLine;

		}
	}
}
