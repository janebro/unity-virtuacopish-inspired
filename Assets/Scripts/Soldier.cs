using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour {

    private GameObject _mEnemy;
    private Animator _mAnimator = null;
    private bool _isActive = false;

    public float upTime = 3.0f;
    public float shootTime = 2.0f;
    public float downTime = 2.0f;

	// Use this for initialization
	void Awake ()
    {
        _mEnemy = transform.GetChild(0).gameObject;
        _mAnimator = _mEnemy.GetComponent<Animator>();
    }

    public void Activate()
    {
        _isActive = true;
        MoveUpwards();

        _mAnimator.SetBool("shoot", true);

        Invoke("MoveDownards", shootTime);
    }

    private void MoveUpwards()
    {
        Vector3 enemyPos = _mEnemy.transform.position;
        enemyPos.y += 4;

        iTween.MoveTo(_mEnemy, enemyPos, upTime);
    }

    private void MoveDownards()
    {
        _mAnimator.SetBool("shoot", false);

        Vector3 enemyPos = _mEnemy.transform.position;
        enemyPos.y -= 4;

        iTween.MoveTo(_mEnemy, iTween.Hash("y", enemyPos.y, "time", downTime, "onComplete", "OnDownComplete", "onCompleteTarget", gameObject));
    }

    void OnDownComplete()
    {
        _isActive = false;
    }

    public bool IsActive
    {
        get { return _isActive; }
    }

    // Update is called once per frame
    void Update ()
    {
	    	
	}
}
