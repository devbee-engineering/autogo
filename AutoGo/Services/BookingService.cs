using Agoda.IoC.Core;
using AutoGo.Data.DbContexts;
using AutoGo.Data.Entities;
using AutoGo.Services.Model;

namespace AutoGo.Services
{
    public interface IBookingService
    {
        Task Assign(long id, int toUserId);
        Task Attach(long id, string attachmentId);
        Task Complete(long id);
        Task<long> Create(BookingCreationModel bookingCreationModel);
        Task<Booking> Get(long id);
        Task Start(long id);
    }

    [RegisterPerRequest]
    public class BookingService(ILogger<BookingService> logger, IBookingRepository bookingRepository, IUserService userService) : IBookingService
    {
        public async Task<long> Create(BookingCreationModel bookingCreationModel)
        {
            var createdByUser = await userService.GetUser(bookingCreationModel.createdById);

            var newBooking = new Booking()
            {
                CreatedById = createdByUser.Id,
                OrganizationId = createdByUser.OrganizationIds.First(),
                VoiceFileId = bookingCreationModel.VoiceFileId,
                MobileNumber = bookingCreationModel.MobileNumber,
            };

            var bookingId = await bookingRepository.Add(newBooking);

            return bookingId;
        }

        public async Task<Booking> Get(long id)
        {
            return await bookingRepository.Get(id);
        }

        public async Task Assign(long id, int toUserId)
        {
            await bookingRepository.Assign(id, toUserId);
        }

        public async Task Start(long id)
        {
            await bookingRepository.Start(id);
        }

        public async Task Complete(long id)
        {
            await bookingRepository.Complete(id);
        }

        public async Task Attach(long id, string attachmentId)
        {
            await bookingRepository.Attach(id, attachmentId);
        }
    }
}
