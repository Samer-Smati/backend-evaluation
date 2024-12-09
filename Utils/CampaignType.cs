namespace PfeProject.Utils
{
    public static class CampaignType
    {
        public const string Quarterly = "Quarterly";
        public const string Annual = "Annual";
        public const string Weekly = "Weekly";

        public static readonly List<string> AllTypes = new List<string>
        {
            Quarterly, Annual, Weekly
        };
    }

}
