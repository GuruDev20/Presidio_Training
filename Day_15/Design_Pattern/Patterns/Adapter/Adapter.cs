﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.Adapter
{
    public class Adapter:ITarget
    {
        private readonly Adaptee _adaptee;
        public Adapter(Adaptee adaptee)
        {
            _adaptee = adaptee;
        }
        public void Request()
        {
            _adaptee.SpecificRequest();
        }
    }
}
