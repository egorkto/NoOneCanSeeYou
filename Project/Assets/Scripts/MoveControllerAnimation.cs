using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControllerAnimation : MonoBehaviour
{
    private Animator _animator;
    private MoveController _moveController;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _moveController = GetComponent<MoveController>();
    }
    private void Update()
    {
        _animator.SetFloat("speed", _moveController.animValue,3f,0.3f);
    }
}
