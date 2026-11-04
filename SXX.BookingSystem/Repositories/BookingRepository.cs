
using SXX.BookingSystem.Enums;
using SXX.BookingSystem.Models;

namespace SXX.BookingSystem.Repositories;

public class BookingRepository
{
    private readonly List<Booking> _bookings = new();
    private readonly object _lock = new();

    public bool SimulateSaveFailure { get; set; }

    public void AddBooking(Booking booking)
    {
        lock (_lock)
        {
            _bookings.Add(booking);
        }
    }

    public Booking? GetBooking(string bookingNumber)
    {
        lock (_lock)
        {
            return _bookings.FirstOrDefault(
                booking => booking.BookingNumber == bookingNumber);
        }
    }

    public ReturnSaveResult TryRegisterReturn(
        string bookingNumber,
        DateTimeOffset returnedAt)
    {
        lock (_lock)
        {
            var booking = _bookings.FirstOrDefault(
                b => b.BookingNumber == bookingNumber);

            if (booking is null ||
                booking.Status != BookingStatus.CheckedOut)
            {
                return ReturnSaveResult.InvalidStatus;
            }

            if (SimulateSaveFailure)
            {
                return ReturnSaveResult.SaveFailed;
            }

            booking.MarkAsReturned(returnedAt);

            return ReturnSaveResult.Success;
        }
    }
}
