using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        //Hide the mouse cursor
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Enemy")
                {
                    hit.transform.parent.GetComponent<Soldier>().Hit();
                }
            }
        }
	}
}
