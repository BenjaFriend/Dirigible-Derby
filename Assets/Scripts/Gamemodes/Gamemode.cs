using System;

/// <summary>
/// All types of gamemodes. Update this, and Create() below when you extend Gamemode
/// </summary>
public enum GamemodeType
{
    Elimination
}

/// <summary>
/// Class for gamemode-related logic. Managed by GameSceneController
/// </summary>
public abstract class Gamemode
{
    protected GameSceneController gameSceneController;
    protected GamemodeType gamemodeType;

    protected GameManager gameManager
    {
        get { return GameManager.Instance; }
    }

    public GamemodeType Type
    {
        get { return gamemodeType; }
    }

    /// <summary>
    /// Creates a new gamemode object of the provided type
    /// </summary>
    public static Gamemode Create(GamemodeType type, GameSceneController parent)
    {
        switch (type)
        {
            case GamemodeType.Elimination:
                return new EliminationGamemode(parent);
            default:
                throw new NotImplementedException("Can not create a gamemode object of type \"" + Enum.GetName(typeof(GamemodeType), type) + "\". Did you forget to add it to the switch statement?");
        }
    }

    public Gamemode(GameSceneController parent, GamemodeType type)
    {
        gameSceneController = parent;
        gamemodeType = type;
    }

    /// <summary>
    /// Prepares the gamemode
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Called in GameSceneController Update()
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Called when leaving the scene
    /// </summary>
    public abstract void Unload();
}