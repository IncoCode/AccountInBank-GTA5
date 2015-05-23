namespace AccountInBank.Model
{
    internal class CharacterStat
    {
        public int Id { get; set; }
        public int Deaths { get; set; }
        public int Arrests { get; set; }

        public CharacterStat( int id, int deaths, int arrests )
        {
            this.Id = id;
            this.Deaths = deaths;
            this.Arrests = arrests;
        }
    }
}