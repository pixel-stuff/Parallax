using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class parallaxPlanSave : parallaxPlan {



	public List<StockAssetStruct> m_stockAsset;
	public int hightId = -1;
	public int lowId = 0;
	

	// Use this for initialization
	void Start () {
		lowId = 0;
		hightId = -1;
		m_stockAsset = new List<StockAssetStruct>();
		InitParralax ();

	}
	
	// Update is called once per frame
	#if UNITY_EDITOR
	void Update () {
	#else
	void FixedUpdate () {
	#endif
		UpdateParralax ();
	}

	public override void moveAsset(float speedX,float speedY) {
		//clone for remove later 
		List<GameObject> temp = new List<GameObject>();
		foreach(GameObject g in visibleGameObjectTab) {
			if(!temp.Contains(g)) {
				temp.Add(g);
			}
		}

		for (int i=0; i<temp.Count; i++) {
			GameObject parrallaxAsset = temp[i];
			Vector3 positionAsset = parrallaxAsset.transform.position;
			if (!isStillVisible(parrallaxAsset)) {
				if(speedSign >0) {
					lowId++;
				} else {
					hightId--;
				}
				parrallaxAsset.SetActive(false);
				visibleGameObjectTab.Remove(parrallaxAsset);
				isInit =true;
			} else {
				positionAsset.x -= speedX;
				positionAsset.y -= speedY;
				#if UNITY_EDITOR
				parrallaxAsset.transform.position = positionAsset;
				#else
				parrallaxAsset.transform.position = Vector3.SmoothDamp(transform.position, positionAsset, ref velocity, dampTime);
				#endif
			}
		}
	}

	float CalculateXSpawnPosition(GameObject asset, float distanceBetweenAsset) {
        if(visibleGameObjectTab.Count == 0)
        {
            return popLimitation.x;
        }

		if (speedSign > 0) {
            float lastAssetRightestPosition = popLimitation.x - space;
			return lastAssetRightestPosition + RightXPosition(asset) + distanceBetweenAsset;
		} else {
            float lastAssetLeftestPosition = popLimitation.x + space;
            return lastAssetLeftestPosition + LeftXPosition(asset) + distanceBetweenAsset;
		}
	}

	void generateNewHightAsset() {
		GenerateAssetStruct assetStruct = generator.generateGameObjectAtPosition();
		GameObject asset = assetStruct.generateAsset;
		Vector3 position = asset.transform.position;
		asset.transform.parent = this.transform;
		float yPosition = 0f;
		if (visibleGameObjectTab.Count == 0) {
			yPosition = this.transform.position.y + yOffset;
		} else {
			yPosition = visibleGameObjectTab [0].transform.position.y;
		}
        asset.transform.position = new Vector3 (CalculateXSpawnPosition(asset, spaceBetweenAsset),yPosition, this.transform.position.z);
		visibleGameObjectTab.Add(asset);
		StockAssetStruct stockAssetStruct = new StockAssetStruct();
		stockAssetStruct.code = assetStruct.code;
		stockAssetStruct.dist = spaceBetweenAsset;
        SpriteRenderer renderer = asset.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            asset.GetComponent<SpriteRenderer>().color = colorTeint;
        }
		m_stockAsset.Add(stockAssetStruct);
		hightId ++;
		generateNewSpaceBetweenAssetValue();
	}


	void generateNewLowAsset() {
		GenerateAssetStruct assetStruct = generator.generateGameObjectAtPosition();
		GameObject asset = assetStruct.generateAsset;
		Vector3 position = asset.transform.position;
		asset.transform.parent = this.transform;
		float yPosition = 0f;
		if (visibleGameObjectTab.Count == 0) {
			yPosition = this.transform.position.y + yOffset;
		} else {
			yPosition = visibleGameObjectTab [0].transform.position.y;
		}
        asset.transform.position = new Vector3(CalculateXSpawnPosition(asset, spaceBetweenAsset), yPosition, this.transform.position.z);
		visibleGameObjectTab.Add(asset);
		StockAssetStruct stockAssetStruct = new StockAssetStruct();
		stockAssetStruct.code = assetStruct.code;
		stockAssetStruct.dist = spaceBetweenAsset;
        SpriteRenderer renderer = asset.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = colorTeint;
        }
        m_stockAsset.Insert(0,stockAssetStruct);
		hightId ++;
		generateNewSpaceBetweenAssetValue();
	}

	void generateOldAsset(int code,float dist)
    {
		GenerateAssetStruct assetStruct = generator.generateGameObjectWithCode(code);
		GameObject asset = assetStruct.generateAsset;
		asset.transform.parent = this.transform;
		float yPosition = 0f;
		if (visibleGameObjectTab.Count == 0) {
			yPosition = this.transform.position.y;
		} else {
			yPosition = visibleGameObjectTab [0].transform.position.y;
		}
/*
        if (speedSign > 0) {
			asset.transform.position = new Vector3(RightestXPosition(asset), yPosition, this.transform.position.z);
		} else {
			asset.transform.position = new Vector3(LeftestXPosition(asset), yPosition, this.transform.position.z);
		*/
        asset.transform.position = new Vector3(CalculateXSpawnPosition(asset, dist), yPosition, this.transform.position.z);
        SpriteRenderer spriteRenderer = asset.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            asset.GetComponent<SpriteRenderer>().color = colorTeint;
        }
		visibleGameObjectTab.Add(asset);
		if (speedSign > 0) {
			hightId ++;
		} else {
			lowId--;
		}
	}
	
	public override void generateAssetIfNeeded(){
		if(speedSign > 0){
			//Debug.Log("Hight ID = " + hightId);
			if(hightId == m_stockAsset.Count-1) {
				//Debug.Log("get Hight with space : "+ spaceBetweenLastAndPopLimitation() + " and space value "+ spaceBetweenAsset);
				if(spaceBetweenLastAndPopLimitation() > spaceBetweenAsset) {
				//	Debug.Log("generate Hight");
					generateNewHightAsset();
                    generateAssetIfNeeded();

                }
			} else { // si on a une valeur 
				//Debug.Log("get old Hight with space : "+ spaceBetweenLastAndPopLimitation() + " and stock value "+ m_stockAsset[hightId +1].dist);
				if(spaceBetweenLastAndPopLimitation() > m_stockAsset[hightId +1].dist) {
				//	Debug.Log("get old Hight");
					generateOldAsset(m_stockAsset[hightId +1].code,m_stockAsset[hightId +1].dist);
                    generateAssetIfNeeded();

                }
			}
		} else { 
			if (lowId == 0) {
				//Debug.Log("get low with space : "+ spaceBetweenLastAndPopLimitation() + " and space value "+ spaceBetweenAsset);
				if(spaceBetweenLastAndPopLimitation() > spaceBetweenAsset) {
					generateNewLowAsset();
                    generateAssetIfNeeded();
                //	Debug.Log("generate low");
                }
			} else {
				///Debug.Log("get old low with space : "+ spaceBetweenLastAndPopLimitation() + " and stock value "+ m_stockAsset[lowId].dist);
				if(spaceBetweenLastAndPopLimitation() > m_stockAsset[lowId].dist) {
					generateOldAsset(m_stockAsset[lowId-1].code,m_stockAsset[lowId].dist);
                    generateAssetIfNeeded();
                //	Debug.Log("get old low");
                }
			}
		}
	}
	
	bool isStillVisible (GameObject parallaxObject) {
		if (speedSign > 0) {
			return (RightestXPosition(parallaxObject) > depopLimitation.x);
		} else {
			return (LeftestXPosition(parallaxObject) < depopLimitation.x);
		}
	}
	
	float spaceBetweenLastAndPopLimitation() {
		if (visibleGameObjectTab.Count != 0) {
			float min = float.MaxValue;
			foreach(GameObject g in visibleGameObjectTab) {
				if (speedSign > 0) {
					min = Mathf.Min(min, popLimitation.x - (RightestXPosition(g)));
				}else {
					min = Mathf.Min(min, LeftestXPosition(g) - popLimitation.x);
				}
			}
			space = min;
			return space;
		} else {
            //Todo cumulé space pour permettre un delta plus grand que la camera
			return float.MaxValue;
		}
	}

	float RightestXPosition(GameObject g)
    {
		return g.transform.position.x + RightXPosition(g);
	}

    float RightXPosition(GameObject g)
    {
        float rightValue = float.MinValue;

        if (g.GetComponent<ParralaxSize>() != null)
        {
            rightValue = g.GetComponent<ParralaxSize>().rightestPosition;
        }
        else if (g.GetComponentsInChildren<SpriteRenderer>() != null)
        {
            foreach (SpriteRenderer spriteRenderer in g.GetComponentsInChildren<SpriteRenderer>())
            {
                rightValue = Mathf.Max(rightValue, spriteRenderer.sprite.bounds.max.x);
            }
        }

        //TODO same things for particules;
        return rightValue;
    }


    float LeftestXPosition(GameObject g)
    { 
		return g.transform.position.x + LeftXPosition(g);
	}

    float LeftXPosition(GameObject g)
    {
        float leftValue = float.MaxValue;

        if (g.GetComponent<ParralaxSize>() != null)
        {
            leftValue = g.GetComponent<ParralaxSize>().leftestPosition;
        }
        else if (g.GetComponentsInChildren<SpriteRenderer>() != null)
        {
            foreach (SpriteRenderer spriteRenderer in g.GetComponentsInChildren<SpriteRenderer>())
            {
                leftValue = Mathf.Min(leftValue, spriteRenderer.sprite.bounds.min.x);
            }
        }

        //TODO same things for particules;
        return leftValue;
    }


    public void Clear(){
		base.Clear ();
		m_stockAsset.Clear ();
	}
}