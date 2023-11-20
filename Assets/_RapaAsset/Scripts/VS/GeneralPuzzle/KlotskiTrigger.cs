using DG.Tweening;
using UnityEngine;

public class KlotskiTrigger : MonoBehaviour
{
    public enum Direction
    {
        X_Right,
        X_Left,
        Z_Forward,
        Z_Back
    }

    public Direction MoveDirection;

    private void Move()
    {
        switch (MoveDirection)
        {
            case Direction.X_Right:
                transform.parent.DOLocalMoveX(1, 1)
                    .SetRelative(true);
                break;

            case Direction.X_Left:
                transform.parent.DOLocalMoveX(-1, 1)
                    .SetRelative(true);
                break;

            case Direction.Z_Forward:
                transform.parent.DOLocalMoveZ(1, 1)
                    .SetRelative(true);
                break;

            case Direction.Z_Back:
                transform.parent.DOLocalMoveZ(-1, 1)
                    .SetRelative(true);
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!Player.Instance.Grounded)
                Move();
        }
    }
}