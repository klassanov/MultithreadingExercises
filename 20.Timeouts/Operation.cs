﻿using System;
using System.Collections.Generic;
using System.Text;

namespace _20.Timeouts
{
    public class Operation: IOperation
    {
        public void Execute()
        {
            Console.WriteLine("Operation Executed");
        }
    }
}
