using UnityEngine;
using System.Collections;

public class GenerateAssetStruct : System.Object
{
	public GameObject generateAsset;
	public int code;
}


abstract public class parralaxAssetGenerator : MonoBehaviour {

	abstract public void Clear ();

	abstract public GenerateAssetStruct generateGameObjectWithCode(int code);

	abstract public GenerateAssetStruct generateGameObjectAtPosition();


	public System.Random random;

	public float randomRange (float min, float max){
		float factor = random.Next () / int.MaxValue;
		return factor * (max - min) + min;
	}
}
