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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AuctionClient
{
    /// <summary>
    /// Логика взаимодействия для LoadingAnim.xaml
    /// </summary>
    public partial class LoadingAnim : Page
    {
        public LoadingAnim()
        {
            InitializeComponent();
        }

        public static LoadingAnim loading;

        public void SetLoading()
        {
            loading = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                By = 360,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                RepeatBehavior = RepeatBehavior.Forever
            };
            //rotate.BeginAnimation(RotateTransform.AngleProperty, animation);
        }
    }
}
