using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ArrowLocal : MonoBehaviour
{

    private Camera mainCamera;
    public Transform arrowhead;
    public Transform arrowbody;

    public void Start()
    {
        mainCamera = Camera.main;
    }
    public void arrowBetween(Vector2 start, Vector2 end)
    {
        toggleArrow(true);

        var length = (end - start).magnitude;

        //scale
        var scale = arrowbody.transform.localScale;
        scale.x = length - arrowhead.localScale.x;
        arrowbody.transform.localScale = scale;

        //rotate
        var angle = Vector2.SignedAngle(Vector2.right, (end - start).normalized);
        var rotation = arrowbody.transform.rotation;
        rotation.eulerAngles = new Vector3(0, 0, angle);
        arrowbody.transform.rotation = rotation;
        arrowhead.rotation = rotation;

        //position
        var position = arrowbody.transform.position;
        position = (((end - start).normalized) * (length - arrowhead.localScale.x) / 2) + start;
        position.z = arrowbody.transform.position.z;
        arrowbody.transform.position = position;

        var position2 = arrowhead.position;
        position2 = (((end - start).normalized) * (length - arrowhead.localScale.x / 2)) + start;
        position2.z = arrowhead.position.z;
        arrowhead.position = position2;
    }
    public void toggleArrow(bool showArrow)
    {

        arrowhead.gameObject.SetActive(showArrow);
        arrowbody.gameObject.SetActive(showArrow);
    }

}
