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

    public GameObject Balloon;

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

    /// <summary>
    /// Whether or not this controller is a duplicate of another player
    /// </summary>
    private bool _isDuplicate;

    /// <summary>
    /// The player this controller is a duplicate of, if it is a duplicate
    /// </summary>
    private PlayerController _parent;

    /// <summary>
    /// Duplicate player controllers, indexed by quad
    /// 
    /// 3 0
    /// 2 1
    /// </summary>
    private PlayerController[] _dupes;

    /// <summary>
    /// The current screenwrap quad the player is in
    /// </summary>
    private int _currentQuad;

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
        _isDuplicate = false;
        _currentQuad = getCurrentScreenWrapQuad();
        initDuplicates();
        Balloon.SetActive(true);
    }

    /// <summary>
    /// Initializes this PlayerController as a duplicate controller for a parent PlayerController
    /// </summary>
    public void InitializeDuplicate(PlayerController parent, int quad)
    {
        _parent = parent;
        _isDuplicate = true;
        _playerHasControl = false;

        _body.isKinematic = true;

        BalloonForcer.enabled = false;
        DeflateForcer.enabled = false;
        LeftProp.enabled = false; // we might have to do something about particle systems for left/right props, probably just make sure that they are controlled by the PlayerController
        RightProp.enabled = false;

        Balloon.SetActive(true);

        _currentQuad = quad;
        updateDuplicate();
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
    /// Initializes duplicate players for each screen wrap quad
    /// </summary>
    private void initDuplicates()
    {
        _dupes = new PlayerController[4];
        for (int i = 0; i < _dupes.Length; i++)
        {
            GameObject dupe = Instantiate(this.gameObject); // clone self
            dupe.name = transform.name + "(Quad " + i + ")";
            dupe.transform.position = transform.position;
            _dupes[i] = dupe.GetComponent<PlayerController>();
            _dupes[i].InitializeDuplicate(this, i);
            if (i == _currentQuad)
                dupe.SetActive(false);
        }
    }

    /// <summary>
    /// Returns the current quad the player is in
    /// </summary>
    /// <returns></returns>
    private int getCurrentScreenWrapQuad()
    {
        Vector2 pos = Camera.main.WorldToViewportPoint(transform.position);
        return getQuadAtPoint(pos);
    }

    /// <summary>
    /// Returns the quad that contains the given point in viewport space
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private int getQuadAtPoint(Vector2 point)
    {
        point -= new Vector2(0.5f, 0.5f);

        bool isRight = (point.x >= 0 && point.x < 1); // whether or not the player is on the right side of the screen
        bool isUpper = (point.y >= 0 && point.y < 1); // whether or not the player is in the upper part of the screen

        /*  _ _
         * |3|0|
         * |2|1|
         */

        if (isRight && isUpper) // quadrant 0
            return 0;
        else if (isRight && !isUpper) // quad 1
            return 1;
        else if (!isRight && !isUpper) // quad 2
            return 2;
        else // quad 4
            return 3;
    }

    /// <summary>
    /// Returns the provided viewport coordinates in the provided quad space
    /// </summary>
    /// <param name="viewportPos"></param>
    /// <returns></returns>
    private Vector2 viewportToQuadSpace(Vector2 viewportPos, int quad)
    {
        switch (quad)
        {
            case 0:
                return viewportPos + new Vector2(-0.5f, -0.5f);
            case 1:
                return viewportPos + new Vector2(-0.5f, 0.5f);
            case 2:
                return viewportPos + new Vector2(0.5f, 0.5f);
            case 3:
                return viewportPos + new Vector2(0.5f, -0.5f);
            default:
                logErrorFormat("Current quadrant value out of range! Value: {0}", quad);
                return Vector2.zero;
        }
    }

    /// <summary>
    /// Returns the provided quad coordinates in the viewport space
    /// </summary>
    /// <param name="viewportPos"></param>
    /// <returns></returns>
    private Vector2 quadToViewportSpace(Vector2 quadPos, int quad)
    {
        switch (quad)
        {
            case 0:
                return quadPos - new Vector2(-0.5f, -0.5f);
            case 1:
                return quadPos - new Vector2(-0.5f, 0.5f);
            case 2:
                return quadPos - new Vector2(0.5f, 0.5f);
            case 3:
                return quadPos - new Vector2(0.5f, -0.5f);
            default:
                logErrorFormat("Current quadrant value out of range! Value: {0}", quad);
                return Vector2.zero;
        }
    }

    /// <summary>
    /// Returns the current position of the player relative to its current screen wrap quadrant
    /// </summary>
    /// <returns></returns>
    private Vector2 getCurrentQuadPos()
    {
        Vector2 pos = Camera.main.WorldToViewportPoint(transform.position);
        return viewportToQuadSpace(pos, _currentQuad);
    }

    private void setCurrentQuad(int quad)
    {
        // translate current quad pos to new quad
        Vector2 quadPos = getCurrentQuadPos();
        Vector2 viewportPos = quadToViewportSpace(quadPos, quad);
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewportPos);
        worldPos.z = transform.position.z;
        transform.position = worldPos;
        _currentQuad = quad;
    }

    /// <summary>
    /// Called when this player's balloon is popped by another player
    /// </summary>
    public void OnPopped()
    {
        Balloon.SetActive(false);

        if (_isDuplicate)
        {
            _parent.OnPopped();
            return;
        }

        SetState(BalloonState.Popped);
    }

    private void Update()
    {
        int lastQuad = _currentQuad;
        _currentQuad = getCurrentScreenWrapQuad();

        if (_isDuplicate)
        {
            updateDuplicate();
            return;
        }

        clampVelo();
        clampRotation();
        updateScreenWrap(lastQuad);

        Debug.DrawLine(transform.position, (Vector2)transform.position + _body.velocity, Color.green);
    }

    private void updateDuplicate()
    {
        // set wrapped position & rotation
        Vector2 quadPos = _parent.getCurrentQuadPos();
        int parentQuad = _parent._currentQuad;
        Vector2 viewportPos = quadToViewportSpace(quadPos, _currentQuad);
        Vector3 pos = Camera.main.ViewportToWorldPoint(viewportPos);
        pos.z = _parent.transform.position.z;
        transform.position = pos;

        transform.rotation = _parent.transform.rotation;
    }

    /// <summary>
    /// Updates screen wrapping for the player (not duplicates)
    /// </summary>
    private void updateScreenWrap(int lastQuad)
    {
        // wrap the player around to the next side of the screen if they go off one of the sides
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 currentViewportPos = viewportPos;
        if (viewportPos.x < 0)
        {
            viewportPos.x += 1;
        }
        else if (viewportPos.x >= 1)
        {
            viewportPos.x -= 1;
        }

        if (viewportPos.y < 0)
        {
            viewportPos.y += 1;
        }
        else if (viewportPos.y >= 1)
        {
            viewportPos.y -= 1;
        }

        if(viewportPos != currentViewportPos)
        {
            setCurrentQuad(getQuadAtPoint(viewportPos));
        }

        // cycle dupes appropriately if player changed quads
        if (_currentQuad != lastQuad)
        {
            _dupes[lastQuad].gameObject.SetActive(true);
            _dupes[lastQuad].updateDuplicate();
            _dupes[_currentQuad].gameObject.SetActive(false);
        }
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
        if (!_isDuplicate)
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

    private void logErrorFormat(string format, params object[] args)
    {
        Debug.LogErrorFormat("[PlayerController] " + format, args);
    }
}
