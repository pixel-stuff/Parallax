using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class assetGenerator : parralaxAssetGenerator {

	public GameObject prefab = null;
	public Sprite spriteForPrefab = null;
	public bool authoriseRandomFlip = false;

	public List<GameObject> GameObjectTabOfTypePrefab = new List<GameObject>();

	// Use this for initialization
	public override void Clear(){
		if (GameObjectTabOfTypePrefab != null) {
			foreach (GameObject go in GameObjectTabOfTypePrefab) {
				DestroyImmediate (go);
			}
		GameObjectTabOfTypePrefab.Clear ();
	}
	}


	public override GenerateAssetStruct generateGameObjectWithCode(int code){
		return generateGameObjectAtPosition ();
	}

	private GameObject generateAssetWithSprite(Sprite sprite) {
		GameObject asset = new GameObject ();
		SpriteRenderer spriteRenderer = asset.AddComponent<SpriteRenderer> ();
		spriteRenderer.sprite = sprite;
		asset.name = sprite.name;
		return asset;
	}

	public override  GenerateAssetStruct generateGameObjectAtPosition() {
		GameObject asset = availableGameobject (GameObjectTabOfTypePrefab);
		if (asset == null) {
			if (prefab == null) {
				asset = generateAssetWithSprite (spriteForPrefab);
				prefab = asset;
			} else {
				asset = Instantiate (prefab);
			}
			asset.GetComponent<SpriteRenderer> ().flipX = randomFlip ();
			GameObjectTabOfTypePrefab.Add (asset);
		}
		GenerateAssetStruct assetStruct = new GenerateAssetStruct();
		assetStruct.generateAsset = asset;
		assetStruct.code = 0;
		return assetStruct; 
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
			int randomValue = random.Next () % 2;
			return (randomValue == 0) ? true : false; 
		}
		return false;
	}
}
