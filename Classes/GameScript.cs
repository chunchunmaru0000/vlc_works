namespace vlc_works015
{
    public class GameScript
    {
        public GameType GameType { get; set; }
        public long Lvl { get; set; }
        public long Prize { get; set; }
        public long Price { get; set; }

        public GameScript(GameType gameType, long lvl, long prize, long price)
        {
            GameType = gameType;
            Lvl = lvl;
            Prize = prize;
            Price = price;
        }

        public GameScript Clone() => new GameScript(GameType, Lvl, Prize, Price);

        public override string ToString() =>
            $"{GameType.View()} {Lvl}; {Prize}; {Price}";
    }
}
