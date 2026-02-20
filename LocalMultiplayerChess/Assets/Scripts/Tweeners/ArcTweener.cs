using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArcTweener : MonoBehaviour, IObjectTweener
{
    [SerializeField] float speed;
    [SerializeField] float height;

    public void MoveTo(Transform transform, Vector3 targetPos)
    {
        float dis = Vector3.Distance(targetPos, transform.position);
        transform.DOJump(targetPos, height, 1, dis / speed);
    }
}
