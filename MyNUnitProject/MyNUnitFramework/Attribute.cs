using System;

namespace MyNUnitFramework
{
    namespace Attribute
    {
        // Annotates test method.
        public class TestAttribute : System.Attribute
        {
            public Type Expected { get; set; }

            public string IgnoreWithCause { get; set; }
        }

        public class BeforeAttribute : System.Attribute {}

        public class AfterAttribute : System.Attribute {}

        public class BeforeClassAttribute : System.Attribute {}

        public class AfterClassAttribute : System.Attribute {}
    }
}
