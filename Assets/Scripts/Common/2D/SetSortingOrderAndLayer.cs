using UnityEngine;

public class SetSortingOrderAndLayer : MonoBehaviour 
{

	public int sortingOrder = 0;
	public string layerName = "Default";

	void Start()
	{
		GetComponent<Renderer>().sortingOrder = sortingOrder;
		GetComponent<Renderer>().sortingLayerName = layerName;
	}

}
