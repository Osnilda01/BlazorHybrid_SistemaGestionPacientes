using System.Runtime.InteropServices;

namespace MauiBlazorHybrid
{
	public partial class App : Application
	{
        
        public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

		//Para cambiar los valores de la ventana:
		protected override Window CreateWindow(IActivationState activationState)
		{
			var window = base.CreateWindow(activationState);

			window.Width = 1024; //Ancho de la ventana
			window.Height = 600; //Largo de la ventana
			window.Title = "MediGestor"; //Titulo de la ventana
            

            return window;
		}
	}
}
