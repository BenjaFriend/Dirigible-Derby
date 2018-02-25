using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Forcer LeftProp, RightProp;

    /// <summary>
    /// How fast the basket can fall with the balloon attached
    /// </summary>
    public float BalloonMaxVelo;

    public float MaxAngle;

    public float MaxPropForce;

    private Rewired.Player _rewired;
    private Rigidbody2D _body;
    private float _initAngularDrag;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _initAngularDrag = _body.angularDrag;
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

    private void Update()
    {
        clampVelo();
        clampRotation();
        Debug.DrawLine(transform.position, (Vector2)transform.position + _body.velocity, Color.green);
    }

    private void clampVelo()
    {
        Vector2 velo = _body.velocity;
        velo.y = Mathf.Clamp(velo.y, -BalloonMaxVelo, float.PositiveInfinity);
        _body.velocity = velo;
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
        setPropForce(LeftProp, data.GetAxis());
    }


    /// <summary>
    /// Called by ReWired when right trigger axis data is updated
    /// </summary>
    private void onRightTriggerUpdate(Rewired.InputActionEventData data)
    {
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

    }

    /// <summary>
    /// Called by ReWired when the deflate button is released
    /// </summary>
    private void onDeflateReleased(Rewired.InputActionEventData data)
    {

    }

    /// <summary>
    /// Called by ReWired when the inflate button is pressed
    /// </summary>
    private void onInflatePressed(Rewired.InputActionEventData data)
    {

    }

    /// <summary>
    /// Called by ReWired when the inflate button is released
    /// </summary>
    private void onInflateReleased(Rewired.InputActionEventData data)
    {

    }

    private void logFormat(string format, params object[] args)
    {
        Debug.LogFormat("[PlayerController] " + format, args);
    }
}
