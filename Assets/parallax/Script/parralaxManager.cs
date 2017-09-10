using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class CameraThreshold {
	public Vector3 popLimitation;
	public Vector3 depopLimitation;
}

[System.Serializable]
public class BasicGeneratorParameters {
	public AssetEntry typeOfAsset;
	public GameObject prefab = null;
	public Sprite sprite = null;
	public bool authoriseRandomFlip = false;
}

[System.Serializable]
public class RandomGeneratorParameters {
	public randomSpawnAssetConfiguration[] AssetConfiguation;
	public bool authoriseRandomFlip = false;
	public bool removeDirectDuplicata = true;
	public bool removeFlipDuplicata = true;
}


[System.Serializable]
public enum ParallaxPlansType {
	BASIC,
	WITHSAVE
};
[System.Serializable]
public enum GeneratorType {
	BASIC,
	RANDOM
};

[System.Serializable]
public enum AssetEntry {
	PREFAB,
	SPRITE
};

[System.Serializable]
public class ParralaxPlanConfiguration : System.Object
{
	public string nameParalaxPlan;
	[Header("Parralax plan selection")]
	public ParallaxPlansType parallaxType = ParallaxPlansType.WITHSAVE;
	[Header("\"Deep\" of the parralax plan, this will define the factor of speed")]
	[Tooltip("0 for ground, > 0 for foreground and <0 for background")]
	public float distance;
	public float yOffset = 0f;
	public float lowSpaceBetweenAsset;
	public float hightSpaceBetweenAsset;
    public float relativeSpeed;
	public Color colorTeinte = Color.clear;

	public int seed=0;

	public GeneratorType generatorType;

	public BasicGeneratorParameters basicGeneratorParameters;

	public RandomGeneratorParameters randomGeneratorParameters;


}
[ExecuteInEditMode]
public class parralaxManager : MonoBehaviour {

	[Header("Tab of all parralax plan configutation")]
	[SerializeField]
	private List<ParralaxPlanConfiguration> configurationParralax;

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
	[SerializeField]
	[Tooltip("seed for all plans if not set")]
	private int m_globalSeed = 123456789;

	private float speed;
	public CameraThreshold cameraThreshold = new CameraThreshold();
	private List<GameObject> parralaxPlans;



	public float CameraWidthSize = 0;

	public bool debugMode = false;

	public bool reset = false;

	private bool isPreviousPositionSet = false;
	private Vector3 previousCameraPosition = Vector3.zero;

	private EditorApplication.CallbackFunction s_backgroundUpdateCB;

	private bool m_refreshZoom = false;


	private void EditorCallback() {
		if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {
			clear ();
		}
		if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) {
			clear ();
		}
	}

	void OnEnable() {
		s_backgroundUpdateCB = new EditorApplication.CallbackFunction(EditorCallback);
		EditorApplication.update += s_backgroundUpdateCB;
	}

	void Awake(){
		clear ();
		var children = new List<GameObject>();
		foreach (Transform child in transform) children.Add(child.gameObject);
		children.ForEach(child => DestroyImmediate(child));

	}

	// Use this for initialization
	void Start () {
		reset = false;
		speed = constantSpeed;
		cameraThreshold.popLimitation = new Vector3 (0, cameraToFollow.transform.position.y - cameraToFollow.rect.height * cameraToFollow.orthographicSize, 0);
		cameraThreshold.depopLimitation = new Vector3 (0, cameraToFollow.transform.position.y - cameraToFollow.rect.height * cameraToFollow.orthographicSize, 0);
		parralaxPlans = new List<GameObject> ();
		foreach (ParralaxPlanConfiguration config in configurationParralax) {
			GameObject tempParralaxPlan = new GameObject();
			tempParralaxPlan.transform.parent = this.transform;
			tempParralaxPlan.name = config.nameParalaxPlan;

			parallaxPlan tempScript;
			if (config.parallaxType == ParallaxPlansType.BASIC) {
				tempScript = tempParralaxPlan.AddComponent<parallaxPlanBasic> (); 
			} else {
				tempScript = tempParralaxPlan.AddComponent<parallaxPlanSave> ();
			}
			tempScript.cameraThreshold = cameraThreshold;
			tempScript.distance = config.distance;
			tempScript.lowSpaceBetweenAsset = config.lowSpaceBetweenAsset;
			tempScript.hightSpaceBetweenAsset = config.hightSpaceBetweenAsset;
            tempScript.relativeSpeed = config.relativeSpeed;
			tempScript.colorTeint = config.colorTeinte;
			tempScript.cameraDistancePlan0 = cameraDistance;
			tempScript.horizonLineDistance = horizonLine;
			tempScript.yOffset = config.yOffset;
			tempScript.seed = (config.seed != 0) ? config.seed : m_globalSeed + (int)config.distance;

			if (config.generatorType == GeneratorType.BASIC) {
				assetGenerator generatorScript = tempParralaxPlan.AddComponent<assetGenerator> ();
				if (config.basicGeneratorParameters.typeOfAsset == AssetEntry.PREFAB) {
					generatorScript.prefab = config.basicGeneratorParameters.prefab;
				} else {
					generatorScript.spriteForPrefab = config.basicGeneratorParameters.sprite;
				}
				generatorScript.authoriseRandomFlip = config.basicGeneratorParameters.authoriseRandomFlip;
				tempScript.generator = generatorScript;
			} else {
				assetRandomGenerator generatorScript = tempParralaxPlan.AddComponent<assetRandomGenerator> ();
				generatorScript.AssetConfiguation = config.randomGeneratorParameters.AssetConfiguation;
				generatorScript.authoriseRandomFlip = config.randomGeneratorParameters.authoriseRandomFlip;
				generatorScript.removeDirectDuplicata = config.randomGeneratorParameters.removeDirectDuplicata;
				generatorScript.removeFlipDuplicata = config.randomGeneratorParameters.removeFlipDuplicata;
		
				tempScript.generator = generatorScript;
			}
				
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
		UpdateCameraThreshold ();
	}

	void UpdateCameraThreshold() {
		//reset the Pop and depop position 
		m_refreshZoom = false;
		float cameraOrthographiqueSize = cameraToFollow.orthographicSize*2;
		float CameraW = cameraToFollow.rect.width +2 ;
		if(CameraWidthSize != cameraOrthographiqueSize*CameraW || CameraWidthSize ==0)
		{
			//zoom
			CameraWidthSize = cameraOrthographiqueSize * CameraW;
			m_refreshZoom = true;
		}
		if (cameraThreshold != null) {
			cameraThreshold.popLimitation = new Vector3(cameraToFollow.transform.position.x + CameraW * cameraOrthographiqueSize,cameraToFollow.transform.position.y , cameraToFollow.transform.position.z);
			cameraThreshold.depopLimitation = new Vector3 (cameraToFollow.transform.position.x - CameraW * cameraOrthographiqueSize, cameraToFollow.transform.position.y, cameraToFollow.transform.position.z);
		}
	}

	void UpdateSpeedAndPosition(){
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
		if (parralaxPlans != null) {
			foreach (GameObject plan in parralaxPlans) {
				plan.GetComponent<parallaxPlan> ().setSpeedOfPlan (speed + cameraSpeedX, cameraSpeedY);
				if (m_refreshZoom) {
					plan.GetComponent<parallaxPlan> ().refreshOnZoom ();
				}
			}
		}
	}


	// Update is called once per frame
	#if UNITY_EDITOR
	void Update () {
	#else
	void FixedUpdate () {
	#endif
		UpdateCameraThreshold ();

		UpdateSpeedAndPosition ();

		if (debugMode) {
			setPlanConstante ();
		}

		if (reset) {
			resetAllPlan ();
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
			parralaxScript.yOffset = config.yOffset;
			parralaxScript.distance = config.distance;
			parralaxScript.lowSpaceBetweenAsset = config.lowSpaceBetweenAsset;
			parralaxScript.hightSpaceBetweenAsset = config.hightSpaceBetweenAsset;
			parralaxScript.relativeSpeed = config.relativeSpeed;
			parralaxScript.colorTeint = config.colorTeinte;
			parralaxScript.cameraDistancePlan0 = cameraDistance;
			parralaxScript.horizonLineDistance = horizonLine;

		}
	}
	private void resetAllPlan(){
		reset = false;
		clear ();
		Start ();
	}

	private void clear(){
		if (parralaxPlans != null) {
			foreach (GameObject plan in parralaxPlans) {
				plan.GetComponent<parallaxPlan> ().Clear ();
				DestroyImmediate (plan);
			}
		
			parralaxPlans.Clear ();
			parralaxPlans = null;
		}
	}
}
