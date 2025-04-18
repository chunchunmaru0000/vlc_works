﻿namespace vlc_works
{
    public class GameVideo
    {
        public PathUri Game { get; set; }
        public PathUri Stop { get; set; }

        public GameVideo(PathUri game, PathUri stop)
        {
            Game = game;
            Stop = stop;
        }

        public GameVideo Clone() => new GameVideo(Game, Stop);
    }
}
