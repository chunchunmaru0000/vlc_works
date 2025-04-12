namespace vlc_works.Classes
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
    }
}
