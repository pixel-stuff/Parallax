using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class parallaxPlanBasic : parallaxPlan {
	

	void Start() {
		InitParralax ();
	}
	// Update is called once per frame
	void Update () {
		UpdateParralax ();
	}
	
	public override void moveAsset(float speedX,float speedY){
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
		
	public override	void generateAssetIfNeeded(){
		if(((spaceBetweenLastAndPopLimitation() < (-spaceBetweenAsset + actualSpeed * speedMultiplicator)) && (speedSign > 0)) ||
		   ((spaceBetweenLastAndPopLimitation() > (spaceBetweenAsset + actualSpeed * speedMultiplicator)) && (speedSign < 0))){
			GenerateAssetStruct assetStruct = generator.generateGameObjectAtPosition();
			GameObject asset = assetStruct.generateAsset;
			Vector3 position = asset.transform.position;
			asset.transform.parent = this.transform;
			asset.GetComponent<SpriteRenderer> ().color = colorTeint;
			float yPosition = 0f;
			if (visibleGameObjectTab.Count == 0) {
				yPosition = this.transform.position.y;
			} else {
				yPosition = visibleGameObjectTab [0].transform.position.y;
			}
			asset.transform.position = new Vector3((popLimitation.x + (speedSign * asset.GetComponent<SpriteRenderer> ().sprite.bounds.max.x)) + (space-spaceBetweenAsset),yPosition,this.transform.position.z);
			visibleGameObjectTab.Add(asset);
			generateNewSpaceBetweenAssetValue();
		}
	}
	

	void generateNewSpaceBetweenAssetValue(){
		
		spaceBetweenAsset = - randomRange (lowSpaceBetweenAsset,hightSpaceBetweenAsset) * speedSign;
	}
	
	
	bool isStillVisible (GameObject parallaxObject) {
		if (speedSign < 0) {
			return (parallaxObject.transform.position.x - (parallaxObject.GetComponent<SpriteRenderer> ().sprite.bounds.max.x ) < depopLimitation.x);
		} else {
			return (parallaxObject.transform.position.x + (parallaxObject.GetComponent<SpriteRenderer> ().sprite.bounds.max.x ) >= depopLimitation.x);
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
			float result  = (g.transform.position.x +(visibleGameObjectTab[visibleGameObjectTab.Count - 1].GetComponent<SpriteRenderer> ().sprite.bounds.max.x)) - popLimitation.x;
			if (result > max){
				max = result;
			}
		}
		return max;
	}

	float getMinValue(){
		float min = 1000;
		foreach(GameObject g in visibleGameObjectTab){
			float result  = (g.transform.position.x -(g.GetComponent<SpriteRenderer> ().sprite.bounds.max.x)) - popLimitation.x;
			if (result < min){
				min = result;
			}
		}
		return min;
	}

	public override void clear(){
		generator.clear ();

		visibleGameObjectTab.Clear ();

	}
}
