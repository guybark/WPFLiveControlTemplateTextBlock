using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFLiveSetting
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            // Find the TextBlock in the ControlTemplate.
            var tb = (TextBlock)GetDescendantFromName(LiveContent, "TestTextBlock");
            if (tb != null)
            {
                // Set new text on the TextBlock in the ControlTemplate.
                tb.Text = "Status has changed.";

                // Now raise a LiveRegionChanged event.
                var peer = FrameworkElementAutomationPeer.FromElement(tb);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
                }
            }
        }

        // Helper function to find an element within the ControlTemplate.
        public static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            if (count < 1)
            {
                return null;
            }

            FrameworkElement element;
            for (int i = 0; i < count; i++)
            {
                element = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (element != null)
                {
                    if (element.Name == name)
                    {
                        return element;
                    }

                    element = GetDescendantFromName(element, name);
                    if (element != null)
                    {
                        return element;
                    }
                }
            }

            return null;
        }
    }

    // Class to force a TextBlock into the Control view of the UI Automation tree.
    public class LiveControlTemplateTextBlock : TextBlock
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new LiveControlTemplateAutomationPeer(this);
        }

        public class LiveControlTemplateAutomationPeer : TextBlockAutomationPeer
        {
            public LiveControlTemplateAutomationPeer(LiveControlTemplateTextBlock owner) :
                base(owner)
            { }

            protected override bool IsControlElementCore()
            {
                return true;
            }

            // For completeness, add the TextBlock to UIA's Content view too.
            protected override bool IsContentElementCore()
            {
                return true;
            }
        }
    }
}
