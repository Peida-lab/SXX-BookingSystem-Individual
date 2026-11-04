
using SXX.BookingSystem.Enums;
using SXX.BookingSystem.Models;
using SXX.BookingSystem.Repositories;

namespace SXX.BookingSystem.Services;

public class ReturnService
{
    private readonly BookingRepository _repository;

    public ReturnService(BookingRepository repository)
    {
        _repository = repository;
    }

    public Booking? GetBooking(string bookingNumber)
    {
        return _repository.GetBooking(bookingNumber);
    }

    public ReturnSaveResult RegisterReturn(
        string bookingNumber,
        DateTimeOffset returnedAt)
    {
        return _repository.TryRegisterReturn(bookingNumber, returnedAt);
    }
}
