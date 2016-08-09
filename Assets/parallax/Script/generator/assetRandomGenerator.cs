﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class randomSpawnAssetConfiguration : System.Object
{
	public GameObject prefabAsset;
	public int probabilityOfApparition;
}

public class assetRandomGenerator : parralaxAssetGenerator {
	
	public randomSpawnAssetConfiguration[] AssetConfiguation;
	public bool authoriseRandomFlip = false;
	public bool removeDirectDuplicata = true;
	public List<GameObject>[] GameObjectTabOfTypePrefabs = null;

	public GameObject additionalPrefabAsset;
	public Vector3 additionalPrefabPosition;

	private int probabilitySomme;
	private int previousId = -1;

	// Use this for initialization
	public override void clear(){
		if(GameObjectTabOfTypePrefabs != null){
			for (int i =0; i < GameObjectTabOfTypePrefabs.Length; i++) {
				GameObjectTabOfTypePrefabs[i].Clear ();
			}
		}
	}

	private int getIdOfNextAsset() {
		int random = Random.Range(0,probabilitySomme);
		for (int i = 0; i < AssetConfiguation.Length; i++) {
			random -= AssetConfiguation[i].probabilityOfApparition;
			if (random < 0){
				if (authoriseRandomFlip) {
					i = i*10;
					if (randomFlip ()) { 
						i += 1;
					}
				}
				if (removeDirectDuplicata && i == previousId && AssetConfiguation.Length > 1) {
					return getIdOfNextAsset ();
				}
				previousId = i;
				return i;
			}
		}
		return -1;
	}


	public void initTabOfTypeIfNeeded() {
		if (GameObjectTabOfTypePrefabs == null) {
			probabilitySomme = 0;
			GameObjectTabOfTypePrefabs = new List<GameObject>[AssetConfiguation.Length];
			for (int i =0; i < AssetConfiguation.Length; i++) {
				probabilitySomme += AssetConfiguation[i].probabilityOfApparition;
				GameObjectTabOfTypePrefabs[i] = new List<GameObject>();
			}
		}
	}


	public GenerateAssetStruct generateAssetStructForId (int id){
		int initialId = id;
		bool isFlipped = false;
		if (authoriseRandomFlip) {
			isFlipped = (id % 2 ==0)? false : true;
			id = id / 10;
		}
		GameObject asset = availableGameobject (GameObjectTabOfTypePrefabs[id]);
		if (asset == null) {
			asset = Instantiate (AssetConfiguation[id].prefabAsset);
			asset.GetComponent<SpriteRenderer> ().flipX = isFlipped;
			if (additionalPrefabAsset != null) {
				GameObject additional = Instantiate (additionalPrefabAsset);
				additional.transform.SetParent (asset.transform);
				additional.transform.position = additionalPrefabPosition;
			}
			GameObjectTabOfTypePrefabs[id].Add (asset);
		}
		GenerateAssetStruct assetStruct = new GenerateAssetStruct();
		assetStruct.generateAsset = asset;
		assetStruct.code = initialId;
		return assetStruct;
	}


	public override GenerateAssetStruct generateGameObjectWithCode(int code) {
		Debug.Log ("generate for code : " + code);
		initTabOfTypeIfNeeded ();
		return generateAssetStructForId(code);
	}


	public override GenerateAssetStruct generateGameObjectAtPosition() {
		initTabOfTypeIfNeeded ();
		int id = getIdOfNextAsset ();
		if (id >= 0) {
			return generateAssetStructForId(id);
			}
		return null;
	}
		

	private GameObject availableGameobject(List<GameObject> list){
		foreach(GameObject gameobject in list){
			if (!gameobject.activeSelf){
				gameobject.SetActive(true);
				return gameobject;
			}
		}
		return null;
	}

	private bool randomFlip(){
		if (authoriseRandomFlip) {
			int random = Random.Range (0, 2);
			return (random == 0) ? true : false; 
		}
		return false;
	}
}
