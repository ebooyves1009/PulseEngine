using PulseEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class TestAsset : MonoBehaviour, IMovable
{
    public event EventHandler OnArrival;
    public Vector3 movingPlace = Vector3.negativeInfinity;
    public float speed = 5;
    public float reachDist = 1;

    public void ArrivedAt()
    {
        if (OnArrival != null)
        {
            OnArrival.Invoke(gameObject, EventArgs.Empty);
        }
    }

    public Task<CommandPath> MoveCommand(Command _cmd, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public void MoveTo(Vector3 position)
    {
        movingPlace = position;
    }

    private void Update()
    {
        if (movingPlace.x != Vector3.negativeInfinity.x)
        {
            //transform.Translate((movingPlace - transform.position).normalized * speed * Time.deltaTime, Space.Self);
            if ((movingPlace - transform.position).sqrMagnitude <= Mathf.Pow(reachDist, 2))
            {
                movingPlace = Vector3.negativeInfinity;
                IMovable mover = this;
                mover.ArrivedAt();
            }
        }
    }
}