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
	public float distance;

	public CameraThreshold cameraThreshold;

	public float hightSpaceBetweenAsset = 0;
	public float lowSpaceBetweenAsset = 0;
    public float relativeSpeed;

	public float cameraDistancePlan0;
	public float horizonLineDistance;

	public Color colorTeint = Color.clear;
	public int seed;

	protected System.Random m_random;

	protected int speedSign = 1;
	protected float actualSpeed = 0.0f;
	protected float YActualSpeed = 0.0f;

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
		

	public parralaxAssetGenerator generator;

    abstract public void refreshOnZoom();

	abstract public void clear();

	protected float randomRange (float min, float max){
		float factor = m_random.Next () / int.MaxValue;
		return factor * (max - min) + min;
	}

	protected void swapPopAndDepop(){
		speedSign = speedSign * -1;
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
}