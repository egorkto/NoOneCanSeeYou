using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _speed;

    private Vector3 _direction;

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";

    void Update()
    {
        _direction = new Vector3(Input.GetAxis(horizontal), 0, Input.GetAxis(vertical));
        _controller.Move(_direction * _speed * Time.deltaTime);
    }
}
