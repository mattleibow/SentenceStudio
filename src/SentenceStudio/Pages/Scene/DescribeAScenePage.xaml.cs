using System.Diagnostics;

namespace SentenceStudio.Pages.Scene;

public partial class DescribeAScenePage : ContentPage
{

	public DescribeAScenePage(DescribeAScenePageModel model)
	{
		InitializeComponent();

		BindingContext = model;

		this.SizeChanged += UpdateLayout;
	}

	private const double minPageWidth = 800;
    private void UpdateLayout(object sender, EventArgs e)
    {
        double currentWidth = ((VisualElement)sender).Width + Shell.Current.FlyoutWidth; // don't get this, instead get the window size
        
        Debug.WriteLine($"currentWidth: {currentWidth}, " +
        $"PageWidth: {((VisualElement)sender).Width}, " +
            $"Shell.Current.FlyoutWidth: {Shell.Current.FlyoutWidth}"   );
		
        if (currentWidth < minPageWidth)
        {
            // change the second column to 0
			// MainGrid.ColumnDefinitions[1].Width = new GridLength(0);
			VisualStateManager.GoToState(MainGrid, "Narrow");
        }
        else 
        {
            // set the second column to *
			VisualStateManager.GoToState(MainGrid, "Wide");
        }
    }
}