namespace DAL1.RBSS_CS.Bifunctors
{
    public class AddBifunctor : IBifunctor
    {
        public byte[] Hash { get; set; }

        public AddBifunctor()
        {
            Hash = new byte[]{ 0 };
        }

        public AddBifunctor(byte[] hash)
        {
            Hash = hash;
        }


        private void Add(byte[] b1, byte[] b2)
        {
            Hash = b1.ToArray();
            int remainder = 0;
            int i;
            for (i = 0; i < b2.Length; i++)
            {
                int result = Hash[i] + b2[i] + remainder;
                Hash[i] = (byte)(result&0xFF);
                remainder = (result >> 8) & 0xFF;
            }

            if (i < Hash.Length)
            {
                Hash[i] += (byte)remainder;
            }
        }
        public void Apply(byte[] bytes)
        {
            if (Hash.Length >= bytes.Length ) Add(Hash, bytes);
            else Add(bytes, Hash);
        }

        public void Apply(IBifunctor other)
        {
            Apply(other.Hash);
        }

        public IBifunctor GetNewEmpty()
        {
            return new AddBifunctor();
        }
    }
}
