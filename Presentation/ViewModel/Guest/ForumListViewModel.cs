using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class ForumListViewModel : ViewModelBase
    {
        private readonly IForumService _forumService;

        public ObservableCollection<ForumDTO> Forums { get; set; }
        public ICommand OpenForumCommand { get; }
        public ICommand CreateNewForumCommand { get; }
        public static event Action<ForumDTO> ViewForumRequested;

        public event Action CreateNewForumRequested;

        public ForumListViewModel()
        {
            _forumService = Injector.CreateInstance<IForumService>();
            Forums = new ObservableCollection<ForumDTO>();

            OpenForumCommand = new RelayCommand(OpenForum);
            CreateNewForumCommand = new RelayCommand(CreateNewForum);

            LoadForums();
        }
        private void LoadForums()
        {
            Forums.Clear();
            var forumsFromService = _forumService.GetAll();
            foreach (var forum in forumsFromService)
            {
                Forums.Add(forum);
            }
        }
        private void OpenForum(object parameter)
        {
            if (parameter is ForumDTO selectedForum)
            {
                ViewForumRequested?.Invoke(selectedForum);
            }
        }
        private void CreateNewForum(object obj)
        {
            CreateNewForumRequested?.Invoke();
        }
    }
}
