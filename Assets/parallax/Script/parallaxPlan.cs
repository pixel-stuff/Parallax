using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StockAssetStruct : System.Object
{
	public int code;
	public float dist; 
}

abstract public class parallaxPlan : MonoBehaviour {
	public float distance;
	
	public GameObject popLimitation;
	public GameObject depopLimitation;
	public float hightSpaceBetweenAsset = 0;
	public float lowSpaceBetweenAsset = 0;
    public float relativeSpeed;

	public float cameraDistancePlan0;
	public float horizonLineDistance;

	public Color colorTeint = Color.clear;

	public parralaxAssetGenerator generator;

	abstract public void setSpeedOfPlan(float newSpeed, float ySpeed);

    abstract public void refreshOnZoom();

	abstract public void reset();

	abstract public void clear();


}