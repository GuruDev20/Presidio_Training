using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.AbstractFactory
{
    public class MacButton : IButton
    {
        public void Render() => Console.WriteLine("Rendering Mac Button");
    }

    public class MacCheckbox : ICheckbox
    {
        public void Render() => Console.WriteLine("Rendering Mac Checkbox");
    }
    public class MacFactory:GUIFactory
    {
        public IButton CreateButton() => new MacButton();
        public ICheckbox CreateCheckbox() => new MacCheckbox();
    }
}
