using System;
using MyNUnitFramework.Attribute;

namespace TestedAssembly
{
    public class TestedClass2
    {
        public double MustBePi { get; set; } = 2.67;

        [Test]
        public void SimpleTest()
        {
            MustBePi = 3.14;
        }

        [Test]
        public void SimpleFailTest()
        {
            throw new Exception("SimpleFailTest");
        }
    }
}