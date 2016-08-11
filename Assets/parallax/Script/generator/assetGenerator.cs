using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class assetGenerator : parralaxAssetGenerator {

	public GameObject prefab = null;
	public Sprite spriteForPrefab = null;
	public bool authoriseRandomFlip = false;

	public List<GameObject> GameObjectTabOfTypePrefab = new List<GameObject>();

	// Use this for initialization
	public override void clear(){
		GameObjectTabOfTypePrefab.Clear ();
	}


	public override GenerateAssetStruct generateGameObjectWithCode(int code){
		return generateGameObjectAtPosition ();
	}

	private GameObject generateAssetWithSprite(Sprite sprite) {
		GameObject asset = new GameObject ();
		SpriteRenderer spriteRenderer = asset.AddComponent<SpriteRenderer> ();
		spriteRenderer.sprite = sprite;
		asset.name = sprite.name;
	}

	public override  GenerateAssetStruct generateGameObjectAtPosition() {
		GameObject asset = availableGameobject (GameObjectTabOfTypePrefab);
		if (asset == null){
			if (prefab == null) {
				asset = generateAssetWithSprite (spriteForPrefab);
				prefab = asset;
			}
			asset = Instantiate (prefab);
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
			int random = Random.Range (0, 2);
			return (random == 0) ? true : false; 
		}
		return false;
	}

}
