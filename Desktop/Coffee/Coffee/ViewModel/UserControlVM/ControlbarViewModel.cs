using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Coffee.ViewModel.UserControlVM
{
    public class ControlbarViewModel : BaseViewModel
    {
        public ICommand closeWindowCommand { get; set; }
        public ICommand minimizeWindowCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }

        public ControlbarViewModel()
        {
            closeWindowCommand = new RelayCommand<UserControl>((p) => { return p != null; }, (p) =>
            {
                FrameworkElement window = getWindowParent(p);
                var w = (window as Window);
                if (w != null)
                {
                    w.Close();
                }
            });

            minimizeWindowCommand = new RelayCommand<UserControl>((p) => { return p != null; }, (p) =>
            {
                FrameworkElement window = getWindowParent(p);
                var w = (window as Window);
                if (w != null)
                {
                    if (w.WindowState != WindowState.Minimized)
                    {
                        w.WindowState = WindowState.Minimized;
                    }
                    else
                    {
                        w.WindowState = WindowState.Maximized;
                    }
                }
            });

            MouseMoveWindowCommand = new RelayCommand<UserControl>((p) => { return p != null; }, (p) =>
            {
                FrameworkElement window = getWindowParent(p);
                var w = (window as Window);
                if (w != null)
                {
                    w.DragMove();
                }
            });
        }

        FrameworkElement getWindowParent(UserControl p)
        {
            FrameworkElement parent = p;
            while (parent.Parent != null)
            {
                parent = parent.Parent as FrameworkElement;
            }
            return parent;
        }
    }
}
