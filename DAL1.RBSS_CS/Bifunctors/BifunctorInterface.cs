namespace DAL1.RBSS_CS.Bifunctors
{
    public interface IBifunctor
    {
        byte[] Hash { get; set; }
        public void Apply(byte[] bytes);
        public void Apply(IBifunctor other);

        public IBifunctor GetNewEmpty();
    }
}
