using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DAL1.RBSS_CS.Bifunctors;
using DAL1.RBSS_CS.Hashfunction;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models.RBSS_CS;
using Xunit;

namespace Tests.RBSS_CS
{
    public class BifunctorTests
    {

        byte[] HostToNetworkBytes(byte[] arr)
        {
            var na = arr.ToArray();
            if (BitConverter.IsLittleEndian) Array.Reverse(na);
            return na;
        }

        [Fact]
        public void XorTest()
        {
            XorBifunctor bifunctor = new();

            int x1 = 5432;
            int x2 = 56154462;
            var x1b = BitConverter.GetBytes(x1);
            var x2b = BitConverter.GetBytes(x2);
            bifunctor.Apply(x1b);

            // Assert.Equal("4ZS4", Convert.ToBase64String(HostToNetworkBytes(x1b)));
            

            Assert.Equal(x1, BitConverter.ToInt32(bifunctor.Hash));
            bifunctor.Apply(x2b);
            Assert.Equal(x1 ^ x2, BitConverter.ToInt32(bifunctor.Hash));
            Assert.Equal("AAAAAA==", Convert.ToBase64String(BitConverter.GetBytes(0)));
            Assert.Equal("/////w==", Convert.ToBase64String(BitConverter.GetBytes(0xFFFFFFFF)));
            Assert.Equal("AAAVOA==", Convert.ToBase64String(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(x1))));
            Assert.Equal("AAAVOA==", Convert.ToBase64String(HostToNetworkBytes(x1b)));
            Assert.Equal("A1jZXg==", Convert.ToBase64String(HostToNetworkBytes(x2b)));
            Assert.Equal("A1jMZg==", Convert.ToBase64String(HostToNetworkBytes(bifunctor.Hash)));
        }

        [Fact]
        public void XorTestStableHash()
        {
            StableHash hash = new StableHash();
            
            SimpleDataObject sdo1 = new("ape", 0, "string");
            SimpleDataObject sdo1b = new("ape", 86475, "string");
            SimpleDataObject sdo1c = new("ape", 0, "updated");
            SimpleDataObject sdo2 = new("bee", 0, "string");
            Assert.Equal("g2SIJg==", Convert.ToBase64String(HostToNetworkBytes(hash.Hash(sdo1))));
            Assert.Equal("g2SIJg==", Convert.ToBase64String(HostToNetworkBytes(hash.Hash(sdo1b))));
            Assert.Equal("M3v27A==", Convert.ToBase64String(HostToNetworkBytes(hash.Hash(sdo1c))));
            Assert.Equal("kTS4zg==", Convert.ToBase64String(HostToNetworkBytes(hash.Hash(sdo2))));

            XorBifunctor bifunctor = new();
            bifunctor.Apply(hash.Hash(sdo1));
            Assert.Equal("g2SIJg==", Convert.ToBase64String(HostToNetworkBytes(bifunctor.Hash)));
            bifunctor.Apply(hash.Hash(sdo2));
            Assert.Equal("ElAw6A==", Convert.ToBase64String(HostToNetworkBytes(bifunctor.Hash)));
        }

        [Fact]
        public void AddTest()
        {
            AddBifunctor bifunctor = new();

            int x1 = 5432;
            int x2 = 56154462;
            var x1b = BitConverter.GetBytes(x1);
            var x2b = BitConverter.GetBytes(x2);
            bifunctor.Apply(x1b);
            Assert.Equal(x1, BitConverter.ToInt32(bifunctor.Hash));
            bifunctor.Apply(x2b);
            Assert.Equal(x1 + x2, BitConverter.ToInt32(bifunctor.Hash));
            Assert.Equal("A1julg==", Convert.ToBase64String(HostToNetworkBytes(bifunctor.Hash)));
        }

        [Fact]
        public void AddTestStableHash()
        {
            StableHash hash = new StableHash();
            
            SimpleDataObject sdo1 = new("ape", 0, "string");
            SimpleDataObject sdo1b = new("ape", 86475, "string");
            SimpleDataObject sdo1c = new("ape", 0, "updated");
            SimpleDataObject sdo2 = new("bee", 0, "string");
            Assert.Equal("g2SIJg==", Convert.ToBase64String(HostToNetworkBytes(hash.Hash(sdo1))));
            Assert.Equal("g2SIJg==", Convert.ToBase64String(HostToNetworkBytes(hash.Hash(sdo1b))));
            Assert.Equal("M3v27A==", Convert.ToBase64String(HostToNetworkBytes(hash.Hash(sdo1c))));
            Assert.Equal("kTS4zg==", Convert.ToBase64String(HostToNetworkBytes(hash.Hash(sdo2))));

            AddBifunctor bifunctor = new();
            bifunctor.Apply(hash.Hash(sdo1));
            Assert.Equal("g2SIJg==", Convert.ToBase64String(HostToNetworkBytes(bifunctor.Hash)));
            bifunctor.Apply(hash.Hash(sdo2));
            Assert.Equal("FJlA9A==", Convert.ToBase64String(HostToNetworkBytes(bifunctor.Hash)));
        }

    }
}
