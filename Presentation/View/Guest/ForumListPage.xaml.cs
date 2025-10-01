using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.View.Guest;
using BookingApp.Presentation.ViewModel.Guest;

namespace BookingApp.Presentation.View.Guest
{ 
    public partial class ForumListPage : UserControl
    {
        private ForumListViewModel? _viewModel;

        public ForumListPage()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
            this.Unloaded += OnUnloaded;
        }
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = this.DataContext as ForumListViewModel;
            if (_viewModel != null)
            {
                _viewModel.CreateNewForumRequested += OnCreateNewForumRequested;
            }
        }
        private void OnCreateNewForumRequested()
        {
            var createForumWindow = new CreateForumView();

            createForumWindow.Owner = Window.GetWindow(this);

            createForumWindow.ShowDialog();
        }
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CreateNewForumRequested -= OnCreateNewForumRequested;
            }
        }
    }
}
