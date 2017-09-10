
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class randomSpawnAssetConfiguration : System.Object
{
	public GameObject prefabAsset = null;
	public Sprite spriteAsset = null;
	public int probabilityOfApparition;
}

[ExecuteInEditMode]
[System.Serializable]
public class assetRandomGenerator : parralaxAssetGenerator {
	
	public randomSpawnAssetConfiguration[] AssetConfiguation;
	public bool authoriseRandomFlip = false;
	public bool removeDirectDuplicata = true;
	public bool removeFlipDuplicata = true;
	public List<GameObject>[] GameObjectTabOfTypePrefabs = null;

	public GameObject additionalPrefabAsset;
	public Vector3 additionalPrefabPosition;

	private int probabilitySomme;
	private int previousId = -1;
	private int previousAssetId = -1;

	public override void Clear(){

		if (GameObjectTabOfTypePrefabs != null) {
			foreach (List<GameObject> list in GameObjectTabOfTypePrefabs) {
				foreach (GameObject go in list) {
					DestroyImmediate (go);
				}
			}
		}

		if(GameObjectTabOfTypePrefabs != null){
			for (int i =0; i < GameObjectTabOfTypePrefabs.Length; i++) {
				GameObjectTabOfTypePrefabs[i].Clear ();
			}
		}

	}

	private int getIdOfNextAsset() {
		int selectedAsset;
		int randomValue = random.Next()%(probabilitySomme);
		for (int i = 0; i < AssetConfiguation.Length; i++) {
				randomValue -= AssetConfiguation[i].probabilityOfApparition;
				if (randomValue < 0){
				selectedAsset = i;
				if (authoriseRandomFlip) {
					i = i*10;
					if (randomFlip ()) { 
						i += 1;
					}
				}
				if (removeDirectDuplicata && i == previousId && AssetConfiguation.Length > 1) {
					return getIdOfNextAsset ();
				}
				if (removeFlipDuplicata && selectedAsset == previousAssetId && AssetConfiguation.Length > 1) {
					return getIdOfNextAsset ();
				}
				previousId = i;
				previousAssetId = selectedAsset;
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

	private GameObject generateAssetWithSprite(Sprite sprite) {
		GameObject asset = new GameObject ();
		SpriteRenderer spriteRenderer = asset.AddComponent<SpriteRenderer> ();
		spriteRenderer.sprite = sprite;
		asset.name = sprite.name;
		return asset;
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
			if (AssetConfiguation [id].prefabAsset == null) {
				asset = generateAssetWithSprite (AssetConfiguation [id].spriteAsset);
				AssetConfiguation [id].prefabAsset = asset;
			} else {
				asset = Instantiate (AssetConfiguation[id].prefabAsset);
			}
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
			int randomValue = random.Next () % 2;//.Range (0, 2);
			return (randomValue == 0) ? true : false; 
		}
		return false;
	}
		
}
