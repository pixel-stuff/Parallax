using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class parallaxPlanBasic : parallaxPlan {
	
	public List<GameObject> visibleGameObjectTab;

	public float space;
	
	private float initSpeed = 0.1f;
	private bool isInit = false;
	
	private float actualSpeed = 0.0f;
	private float YActualSpeed = 0.0f;
	
	private float spaceBetweenAsset = 0.0f;
	private float speedMultiplicator;
	private float speedMultiplicatorY;
	private float initialY;
	
	private int speedSign = 1;
	
	// Use this for initialization
	void Start () {
		generator.clear ();

		visibleGameObjectTab.Clear ();


		actualSpeed = 0;
		speedSign = 1;
		speedMultiplicatorY = distance /(cameraDistancePlan0+Mathf.Abs (distance));
		speedMultiplicator = (Mathf.Abs (horizonLineDistance) + distance) / (Mathf.Abs (horizonLineDistance) + cameraDistancePlan0);
		generator.clear ();
		isInit = false;
		initSpeed = Mathf.Max( initSpeed * speedMultiplicator,0.01f);
		setSpeedOfPlan (initSpeed,0);
		while (!isInit) {
			Debug.Log("INIT");
			moveAsset (initSpeed,0);
			//			Debug.Log();
			generateAssetIfNeeded ();
		}
		initialY = this.transform.position.y; //TODO change this for Y in the config
	}
	
	// Update is called once per frame
	void Update () {
		moveAsset (actualSpeed * speedMultiplicator, YActualSpeed * speedMultiplicatorY);
		generateAssetIfNeeded ();
	}
	
	void moveAsset(float speedX,float speedY){
		List<GameObject> temp = new List<GameObject>();
		foreach(GameObject g in visibleGameObjectTab) {
			if(temp.Contains(g)){
				Debug.Log("WTF§§§§§§!!!!!!");
			}else {
				temp.Add(g);
			}
		}

		for (int i=0; i<temp.Count; i++) {
			GameObject parrallaxAsset = temp[i];
			Vector3 positionAsset = parrallaxAsset.transform.position;
			if (!isStillVisible(parrallaxAsset)){
				parrallaxAsset.SetActive(false);
				visibleGameObjectTab.Remove(parrallaxAsset);
				isInit =true;
			} else {
				positionAsset.x -= speedX;
				positionAsset.y -= speedY;
				parrallaxAsset.transform.position = positionAsset;
			}
		}
	}
		
	void generateAssetIfNeeded(){
		if(((spaceBetweenLastAndPopLimitation() < (-spaceBetweenAsset + actualSpeed * speedMultiplicator)) && (speedSign > 0)) ||
		   ((spaceBetweenLastAndPopLimitation() > (spaceBetweenAsset + actualSpeed * speedMultiplicator)) && (speedSign < 0))){
			GenerateAssetStruct assetStruct = generator.generateGameObjectAtPosition();
			GameObject asset = assetStruct.generateAsset;
			Vector3 position = asset.transform.position;
			asset.transform.parent = this.transform;
			asset.GetComponent<SpriteRenderer> ().color = colorTeint;
			float yPosition = 0f;
			if (visibleGameObjectTab.Count == 0) {
				yPosition = initialY;
			} else {
				yPosition = visibleGameObjectTab [0].transform.position.y;
			}
			asset.transform.position = new Vector3((popLimitation.transform.position.x + (speedSign * asset.GetComponent<SpriteRenderer> ().sprite.bounds.max.x)) + (space-spaceBetweenAsset),yPosition,this.transform.position.z);
			visibleGameObjectTab.Add(asset);
			generateNewSpaceBetweenAssetValue();
		}
	}
	
	
	void generateNewSpaceBetweenAssetValue(){
		spaceBetweenAsset = - Random.Range (lowSpaceBetweenAsset,hightSpaceBetweenAsset) * speedSign;
	}
	
	
	public override void setSpeedOfPlan(float newSpeed, float ySpeed){
		if ((actualSpeed > 0 && speedSign < 0) || (actualSpeed < 0 && speedSign > 0)) {
			swapPopAndDepop ();
		}
		actualSpeed = newSpeed;
		YActualSpeed = ySpeed;
	}
	
	void swapPopAndDepop(){
		GameObject temp = popLimitation;
		popLimitation = depopLimitation;
		depopLimitation = temp;
		speedSign = speedSign * -1;
	}
	
	
	bool isStillVisible (GameObject parallaxObject) {
		if (speedSign < 0) {
			return (parallaxObject.transform.position.x - (parallaxObject.GetComponent<SpriteRenderer> ().sprite.bounds.max.x ) < depopLimitation.transform.position.x);
		} else {
			return (parallaxObject.transform.position.x + (parallaxObject.GetComponent<SpriteRenderer> ().sprite.bounds.max.x ) >= depopLimitation.transform.position.x);
		}
	}
	
	
	float spaceBetweenLastAndPopLimitation() {
		if (visibleGameObjectTab.Count != 0) {
			if (speedSign > 0){
				space = getMaxValue();
			}else {
				space = getMinValue();
			}
			return space;
		} else {
			return - float.MaxValue;
		}
	}


	float getMaxValue(){
		float max = -1000;
		foreach(GameObject g in visibleGameObjectTab){
			float result  = (g.transform.position.x +(visibleGameObjectTab[visibleGameObjectTab.Count - 1].GetComponent<SpriteRenderer> ().sprite.bounds.max.x)) - popLimitation.transform.position.x;
			if (result > max){
				max = result;
			}
		}
		return max;
	}

	float getMinValue(){
		float min = 1000;
		foreach(GameObject g in visibleGameObjectTab){
			float result  = (g.transform.position.x -(g.GetComponent<SpriteRenderer> ().sprite.bounds.max.x)) - popLimitation.transform.position.x;
			if (result < min){
				min = result;
			}
		}
		return min;
	}

    public override void refreshOnZoom()
    {
        swapPopAndDepop();
        moveAsset(0,0);
        generateAssetIfNeeded();
        swapPopAndDepop();
    }
		

	public override void clear(){
		generator.clear ();

		visibleGameObjectTab.Clear ();

	}
}
