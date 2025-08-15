using BookingApp.Domain.Interfaces;
using BookingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class Injector
    {
        private static Dictionary<Type, object> _implementations = new Dictionary<Type, object>
        {
            { typeof(IUserRepository), new UserRepository() },
            { typeof(IUserService), new UserService(CreateInstance<IUserRepository>()) },
            { typeof(IAccommodationRepository), new AccommodationRepository() },
            { typeof(IAccommodationService), new AccommodationService(CreateInstance<IAccommodationRepository>()) },
            { typeof(IAccommodationImageRepository), new AccommodationImageRepository() },
            { typeof(IAccommodationImageService), new AccommodationImageService(CreateInstance<IAccommodationImageRepository>()) },
            { typeof(ICommentRepository), new CommentRepository() },
            { typeof(ICommentService), new CommentService(CreateInstance<ICommentRepository>()) },
            { typeof(ILocationRepository), new LocationRepository() },
            { typeof(ILocationService), new LocationService(CreateInstance<ILocationRepository>()) },


             // Add more implementations here
  };

        public static T CreateInstance<T>()
        {
            Type type = typeof(T);

            if (_implementations.ContainsKey(type))
            {
                return (T)_implementations[type];
            }

            throw new ArgumentException($"No implementation found for type {type}");
        }
    }
}
