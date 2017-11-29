using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{

    private GameObject _mEnemy;
    private Animator _mAnimator = null;
    private bool _isActive = false;
    private Vector3 _mStartPos = Vector3.zero;
    private AudioSource audioSrc;

    public ParticleSystem muzzleFlashParticle;
    public float upTime = 3.0f;
    public float shootTime = 2.0f;
    public float downTime = 2.0f;

    // Use this for initialization
    void Awake()
    {
        _mEnemy = transform.GetChild(0).gameObject;
        _mAnimator = _mEnemy.GetComponent<Animator>();
        _mStartPos = _mEnemy.transform.position;
        audioSrc = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        _isActive = true;
        _mEnemy.transform.position = _mStartPos;
        MoveUpwards();
        Invoke("MoveDownards", shootTime);
    }

    private void MoveUpwards()
    {
        Vector3 enemyPos = _mEnemy.transform.position;
        enemyPos.y += 4;

        iTween.MoveTo(_mEnemy, iTween.Hash("y", enemyPos.y, "time", upTime, "onComplete", "OnUpComplete", "onCompleteTarget", gameObject));
    }

    internal void Hit()
    {
        _mAnimator.SetTrigger("hit");
        muzzleFlashParticle.Stop();
        audioSrc.Stop();
    }

    private void MoveDownards()
    {
        _mAnimator.SetBool("shoot", false);
        muzzleFlashParticle.Stop();
        audioSrc.Stop();

        Vector3 enemyPos = _mEnemy.transform.position;
        enemyPos.y -= 4;

        iTween.MoveTo(_mEnemy, iTween.Hash("y", enemyPos.y, "time", downTime, "onComplete", "OnDownComplete", "onCompleteTarget", gameObject));
    }

    void OnDownComplete()
    {
        _isActive = false;
    }

    void OnUpComplete()
    {
        _mAnimator.SetBool("shoot", true);
        muzzleFlashParticle.Play();
        audioSrc.Play();
    }

    public bool IsActive
    {
        get { return _isActive; }
    }
}
