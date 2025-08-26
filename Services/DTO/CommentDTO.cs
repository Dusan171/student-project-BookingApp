using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class CommentDTO : INotifyPropertyChanged
    {
       
        private int _id;
        private DateTime _creationTime;
        private string _text;
        private int _userId;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
       
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public DateTime CreationTime
        {
            get => _creationTime;
            set
            {
                if (_creationTime != value)
                {
                    _creationTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();
                }
            }
        }

 
        public CommentDTO() { }

        public CommentDTO(Comment c)
        {
            _id = c.Id;
            _creationTime = c.CreationTime;
            _text = c.Text;
            _userId = c.User.Id;
        }

    
        public CommentDTO FromModel(Comment model)
        {
            return new CommentDTO
            {
                Id = model.Id,
                CreationTime = model.CreationTime,
                Text = model.Text,
                UserId = model.User?.Id ?? 0
            };
        }

    
        public Comment ToComment()
        {
            return new Comment
            {
                Id = this.Id,
                CreationTime = this.CreationTime,
                Text = this.Text,
                User = new User { Id = this.UserId }
            };
        }
    }
}

