using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static class PlayerLobby
    {
        public const int MaxPlayers = 4;
    }

    public static class RewiredInputActions
    {
        public const string JoinGame = "JoinGame";
        public const string Deflate = "Deflate";
        public const string Inflate = "Inflate";
        public const string LeftTrigger = "LeftTrigger";
        public const string RightTrigger = "RightTrigger";
    }

    public static class PlayerPhysicsLayers
    {
        public const int Player_0_Layer = 8;
        public const int Player_1_Layer = 9;
        public const int Player_2_Layer = 10;
        public const int Player_3_Layer = 11;
    }

    public static class Mixer
    {
        public const string Path = "Audio/DD_AudioMixer";
        public static class MixerGroups
        {
            public static class Master
            {
                public const string Name = "Master";
                public static class SFX
                {
                    public const string Name = Master.Name + "/SFX";
                }

                public static class BackgroundMusic
                {
                    public const string Name = Master.Name + "/Background Music";
                }
            }
        }
    }

    public static class AudioPoolSize
    {
        public const uint Music = 2;
        public const uint SFX = 10;
    }

    public static class Scenes
    {
        public const string MainMenu = "MainMenu";
        public const string PlayerLobby = "PlayerLobby";
        public const string Playtesting = "Playtesting";
        public const string Credits = "Credits";
    }

    public static class Tags
    {
        public const string SpawnPoint = "Spawn Point";
    }

    public static class Gamemodes
    {
        public static class Elimination
        {
            public const int Lives = 1;
        }
    }

}
