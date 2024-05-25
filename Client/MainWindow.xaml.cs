using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Communicator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	MainViewModel MainViewModel;
    public MainWindow()
    {
        InitializeComponent(); 
        this.ResizeMode = ResizeMode.NoResize;
		MainViewModel = new MainViewModel();

		DataContext = MainViewModel;
        PreviewKeyDown += MessageTextBox_KeyDown;
		MainViewModel.Init();
	}

	private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
        {
            Button_Click(null, null);
			
		}
	}

	private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{

	}

	private void Button_Click(object? sender, RoutedEventArgs? e)
	{
		string text = MessageTextBox.Text.Trim();
		if (text.Length == 0) return; 
		Message message = new Message(text, DateTime.Now, "Adam", true);
        //MainViewModel.AddMessage(message);
        MessageTextBox.Text = "";
        MainViewModel.client.Send(message);
	}
}