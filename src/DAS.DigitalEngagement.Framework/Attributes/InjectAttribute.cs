﻿using System;
using Microsoft.Azure.WebJobs.Description;

namespace DAS.DigitalEngagement.Framework.Attributes
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}
