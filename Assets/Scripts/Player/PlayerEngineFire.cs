using UnityEngine;

/// <summary>
/// Set the emission rate over time of the left and right
/// engines based on the rewired input values
/// </summary>
public class PlayerEngineFire : MonoBehaviour
{
	#region Fields

    /// <summary>
    /// The left engine partcile system
    /// </summary>
    public ParticleSystem LeftEngine;

    /// <summary>
    /// The right engine particle system
    /// </summary>
    public ParticleSystem RightEngine;

	/// <summary>
	/// The scale that will be added to the flame partciles emmision rate
	/// </summary>
	public float FlameScale = 10.0f;

	/// <summary>
	/// The rewired player
	/// </summary>
	private Rewired.Player _rewired;

	/// <summary>
	/// The baselnie emmision for the particle systems.
	/// </summary>
	private float _startingEmmision;

	/// <summary>
	/// The left emmision module of the particle system
	/// </summary>
	private ParticleSystem.EmissionModule _leftEmmisionModule;

	/// <summary>
	/// The right emission module of the particle system
	/// </summary>
	private ParticleSystem.EmissionModule _rightEmmisionModule;

	#endregion

	/// <summary>
	/// Gets the baseline rate over time emmmision
	/// </summary>
	private void Start()
	{
		if (LeftEngine != null) 
		{
			_leftEmmisionModule = LeftEngine.emission;
			_startingEmmision = LeftEngine.emission.rateOverTimeMultiplier;
		}

		if (RightEngine != null) 
		{
			_rightEmmisionModule = RightEngine.emission;
		}
	}

	/// <summary>
	/// Setup rewired and add the input event delegates. 
	/// </summary>
	/// <param name="rewiredPlayerID">Rewired player ID number.</param>
	public void InitRewired(int rewiredPlayerID)
	{
		_rewired = Rewired.ReInput.players.GetPlayer(rewiredPlayerID);

		if (_rewired == null) 
		{
			Debug.Log ("Reiwred player " + rewiredPlayerID.ToString() + " is null!");
			return; 
		}

		// register input event listeners
		_rewired.AddInputEventDelegate(
			setLeftEngineEmmision,
			Rewired.UpdateLoopType.Update,
			Rewired.InputActionEventType.Update,
			Constants.RewiredInputActions.LeftTrigger);

		_rewired.AddInputEventDelegate(
			setRightEngineEmmision,
			Rewired.UpdateLoopType.Update,
			Rewired.InputActionEventType.Update,
			Constants.RewiredInputActions.RightTrigger);
	}

	/// <summary>
	/// Remove the rewired input event delegates
	/// </summary>
    private void OnDisable()
    {
		if (_rewired == null) { return; }

        // Un-hook rewired events
		_rewired.RemoveInputEventDelegate(
			setLeftEngineEmmision,
			Rewired.UpdateLoopType.Update,
			Rewired.InputActionEventType.Update,
			Constants.RewiredInputActions.LeftTrigger);

		_rewired.RemoveInputEventDelegate(
			setRightEngineEmmision,
			Rewired.UpdateLoopType.Update,
			Rewired.InputActionEventType.Update,
			Constants.RewiredInputActions.RightTrigger);
    }

    /// <summary>
    /// Set the emmision.rateOverTimeMultiplier of the left engine based on the axis
	/// of the reiwred input data
    /// </summary>
    /// <param name="data">Rewired input data</param>
	private void setLeftEngineEmmision(Rewired.InputActionEventData data)
    {
		// Set the left engine emmision
		if(LeftEngine == null) { return; } 

		float value = _startingEmmision + data.GetAxis () * FlameScale;;

		_leftEmmisionModule.rateOverTimeMultiplier = value;

	}

	/// <summary>
	/// Set the emmision.rateOverTimeMultiplier of the right engine based on the axis
	/// of the rewired input data
	/// </summary>
	/// <param name="data">Rewired input data</param>
	private void setRightEngineEmmision(Rewired.InputActionEventData data)
    {
		// Set the right engine emmission
		if(RightEngine == null) { return; }

		float value = _startingEmmision + data.GetAxis () * FlameScale;;

		_rightEmmisionModule.rateOverTimeMultiplier = value;
	}
	
}
