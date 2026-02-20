using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineTweener : MonoBehaviour, IObjectTweener
{
    [SerializeField] float speed;

    public void MoveTo(Transform transform, Vector3 targetPos)
    {
        float dis = Vector3.Distance(targetPos, transform.position);
        transform.DOMove(targetPos, dis / speed);
    }

}
