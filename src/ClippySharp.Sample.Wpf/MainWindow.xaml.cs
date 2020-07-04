using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClippySharp.Sample.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Agent Agent;
        
        public MainWindow()
        {
            AgentEnvirontment.Current.Initialize(new AgentDelegate(), new SoundPlayer());
            InitializeComponent();

            foreach (var item in AgentEnvirontment.Current.GetAgents())
            {
                agentPopupButton.Items.Add(item);
            }

            agentPopupButton.SelectionChanged += AgentPopupButton_SelectionChanged;

            animationPopupButton.SelectionChanged += AnimationPopupButton_SelectionChanged;

            agentPopupButton.SelectedIndex = 0;

            AgentPopupButton_SelectionChanged(null, null);
        }

        private void AnimationPopupButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (animationPopupButton.SelectedItem == null)
                return;
            var animation = animationPopupButton.SelectedItem.ToString ();
            Agent.Play(animation);
        }

        private void AgentPopupButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var agentName = agentPopupButton.SelectedItem.ToString ();

            if (this.Agent != null)
            {
                this.Agent.NeedsRender -= Agent_NeedsRender;
                this.Agent.Dispose();
            }

            Agent = new Agent(agentName);

            Agent.NeedsRender += Agent_NeedsRender;
            Agent.RefreshImage();
            Agent.Animate();

            animationPopupButton.Items.Clear();
            foreach (var animation in Agent.GetAnimations())
            {
                //fill with all animation
                animationPopupButton.Items.Add (animation);
            }
            animationPopupButton.SelectedIndex = 0;
            AnimationPopupButton_SelectionChanged(null, null);
        }

        private void Agent_NeedsRender(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => {
                var image = Agent.GetCurrentImage();
                if (image != null)
                    imagTest.Source = image.NativeObject as ImageSource;
            });
        }
    }
}
