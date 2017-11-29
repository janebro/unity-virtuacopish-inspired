using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text txtHealth;
    public Text txtTimer;
    public RectTransform panelGameOver;
    public Text txtGameOver;


    void Start()
    {
        GameController.Instance.GameOverEvent += OnGameOverEvent;
    }

    private void OnGameOverEvent(object sender, EventArgs e)
    {
        panelGameOver.gameObject.SetActive(true);
        txtGameOver.text = GameController.Instance.isWin ? "YOU WIN!" : "YOU LOSE!";
    }

    // Update is called once per frame
    void Update()
    {
        txtHealth.text = GameController.Instance.Health.ToString();
        txtTimer.text = GameController.Instance.Timer.ToString();
    }
}
