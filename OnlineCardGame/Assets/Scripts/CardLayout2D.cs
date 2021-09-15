using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardLayout2D : PhotonView
{

    public float distanceBetween;
    [SerializeField]
    float yScale;
    [SerializeField]
    float angle;
    [HideInInspector]
    public Quaternion oldRotation;
    public bool isMaster;

    public void Start()
    {
        FixCardLayout();
    }

    private void Update()
    {

    }
    public void FixCardLayout()
    {
        if (oldRotation == null && transform.childCount > 0)
            oldRotation = transform.GetChild(0).rotation;

        float tempX = (transform.childCount % 2 == 0 ? -(transform.childCount / 2) * distanceBetween + (0.5f * distanceBetween)
    : -(transform.childCount / 2) * distanceBetween);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            float y = -(Mathf.Pow(tempX, 2.0f)) * yScale;
            child.localPosition = Vector3.zero;
            Vector3 pos = child.localPosition;
            pos.x = tempX;
            pos.y = y;
            pos.z += i * 0.05f;
            child.localPosition = pos;
            child.transform.rotation = oldRotation;
            child.RotateAroundLocal(Vector3.forward, angle * -tempX);
            tempX += distanceBetween;
        }
    }
    public void FixCardLayout(Transform skipThis)
    {
        if (oldRotation == null && transform.childCount > 0)
            oldRotation = transform.GetChild(0).rotation;

        float tempX = (transform.childCount % 2 == 0 ? -(transform.childCount / 2) * distanceBetween + (0.5f * distanceBetween)
    : -(transform.childCount / 2) * distanceBetween);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (skipThis.gameObject != child.gameObject)
            {
                float y = -(Mathf.Pow(tempX, 2.0f)) * yScale;
                child.localPosition = Vector3.zero;
                Vector3 pos = child.localPosition;
                pos.x = tempX;
                pos.y = y;
                pos.z += i * 0.05f;
                child.localPosition = pos;
                child.transform.rotation = oldRotation;
                child.RotateAroundLocal(Vector3.forward, angle * -tempX);
            }
            tempX += distanceBetween;
        }
    }


    public void SortCard()
    {
        Transform[] children = new Transform[transform.childCount]; 
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }
        int[] oldIndex = new int[transform.childCount];
        int[] newIndex = new int[transform.childCount];
        if (isMaster)
        {
            children = children.OrderBy(child => child.transform.position.x).ToArray();
        }
        else
        {
            children = children.OrderByDescending(child => child.transform.position.x).ToArray();
        }
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }

}
