﻿using System;
using MyNUnitFramework.Attribute;

namespace TestedAssembly
{
    [Test]
    public class FailStartClass
    {
        public static bool IsTestRun { get; private set; }

        [Before]
        public void FailSetUp()
        {
            throw new Exception("FailSetUp");
        }

        [Test]
        public void Test()
        {
            IsTestRun = true;
        }
    }
}