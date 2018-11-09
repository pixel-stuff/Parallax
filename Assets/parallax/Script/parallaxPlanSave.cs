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
				if(speedState == SpeedState.forward) {
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

		if (speedState == SpeedState.forward) {
            float lastAssetRightestPosition = popLimitation.x - spaceBetweenLastAndPopLimitation;
			return lastAssetRightestPosition + RightXPosition(asset) + distanceBetweenAsset;
		} else {
            float lastAssetLeftestPosition = popLimitation.x + spaceBetweenLastAndPopLimitation;
            return lastAssetLeftestPosition + LeftXPosition(asset) + distanceBetweenAsset;
		}
	}

    StockAssetStruct GenerateNewAsset(float distanceBetweenAsset, int code = -1)
    {
        GenerateAssetStruct assetStruct;
        //create
        if (code != -1) // we generate an old asset
        {
            assetStruct = generator.generateGameObjectWithCode(code);
        }
        else
        {
            assetStruct = generator.generateGameObjectAtPosition();
        }
        GameObject asset = assetStruct.generateAsset;

        //position
        Vector3 position = asset.transform.position;
        asset.transform.parent = this.transform;
        float yPosition = groundYPosition - BottomYPosition(asset) - (popLimitation.y - groundYPosition) * speedMultiplicatorY;
        asset.transform.position = new Vector3(CalculateXSpawnPosition(asset, distanceBetweenAsset), yPosition, this.transform.position.z);

        //modify 
        //todo, all sprite rendrer
        SpriteRenderer renderer = asset.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            asset.GetComponent<SpriteRenderer>().color = colorTeint;
        }

        //save
        visibleGameObjectTab.Add(asset);

        StockAssetStruct stockAssetStruct = new StockAssetStruct();
        stockAssetStruct.code = assetStruct.code;
        stockAssetStruct.dist = distanceBetweenAsset;
        return stockAssetStruct;
    }
	
	public override void generateAssetIfNeeded(){
        RefreshSpaceBetweenLastAndPopLimitation();
		if(speedState == SpeedState.forward)
        {
			//Debug.Log("Hight ID = " + hightId);
			if(hightId == m_stockAsset.Count-1) {
				//Debug.Log("get Hight with space : "+ spaceBetweenLastAndPopLimitation() + " and space value "+ spaceBetweenAsset);
				if(spaceBetweenLastAndPopLimitation > spaceBetweenAsset) {
				//	Debug.Log("generate Hight");
					StockAssetStruct generatedAssetStruct = GenerateNewAsset(spaceBetweenAsset);
                    m_stockAsset.Add(generatedAssetStruct);
                    hightId++;
                    generateNewSpaceBetweenAssetValue();
                    generateAssetIfNeeded();

                }
			} else { // si on a une valeur 
				//Debug.Log("get old Hight with space : "+ spaceBetweenLastAndPopLimitation() + " and stock value "+ m_stockAsset[hightId +1].dist);
				if(spaceBetweenLastAndPopLimitation > m_stockAsset[hightId +1].dist) {
                    //	Debug.Log("get old Hight");
                    GenerateNewAsset(m_stockAsset[hightId +1].dist, m_stockAsset[hightId + 1].code);
                    hightId++;
                    generateAssetIfNeeded();

                }
			}
		} else { 
			if (lowId == 0) {
				//Debug.Log("get low with space : "+ spaceBetweenLastAndPopLimitation() + " and space value "+ spaceBetweenAsset);
				if(spaceBetweenLastAndPopLimitation > spaceBetweenAsset) {
                    StockAssetStruct generatedAssetStruct = GenerateNewAsset(spaceBetweenAsset);
                    m_stockAsset.Insert(0, generatedAssetStruct);
                    hightId++;
                    generateNewSpaceBetweenAssetValue();
                    generateAssetIfNeeded();
                //	Debug.Log("generate low");
                }
			} else {
				///Debug.Log("get old low with space : "+ spaceBetweenLastAndPopLimitation() + " and stock value "+ m_stockAsset[lowId].dist);
				if(spaceBetweenLastAndPopLimitation > m_stockAsset[lowId].dist) {
					GenerateNewAsset(m_stockAsset[lowId].dist, m_stockAsset[lowId - 1].code);
                    lowId--;
                    generateAssetIfNeeded();
                //	Debug.Log("get old low");
                }
			}
		}
	}
	
	bool isStillVisible (GameObject parallaxObject) {
		if (speedState == SpeedState.forward) {
			return (RightestXPosition(parallaxObject) > depopLimitation.x);
		} else {
			return (LeftestXPosition(parallaxObject) < depopLimitation.x);
		}
	}
	
	void RefreshSpaceBetweenLastAndPopLimitation() {
		if (visibleGameObjectTab.Count != 0) {
			float min = float.MaxValue;
			foreach(GameObject g in visibleGameObjectTab) {
				if (speedState == SpeedState.forward) {
					min = Mathf.Min(min, popLimitation.x - (RightestXPosition(g)));
				}else {
					min = Mathf.Min(min, LeftestXPosition(g) - popLimitation.x);
				}
			}
            spaceBetweenLastAndPopLimitation = min;
		} else {
            //Todo cumulé space pour permettre un delta plus grand que la camera
            spaceBetweenLastAndPopLimitation = float.MaxValue;
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

    float BottomYPosition(GameObject g)
    {
        float bottomValue = float.MaxValue;

        if (g.GetComponent<ParralaxSize>() != null)
        {
            //todo
           // bottomValue = g.GetComponent<Pa>().leftestPosition;
        }
        else if (g.GetComponentsInChildren<SpriteRenderer>() != null)
        {
            foreach (SpriteRenderer spriteRenderer in g.GetComponentsInChildren<SpriteRenderer>())
            {
                bottomValue = Mathf.Min(bottomValue, spriteRenderer.sprite.bounds.min.y);
            }
        }

        //TODO same things for particules;
        return bottomValue;
    }

    public void Clear(){
		base.Clear ();
		m_stockAsset.Clear ();
	}
}