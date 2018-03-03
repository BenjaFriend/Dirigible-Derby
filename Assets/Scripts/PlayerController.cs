using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Forcer BalloonForcer;
    public Forcer DeflateForcer;
    public Forcer LeftProp, RightProp;

    /// <summary>
    /// How fast the basket can rise with the balloon attached
    /// </summary>
    public float BalloonMaxYVelo;

    /// <summary>
    /// How fast the basket can fall with the balloon attached
    /// </summary>
    public float BalloonMinYVelo;

    /// <summary>
    /// How fast the basket can fall when deflate is pressed
    /// </summary>
    public float DeflateMinYVelo;

    /// <summary>
    /// How fast teh basket can fall when popped
    /// </summary>
    public float PoppedMinYVelo;

    /// <summary>
    /// How fast the MinYVelo changes over time
    /// </summary>
    public float MinYVeloLerpSpeed;

    /// <summary>
    /// Maxiumum amount the baloon can rotate from 0 on either side
    /// </summary>
    public float MaxAngle;

    /// <summary>
    /// Maximum propellor force
    /// </summary>
    public float MaxPropForce;

    /// <summary>
    /// How much force the inflate button applies
    /// </summary>
    public float InflateForce;

    /// <summary>
    /// How much force the deflate button applies
    /// </summary>
    public float DeflateForce;

    private Vector2 _balloonDefaultForce;

    private Rewired.Player _rewired;
    private Rigidbody2D _body;

    public enum BalloonState
    {
        Idle,
        Inflate,
        Deflate,
        Popped
    }
    private BalloonState _state;
    public BalloonState State
    {
        get { return _state; }
    }

    private float _minYVelo, _maxYVelo;
    private float _targetMinYVelo;
    private bool _playerHasControl;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        ResetValues();
    }

    public void ResetValues()
    {
        _minYVelo = BalloonMinYVelo;
        _targetMinYVelo = BalloonMinYVelo;
        _maxYVelo = BalloonMaxYVelo;
        SetState(BalloonState.Idle);
        _playerHasControl = true;
    }

    public void Initialize(int rewiredPlayerID)
    {
        initRewired(rewiredPlayerID);
    }

    private void initRewired(int rewiredPlayerID)
    {
        _rewired = Rewired.ReInput.players.GetPlayer(rewiredPlayerID);

        // register input event listeners
        _rewired.AddInputEventDelegate(
            onLeftTriggerUpdate,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.Update,
            Constants.RewiredInputActions.LeftTrigger);

        _rewired.AddInputEventDelegate(
            onRightTriggerUpdate,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.Update,
            Constants.RewiredInputActions.RightTrigger);

        _rewired.AddInputEventDelegate(
            onRightTriggerUpdate,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.Update,
            Constants.RewiredInputActions.RightTrigger);

        _rewired.AddInputEventDelegate(
            onDeflatePressed,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.ButtonJustPressed,
            Constants.RewiredInputActions.Deflate);

        _rewired.AddInputEventDelegate(
            onDeflateReleased,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.ButtonJustReleased,
            Constants.RewiredInputActions.Deflate);

        _rewired.AddInputEventDelegate(
            onInflatePressed,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.ButtonJustPressed,
            Constants.RewiredInputActions.Inflate);

        _rewired.AddInputEventDelegate(
            onInflateReleased,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.ButtonJustReleased,
            Constants.RewiredInputActions.Inflate);
    }

    /// <summary>
    /// Called when this player's balloon is popped by another player
    /// </summary>
    public void OnPopped()
    {
        SetState(BalloonState.Popped);
    }

    private void Update()
    {
        clampVelo();
        clampRotation();

        Debug.DrawLine(transform.position, (Vector2)transform.position + _body.velocity, Color.green);
    }

    private void clampVelo()
    {
        // lerp _minYVelo to _targetMinYVelo
        _minYVelo = Mathf.Lerp(_minYVelo, _targetMinYVelo, MinYVeloLerpSpeed * Time.deltaTime);

        setBodyVelocityY(Mathf.Clamp(_body.velocity.y, _minYVelo, _maxYVelo));
    }

    private void clampRotation()
    {
        _body.rotation = Mathf.Clamp(_body.rotation, -MaxAngle, MaxAngle);
    }

    /// <summary>
    /// Called by ReWired when left trigger axis data is updated
    /// </summary>
    private void onLeftTriggerUpdate(Rewired.InputActionEventData data)
    {
        if (!_playerHasControl)
            return;

        setPropForce(LeftProp, data.GetAxis());
    }

    /// <summary>
    /// Called by ReWired when right trigger axis data is updated
    /// </summary>
    private void onRightTriggerUpdate(Rewired.InputActionEventData data)
    {
        if (!_playerHasControl)
            return;

        setPropForce(RightProp, data.GetAxis());
    }

    /// <summary>
    /// Sets the Y value of the prop based on the trigger's axis
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="axis"></param>
    private void setPropForce(Forcer prop, float axis)
    {
        prop.Force.y = Mathf.Lerp(0f, MaxPropForce, axis);
    }

    /// <summary>
    /// Called by ReWired when the deflate button is pressed
    /// </summary>
    private void onDeflatePressed(Rewired.InputActionEventData data)
    {
        if (!_playerHasControl)
            return;
        SetState(BalloonState.Deflate);
    }

    /// <summary>
    /// Called by ReWired when the deflate button is released
    /// </summary>
    private void onDeflateReleased(Rewired.InputActionEventData data)
    {
        if (!_playerHasControl)
            return;

        SetState(BalloonState.Idle);
    }

    /// <summary>
    /// Called by ReWired when the inflate button is pressed
    /// </summary>
    private void onInflatePressed(Rewired.InputActionEventData data)
    {
        if (!_playerHasControl)
            return;

        SetState(BalloonState.Inflate);
    }

    /// <summary>
    /// Called by ReWired when the inflate button is released
    /// </summary>
    private void onInflateReleased(Rewired.InputActionEventData data)
    {
        if (!_playerHasControl)
            return;

        SetState(BalloonState.Idle);
    }

    private void setBodyVelocity(float x, float y)
    {
        _body.velocity = new Vector2(x, y);
    }

    private void setBodyVelocityX(float x)
    {
        Vector2 velocity = _body.velocity;
        velocity.x = x;
        _body.velocity = velocity;
    }

    private void setBodyVelocityY(float y)
    {
        Vector2 velocity = _body.velocity;
        velocity.y = y;
        _body.velocity = velocity;
    }

    private void OnDisable()
    {
        removeRewired();
    }

    public void SetState(BalloonState state)
    {
        onStateExit(_state);
        onStateEnter(state);
        _state = state;
    }

    private void onStateEnter(BalloonState state)
    {
        switch (state)
        {
            case BalloonState.Idle:
                break;
            case BalloonState.Inflate:
                if (_body.velocity.y < 0)
                    setBodyVelocityY(_body.velocity.y / 2f);

                _balloonDefaultForce = BalloonForcer.Force;
                BalloonForcer.Force = InflateForce * Vector2.up;
                break;
            case BalloonState.Deflate:
                if (_body.velocity.y > 0)
                    setBodyVelocityY(_body.velocity.y / 2);

                _targetMinYVelo = DeflateMinYVelo;
                DeflateForcer.Force = DeflateForce * Vector2.down;
                break;
            case BalloonState.Popped:
                _playerHasControl = false;
                _targetMinYVelo = PoppedMinYVelo;
                // apply deflate forcer to drop with "weight"
                DeflateForcer.Force = DeflateForce * Vector2.down;
                break;
            default:
                logWarningFormat("No handler for state {0} in _onStateEnter, did you forget to add it to the switch statement?", Enum.GetName(typeof(BalloonState), state));
                break;
        }
    }

    private void onStateExit(BalloonState state)
    {
        switch (state)
        {
            case BalloonState.Idle:
                break;
            case BalloonState.Inflate:
                BalloonForcer.Force = _balloonDefaultForce;
                break;
            case BalloonState.Deflate:
                //TODO: check if the balloon is popped so we don't accidentally set the wrong min y here
                _targetMinYVelo = BalloonMinYVelo;

                DeflateForcer.Force = Vector2.zero;
                break;
            case BalloonState.Popped:
                _playerHasControl = true;
                break;
            default:
                logWarningFormat("No handler for state {0} in _onStateExit, did you forget to add it to the switch statement?", Enum.GetName(typeof(BalloonState), state));
                break;
        }
    }

    /// <summary>
    /// Unhook rewired events from this player, so that we don't get error when
    /// this object is deleted
    /// </summary>
    private void removeRewired()
    {
        // register input event listeners
        _rewired.RemoveInputEventDelegate(
            onLeftTriggerUpdate,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.Update,
            Constants.RewiredInputActions.LeftTrigger);

        _rewired.RemoveInputEventDelegate(
            onRightTriggerUpdate,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.Update,
            Constants.RewiredInputActions.RightTrigger);

        _rewired.RemoveInputEventDelegate(
            onRightTriggerUpdate,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.Update,
            Constants.RewiredInputActions.RightTrigger);

        _rewired.RemoveInputEventDelegate(
            onDeflatePressed,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.ButtonJustPressed,
            Constants.RewiredInputActions.Deflate);

        _rewired.RemoveInputEventDelegate(
            onDeflateReleased,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.ButtonJustReleased,
            Constants.RewiredInputActions.Deflate);

        _rewired.RemoveInputEventDelegate(
            onInflatePressed,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.ButtonJustPressed,
            Constants.RewiredInputActions.Inflate);

        _rewired.RemoveInputEventDelegate(
            onInflateReleased,
            Rewired.UpdateLoopType.Update,
            Rewired.InputActionEventType.ButtonJustReleased,
            Constants.RewiredInputActions.Inflate);
    }

    private void logFormat(string format, params object[] args)
    {
        Debug.LogFormat("[PlayerController] " + format, args);
    }

    private void logWarningFormat(string format, params object[] args)
    {
        Debug.LogWarningFormat("[PlayerController] " + format, args);
    }
}
