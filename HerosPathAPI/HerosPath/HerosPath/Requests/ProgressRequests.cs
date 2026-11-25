namespace HerosPath.Requests
{
    public class SaveProgressRequest
    {
        public string Token { get; set; }
        public int LevelId { get; set; }
        public int Stars { get; set; }
        public int Moves { get; set; }
    }
}