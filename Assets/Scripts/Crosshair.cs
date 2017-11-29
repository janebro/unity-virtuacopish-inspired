using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

    private AudioSource audioSrc;

	// Use this for initialization
	void Start ()
    {
        //Hide the mouse cursor
        Cursor.visible = false;
        GameController.Instance.GameOverEvent += OnGameOverEvent;
        audioSrc = GetComponent<AudioSource>();
	}

    private void OnGameOverEvent(object sender, EventArgs e)
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
        transform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            audioSrc.Play();

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
