using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public TextMesh text_fx_name;
	public GameObject[] fx_prefabs;
	public int index_fx = 0;
	private Ray ray;
	private RaycastHit ray_cast_hit;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetMouseButtonDown(0) ){
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if ( Physics.Raycast (ray.origin, ray.direction, out ray_cast_hit, 1000f) ){
				Instantiate(fx_prefabs[ index_fx ], new Vector3(ray_cast_hit.point.x, ray_cast_hit.point.y, ray_cast_hit.point.z), Quaternion.identity);	
			}
		}
		//Change-FX keyboard..	
		if ( Input.GetKeyDown("z") || Input.GetKeyDown("left") ){
			index_fx--;
			if(index_fx <= -1)
				index_fx = fx_prefabs.Length - 1;
			text_fx_name.text = "[" + (index_fx + 1) + "] " + fx_prefabs[ index_fx ].name;	
		}

		if ( Input.GetKeyDown("x") || Input.GetKeyDown("right")){
			index_fx++;
			if(index_fx >= fx_prefabs.Length)
				index_fx = 0;
			text_fx_name.text = "[" + (index_fx + 1) + "] " + fx_prefabs[ index_fx ].name;
		}

		if ( Input.GetKeyDown("space") ){
			Instantiate(fx_prefabs[ index_fx ], new Vector3(0, 0, 2.0f), Quaternion.identity);	
		}
	}
}
