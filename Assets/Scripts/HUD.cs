using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text txtHealth;

    // Update is called once per frame
    void Update()
    {
        txtHealth.text = GameController.Instance.Health.ToString();
    }
}
