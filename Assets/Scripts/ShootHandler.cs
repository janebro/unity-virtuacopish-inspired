using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootHandler : MonoBehaviour
{
    public void ShootEvent()
    {
        int doShoot = Random.Range(0, 2);

        if (doShoot > 0)
        {
            int damage = Random.Range(0, 6);
            GameController.Instance.SetDamage(damage);
        }
    }
}
