using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design_Pattern.Patterns.AbstractFactory
{
    public class WindowsButton : IButton
    {
        public void Render() => Console.WriteLine("Rendering Windows Button");
    }

    public class WindowsCheckbox : ICheckbox
    {
        public void Render() => Console.WriteLine("Rendering Windows Checkbox");
    }
    public class WindowsFactory:GUIFactory
    {
        public IButton CreateButton() => new WindowsButton();
        public ICheckbox CreateCheckbox() => new WindowsCheckbox();
    }
}
