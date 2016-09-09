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

	private float speed;
	private GameObject rightBorder;
	private GameObject leftBorder;
	private List<GameObject> parralaxPlans;



    private float CameraWidthSize = 0;

	public bool debugMode = false;
	
	// Use this for initialization
	void Start () {
		speed = constantSpeed;
		rightBorder = Instantiate ( new GameObject());
		rightBorder.name = "rightBorder";
		rightBorder.transform.position = new Vector3 (rightBorder.transform.position.x, cameraToFollow.transform.position.y - cameraToFollow.rect.height * cameraToFollow.orthographicSize, rightBorder.transform.position.z);
		//rightBorder.transform.parent = this.transform;
		leftBorder = Instantiate (new GameObject ());
		leftBorder.name = "leftBorder";
		leftBorder.transform.position = new Vector3 (leftBorder.transform.position.x, cameraToFollow.transform.position.y - cameraToFollow.rect.height * cameraToFollow.orthographicSize, leftBorder.transform.position.z);
		//leftBorder.transform.parent = this.transform;
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
		float cameraOrthographiqueSize = cameraToFollow.orthographicSize*2;
		float CameraW = cameraToFollow.rect.width;
        if (CameraWidthSize ==0) {
			this.transform.position = new Vector3(cameraToFollow.transform.position.x, this.transform.position.y, this.transform.position.z);
        }
        if(CameraWidthSize != cameraOrthographiqueSize*CameraW || CameraWidthSize ==0)
        {
            //zoom
            CameraWidthSize = cameraOrthographiqueSize * CameraW;
            refreshZoom = true;
        }
		rightBorder.transform .position = new Vector3 (cameraToFollow.transform.position.x + CameraW * cameraOrthographiqueSize, rightBorder.transform.position.y,rightBorder.transform .position.z);
		leftBorder.transform .position = new Vector3 (cameraToFollow.transform.position.x - CameraW * cameraOrthographiqueSize, leftBorder.transform.position.y,leftBorder.transform .position.z);


		float cameraSpeedX=0;
		float cameraSpeedY = 0;
		if (cameraToFollow != null){
			cameraSpeedX = (cameraToFollow.transform.position.x - this.transform.position.x);
			cameraSpeedY = (cameraToFollow.transform.position.y - this.transform.position.y);
			this.transform.position = new Vector3(cameraToFollow.transform.position.x, cameraToFollow.transform.position.y, this.transform.position.z);
		}
		
		foreach (GameObject plan in parralaxPlans) {
			plan.GetComponent<parallaxPlan> ().setSpeedOfPlan (speed+ cameraSpeedX,0); // TODO set speed Y
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

		}
	}
}
