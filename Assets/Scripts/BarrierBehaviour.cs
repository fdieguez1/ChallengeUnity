using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierBehaviour : MonoBehaviour
{
    Transform[] children;
    private void Start()
    {
        children = transform.GetComponentsInChildren<Transform>();
    }
    private void OnEnable()
    {
        GameController.OnGameStartEvent += BarrierStart;
        GameController.OnWinGameDelegate += BarrierStart;
    }
    private void OnDisable()
    {
        GameController.OnGameStartEvent -= BarrierStart;
        GameController.OnWinGameDelegate -= BarrierStart;
    }
    void BarrierStart()
    {
        if (children != null)
        {
            foreach (Transform child in children)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
