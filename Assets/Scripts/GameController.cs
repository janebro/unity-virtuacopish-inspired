using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    protected GameController() { } // guarantee this will be always a singleton only - can't use the constructor!

    private int _mHealth = 100;
    private int _mTimer = 30;
    private bool _mGameOver = false;
    public EventHandler GameOverEvent;

    private void OnGameOver()
    {
        if (GameOverEvent != null)
            GameOverEvent(this, EventArgs.Empty);
    }

    private void Start()
    {
        InvokeRepeating("Count", 0.0f, 1.0f);
    }

    void Count()
    {
        if (_mTimer == 0)
        {
            _mGameOver = true;
            CancelInvoke("Count");
            OnGameOver();
        }
        else
        {
            _mTimer--;
        }
    }

    public void SetDamage(int damage)
    {
        if (_mGameOver)
            return;

        _mHealth -= damage;

        if (_mHealth < 0)
        {
            _mHealth = 0;
            _mGameOver = true;
            CancelInvoke("Count");
            OnGameOver();
        }
    }

    public bool IsGameOver
    {
        get { return _mGameOver; }
    }

    public bool isWin
    {
        get
        {
            if (Health <= 0)
                return false;

            return true;
        }
    }

    public int Health
    {
        get { return _mHealth; }
    }

    public int Timer
    {
        get { return _mTimer; }
    }
}
