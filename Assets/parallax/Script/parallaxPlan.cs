using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SpeedState {
	forward,
	backward,
	error
}


[System.Serializable]
public class StockAssetStruct : System.Object
{
	public int code;
	public float dist; 
}

abstract public class parallaxPlan : MonoBehaviour {

	/*****************/
	// Configuration set by Camera Manager
	/*****************/

	public CameraThreshold cameraThreshold;

	public parralaxAssetGenerator generator;

	public float distance;

	public float hightSpaceBetweenAsset = 0;
	public float lowSpaceBetweenAsset = 0;
    public float relativeSpeed;
	public Color colorTeint = Color.clear;

	public float cameraDistancePlan0;
	public float horizonLineDistance;

	public int seed;
	public float yOffset = 0f;
	/*****************/
	// END Configuration set by Camera Manager
	/*****************/



	/*****************/
	// Internal var 
	/*****************/

	protected System.Random m_random;

	protected int speedSign = 1;
	protected float actualSpeed = 0.0f;
	protected float YActualSpeed = 0.0f;


	protected float initSpeed = 0.1f;
	protected bool isInit = false;

	protected List<GameObject> visibleGameObjectTab;

	protected float space;


	protected float spaceBetweenAsset = 0.0f;
	protected float speedMultiplicator;
	protected float speedMultiplicatorY;


	public float dampTime = 0.15f;
	protected Vector3 velocity = Vector3.zero;

	protected SpeedState speedState { 
		get {
			if (speedSign > 0) {
				return SpeedState.forward;
			} else if (speedSign < 0) {
				return SpeedState.backward;
			} else {
				Debug.LogError ("Error : SpeedSign = 0");
				return SpeedState.error;
			}
		}
	}

	protected Vector3 popLimitation{ 
		get{
			switch (speedState) {
			case SpeedState.forward:
				return cameraThreshold.popLimitation;

			case SpeedState.backward:
				return cameraThreshold.depopLimitation;
			default:
				return Vector3.zero;
			}
		}
	}
	protected Vector3 depopLimitation {
		get{
			switch (speedState) {
			case SpeedState.forward:
				return cameraThreshold.depopLimitation;

			case SpeedState.backward:
				return cameraThreshold.popLimitation;
			default:
				return Vector3.zero;
			}
		}
	}
		

	/*****************/
	// End Internal var 
	/*****************/

	/*****************/
	// Fonction
	/*****************/

	// Use this for initialization
	protected void InitParralax () {
		m_random = new System.Random (seed);
		generator.random = m_random;

		generator.Clear ();

		visibleGameObjectTab = new List<GameObject>();

		actualSpeed = 0;
		speedSign = 1;
		setTheDistanceMultiplicator ();
		generator.Clear ();
		isInit = false;
		initSpeed = Mathf.Max( initSpeed * speedMultiplicator,0.01f);
		setSpeedOfPlan (initSpeed,0);
		generateNewSpaceBetweenAssetValue();
		while (!isInit) {
			moveAsset (initSpeed,0);
			generateAssetIfNeeded ();
		}
	}


	void setTheDistanceMultiplicator() {
		speedMultiplicatorY = distance /(cameraDistancePlan0+Mathf.Abs (distance));
		speedMultiplicator = (Mathf.Abs (horizonLineDistance)+ distance) / (Mathf.Abs (horizonLineDistance) + cameraDistancePlan0);
	}

	protected void UpdateParralax() {
		moveAsset (actualSpeed * speedMultiplicator, YActualSpeed * speedMultiplicatorY);
		generateAssetIfNeeded ();
	}

	protected float randomRange (float min, float max){
		float factor = m_random.Next () / int.MaxValue;
		return factor * (max - min) + min;
	}

	protected void swapPopAndDepop(){
		speedSign = speedSign * -1;
	}

	protected void generateNewSpaceBetweenAssetValue(){

		spaceBetweenAsset = randomRange (lowSpaceBetweenAsset,hightSpaceBetweenAsset);
	}


	//call by manager for set the new speed each loop
	public void setSpeedOfPlan(float newSpeed, float ySpeed){
		float speed = newSpeed + relativeSpeed;
		if ((speed > 0 && speedSign < 0) || (speed < 0 && speedSign > 0)) {
			swapPopAndDepop ();
			print ("Swap");
		}
		actualSpeed = speed;
		YActualSpeed = ySpeed;
	}
		
	public void refreshOnZoom() {
		if (isInit) {
			swapPopAndDepop();
			moveAsset(0,0);
			generateAssetIfNeeded();
			swapPopAndDepop();
		}
	}

	//abstract public void clear();
	public void Clear(){

		generator.Clear ();

		visibleGameObjectTab.Clear ();
	}


	abstract public void moveAsset (float speedX, float speedY);
	abstract public void generateAssetIfNeeded ();

}