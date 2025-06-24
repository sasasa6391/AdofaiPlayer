using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Planet : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [SerializeField] private GameObject ring;

    public float angleSpeed = 1f;

    public float angle = 0.0f;

    public float tickAngle = 0.0f;
    private bool _isRotate = false;

    private Tweener ringTween = null;

    public void SetRotate(bool value)
    {

        if(value == true)
        {
            ring.gameObject.SetActive(false);
        }
        else
        {
            ring.gameObject.SetActive(true);
            ringTween?.Kill();
            ringTween = DOVirtual.Float(1.4f, 1.75f, 0.2f, x =>
            {
                ring.transform.localScale = Vector3.one * x;
            });
        }
        _isRotate = value;
    }

    /*
    void Update()
    {
    }
    */

    public void UpdatePlanet()
    {
        if (_isRotate == true)
        {

            tickAngle += Time.deltaTime * angleSpeed * Conductor.bpm / 60.0f;

            if (PlanetController.IsCW == false)
            {
                angle -= Time.deltaTime * angleSpeed * Conductor.bpm / 60.0f;
                if (angle <= -360.0f)
                {
                    angle += 360.0f;
                }
            }
            else
            {
                angle += Time.deltaTime * angleSpeed * Conductor.bpm / 60.0f;
                if (angle >= 360.0f)
                {
                    angle -= 360.0f;
                }
            }


            float rad = angle * Mathf.Deg2Rad;

            transform.position = playerTransform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * 1.5f;
        }
    }

}