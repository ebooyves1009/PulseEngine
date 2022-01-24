using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Enemy : Character
{
    #region Constants #############################################################

    #endregion

    #region Variables #############################################################

    [SerializeField] private bool _defense;
    [SerializeField] private float _defenseDuration = 10;
    [SerializeField] private Character _target;
    [SerializeField] private float _maxDistance = 10;
    [SerializeField] private float _minDistance = 2;

    private float _defenseChrono;
    private bool _moving;

    #endregion

    #region Statics   #############################################################

    #endregion

    #region Inner Types ###########################################################

    #endregion

    #region Properties ############################################################

    #endregion

    #region Public Functions ######################################################

    public override float GetHit(Vector3 hitMakerPosition, Vector3 hitBoxCenter, float damages, Direction hitDirection = Direction.forward, int hitIntensity = 0)
    {
        float _damages = base.GetHit(hitMakerPosition, hitBoxCenter, damages, hitDirection, hitIntensity);
        _defenseChrono = _defenseDuration;
        Task.Run(async () =>
        {
            await Task.Delay(500);
            _defense = true;
        });
        //TimeManager.BreakTime(0.1f * damages);
        return _damages;
    }

    #endregion

    #region Private Functions #####################################################

    protected override void GetActions()
    {
        base.GetActions();
        DefenseAction?.Invoke(_defense);
    }

    protected override void CalculateDesiredDirection(float deltaTime)
    {
        base.CalculateDesiredDirection(deltaTime);

        if (SuspendInputs)
        {
            DesiredDirection = Vector3.zero;
            return;
        }
        if (_target == null)
        {
            DesiredDirection = Vector3.zero;
            return;
        }
        if (Vector3.Distance(_target.transform.position, transform.position) < _minDistance)
        {
            DesiredDirection = Vector3.zero;
            _moving = false;
            return;
        }
        if (Vector3.Distance(_target.transform.position, transform.position) < _maxDistance && !_moving)
        {
            DesiredDirection = Vector3.zero;
            return;
        }
        DesiredDirection = (_target.transform.position - transform.position).normalized;
        _moving = true;
    }

    #endregion

    #region Jobs      #############################################################

    #endregion

    #region MonoBehaviours ########################################################

    protected override void Update()
    {
        base.Update();
        if (_defense && _defenseChrono > 0)
        {
            _defenseChrono -= Time.deltaTime;
            if (_defenseChrono <= 0)
            {
                _defense = false;
            }
        }
    }

    #endregion
}

