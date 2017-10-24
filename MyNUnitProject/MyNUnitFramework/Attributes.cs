using System;

namespace MyNUnitFramework
{
    namespace Attributes
    {
        // Annotates test method.
        public class TestAttribute : Attribute
        {
            public Type Expected { get; set; }

            public string IgnoreWithCause { get; set; }
        }

        // Annotates methods being run before any test method.
        public class BeforeAttribute : Attribute {}

        // Annotates methods being run after any test method.
        public class AfterAttribute : Attribute {}

        // Annotates methods being run after testing any class.
        public class BeforeClassAttribute : Attribute {}

        // Annotates methods being run after testing any class.
        public class AfterClassAttribute : Attribute {}
    }
}
