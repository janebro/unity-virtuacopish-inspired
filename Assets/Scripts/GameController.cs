using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    protected GameController() { } // guarantee this will be always a singleton only - can't use the constructor!

    private int _mHealth = 100;

    public void SetDamage(int damage)
    {
        _mHealth -= damage;

        if (_mHealth < 0)
        {
            _mHealth = 0;

            //TODO: Player death logic
        }
    }

    public int Health
    {
        get
        {
            return _mHealth;
        }
    }
}
