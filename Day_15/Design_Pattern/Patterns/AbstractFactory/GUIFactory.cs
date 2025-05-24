using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.AbstractFactory
{
    public interface GUIFactory
    {
        IButton CreateButton();
        ICheckbox CreateCheckbox();
    }
}
