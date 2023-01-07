using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for this help pop-up
    /// </summary>
    public partial class TeacherAssignmentHelp : Page
    {
        public TeacherAssignmentHelp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Navigates to web address in a text block.
        /// </summary>
        /// <param name="sender">The sending control</param>
        /// <param name="e">The event arguments</param>
        private void NavigateLink(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Hyperlink address = (Hyperlink)sender;
            string navigateUri = address.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }
    }
}
