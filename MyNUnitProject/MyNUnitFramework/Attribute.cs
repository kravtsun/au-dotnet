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

        // Annotates methods being run before any test method.
        public class BeforeAttribute : System.Attribute {}

        // Annotates methods being run after any test method.
        public class AfterAttribute : System.Attribute {}

        // Annotates methods being run after testing any class.
        public class BeforeClassAttribute : System.Attribute {}

        // Annotates methods being run after testing any class.
        public class AfterClassAttribute : System.Attribute {}
    }
}
