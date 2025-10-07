using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Services
{
    public class ForumDisplayService: IForumDisplayService
    {
        private readonly AssemblerDependencies _deps;
        private readonly ICommentReportService _reportService;

        public ForumDisplayService(AssemblerDependencies dependencies, ICommentReportService reportService) 
        {
            _deps = dependencies;
            _reportService = reportService;
        }
        public ForumDTO AssembleForumDTO(Forum forum)
        {
            LinkBaseForumData(forum);
            var dto = new ForumDTO(forum);
            var comments = LoadAndAssembleComments(forum);

            dto.Comments = new ObservableCollection<CommentDTO>(comments);
            dto.CommentCount = dto.Comments.Count;

            dto.OwnerCommentsCount = dto.Comments.Count(c => c.IsFromOwner);
            dto.GuestCommentsCount = dto.Comments.Count(c => c.IsFromVisitor);

            dto.IsVeryUseful = dto.OwnerCommentsCount >= 10 && dto.GuestCommentsCount >= 20;

            dto.CanBeClosed = CalculateCanBeClosed(forum);

            return dto;
        }
        private void LinkBaseForumData(Forum forum)
        {
            if (forum.Location == null)
                forum.Location = _deps.LocationRepository.GetById(forum.LocationId);
            if (forum.Creator == null)
                forum.Creator = _deps.UserRepository.GetById(forum.CreatorId);
        }
        private List<CommentDTO> LoadAndAssembleComments(Forum forum)
        {
            var allReservations = _deps.ReservationRepository.GetAll();
            var allAccommodations = _deps.AccommodationRepository.GetAll();
            var allComments = _deps.CommentRepository.GetAll();
            var commentLinks = _deps.ForumCommentRepository.GetByForumId(forum.Id);

            return commentLinks
                .Select(link => FindAndAssembleComment(link, forum, allComments, allReservations, allAccommodations))
                .Where(commentDto => commentDto != null) 
                .ToList();
        }
        private CommentDTO FindAndAssembleComment(ForumComment link, Forum forum, List<Comment> allComments, List<Reservation> allReservations, List<Accommodation> allAccommodations)
        {
            var comment = allComments.FirstOrDefault(c => c.Id == link.CommentId);
            if (comment == null) return null;

            comment.User = _deps.UserRepository.GetById(comment.User.Id);
            if (comment.User == null) return null;

            var context = new CommentStatusContext(comment, forum.Location, allReservations, allAccommodations);
            return MapCommentToDTO(context);
        }
        private CommentDTO MapCommentToDTO(CommentStatusContext context)
        {

            var dto = new CommentDTO(context.Comment);

            SetAuthorStatus(dto, context);
            SetReportStatus(dto, context.Comment.Id);
            SetStayInfoText(dto, context);

            return dto;
        }
        private void SetAuthorStatus(CommentDTO dto, CommentStatusContext context)
        {
            dto.IsFromVisitor = IsUserVisitor(context);
            dto.IsFromOwner = context.Comment.User.Role == UserRole.OWNER;
        }
        private void SetReportStatus(CommentDTO dto, int commentId)
        {
            dto.CanReport = Session.CurrentUser.Role == UserRole.OWNER && !dto.IsFromOwner;
            dto.ReportsCount = _reportService.GetReportsCount(commentId);
        }
        private void SetStayInfoText(CommentDTO dto, CommentStatusContext context)
        {
            if (dto.IsFromVisitor)
            {
                var reservation = FindLatestReservationOnLocation(context);
                dto.StayDatesText = (reservation != null)
                    ? $"{reservation.StartDate:dd-MM-yyyy} - {reservation.EndDate:dd-MM-yyyy}"
                    : "Visited on an unknown date"; 
            }
            else if (dto.IsFromOwner)
            {
                dto.StayDatesText = "Property owner";
            }
        }
        private Reservation FindLatestReservationOnLocation(CommentStatusContext context)
        {
            return context.AllReservations
                .Where(r => r.GuestId == context.Comment.User.Id &&
                            context.AllAccommodations.Any(a =>
                                a.Id == r.AccommodationId &&
                                a.GeoLocation.Id == context.Location.Id))
                .OrderByDescending(r => r.EndDate)
                .FirstOrDefault();
        }
        private bool IsUserVisitor(CommentStatusContext context)
        {
            return context.AllReservations.Any(r =>
                r.GuestId == context.Comment.User.Id &&
                context.AllAccommodations.Any(a => a.Id == r.AccommodationId && a.GeoLocation.Id == context.Location.Id));
        }
        private bool CalculateCanBeClosed(Forum forum)
        {
            return !forum.IsClosed && forum.CreatorId == Session.CurrentUser.Id;
        }
    }
    public class AssemblerDependencies
    {
        public IUserRepository UserRepository { get; }
        public ILocationRepository LocationRepository { get; }
        public ICommentRepository CommentRepository { get; }
        public IForumCommentRepository ForumCommentRepository { get; }
        public IReservationRepository ReservationRepository { get; }
        public IAccommodationRepository AccommodationRepository { get; }

        public AssemblerDependencies(IUserRepository ur, ILocationRepository lr, ICommentRepository cr,
                                     IForumCommentRepository fcr, IReservationRepository rr, IAccommodationRepository ar)
        {
            UserRepository = ur;
            LocationRepository = lr;
            CommentRepository = cr;
            ForumCommentRepository = fcr;
            ReservationRepository = rr;
            AccommodationRepository = ar;
        }
    }
    public class CommentStatusContext
    {
        public Comment Comment { get; }
        public Location Location { get; }
        public List<Reservation> AllReservations { get; }
        public List<Accommodation> AllAccommodations { get; }

        public CommentStatusContext(Comment comment, Location location, List<Reservation> reservations, List<Accommodation> accommodations)
        {
            Comment = comment;
            Location = location;
            AllReservations = reservations;
            AllAccommodations = accommodations;
        }
    }
}